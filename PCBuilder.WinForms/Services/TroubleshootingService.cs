using Microsoft.Data.SqlClient;
using PCBuilder.Data;

namespace PCBuilder.Services;

/// <summary>A node in the SQL-backed decision tree.</summary>
public sealed record TroubleStep(
    int NodeID,
    string NodeType,
    string Question,
    string[] Options,
    int[] Next,
    string? Diagnosis = null,
    string? Action = null,
    bool IsRoot = false);

/// <summary>
/// Reads the decision tree from SQL Server. The navigation algorithm remains in C#:
/// selecting an answer follows that answer's NextNodeID, and question-depth/cycle
/// checks protect the user from reaching a result too early or looping forever.
/// </summary>
public sealed class TroubleshootingService
{
    private const int MinimumQuestionNodesBeforeResult = 4;
    private readonly Dictionary<int, TroubleStep> _steps;

    public int Root { get; }

    public TroubleshootingService()
    {
        var nodes = new Dictionary<int, NodeBuilder>();
        const string sql = @"SELECT n.NodeID, n.NodeType, n.Question, n.Diagnosis, n.[Action], n.IsRoot,
        o.OptionText, o.NextNodeID, o.SortOrder
        FROM dbo.TroubleshootingNodes AS n
        LEFT JOIN dbo.TroubleshootingOptions AS o
        ON o.NodeID = n.NodeID
        AND EXISTS (
        SELECT 1
        FROM dbo.TroubleshootingNodes AS destination
        WHERE destination.NodeID = o.NextNodeID
        AND destination.IsActive = 1
        )
        WHERE n.IsActive = 1
        ORDER BY n.NodeID, o.SortOrder;";

        using var connection = Db.CreateConnection();
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var id = Convert.ToInt32(reader["NodeID"]);
                if (!nodes.TryGetValue(id, out var node))
                {
                    node = new NodeBuilder
                    {
                        NodeID = id,
                        NodeType = Convert.ToString(reader["NodeType"]) ?? "Q",
                        Question = Convert.ToString(reader["Question"]) ?? string.Empty,
                        Diagnosis = reader["Diagnosis"] == DBNull.Value ? null : Convert.ToString(reader["Diagnosis"]),
                        Action = reader["Action"] == DBNull.Value ? null : Convert.ToString(reader["Action"]),
                        IsRoot = Convert.ToBoolean(reader["IsRoot"])
                    };
                    nodes.Add(id, node);
                }

                if (reader["OptionText"] != DBNull.Value && reader["NextNodeID"] != DBNull.Value)
                {
                    node.Options.Add((
                        Convert.ToInt32(reader["SortOrder"]),
                        Convert.ToString(reader["OptionText"]) ?? string.Empty,
                        Convert.ToInt32(reader["NextNodeID"])));
                }
            }
        }

        if (nodes.Count == 0)
        {
            throw new InvalidOperationException(
                "Chưa có dữ liệu cây chẩn đoán trong SQL Server. Hãy chạy file Upgrade_Troubleshooting_DecisionTree_Depth.sql trước.");
        }

        var roots = nodes.Values.Where(n => n.IsRoot && n.NodeType == "Q").ToList();
        if (roots.Count != 1)
        {
            throw new InvalidOperationException(
                $"Cây chẩn đoán cần đúng một node gốc đang hoạt động; hiện tìm thấy {roots.Count} node.");
        }

        _steps = nodes.Values.ToDictionary(
            n => n.NodeID,
            n => new TroubleStep(
                n.NodeID,
                n.NodeType,
                n.Question,
                n.Options.OrderBy(o => o.SortOrder).Select(o => o.Text).ToArray(),
                n.Options.OrderBy(o => o.SortOrder).Select(o => o.NextNodeID).ToArray(),
                n.Diagnosis,
                n.Action,
                n.IsRoot));
        Root = roots[0].NodeID;

        if (_steps[Root].Options.Length == 0)
            throw new InvalidOperationException("Node gốc chưa có lựa chọn nhóm sự cố. Hãy kiểm tra TroubleshootingOptions.");

        ValidateDecisionDepth();
    }

    public TroubleStep Get(int id)
        => _steps.TryGetValue(id, out var step)
            ? step
            : throw new InvalidOperationException("Node không tồn tại hoặc đã bị vô hiệu hóa. Hãy bắt đầu chẩn đoán lại.");

    public int Follow(int id, int optionIndex)
    {
        var step = Get(id);
        if (step.NodeType != "Q" || optionIndex < 0 || optionIndex >= step.Next.Length)
            throw new ArgumentOutOfRangeException(nameof(optionIndex), "Lựa chọn không hợp lệ cho node hiện tại.");
        return step.Next[optionIndex];
    }

    public bool IsTerminal(int id) => string.Equals(Get(id).NodeType, "R", StringComparison.OrdinalIgnoreCase);

    /// <summary>Returns the current question's 1-based depth within its group; the group question is step 1.</summary>
    public int CountQuestionNodes(IEnumerable<int> visitedNodeIds, int currentNodeId)
    {
        var count = visitedNodeIds
            .Append(currentNodeId)
            .Distinct()
            .Select(id => Get(id))
            .Count(step => !step.IsRoot && !IsTerminal(step.NodeID));
        return count;
    }

    private void ValidateDecisionDepth()
    {
        var root = _steps[Root];
        foreach (var groupNodeId in root.Next)
        {
            var groupNode = Get(groupNodeId);
            if (groupNode.NodeType != "Q")
                throw new InvalidOperationException("Mỗi nhóm sự cố phải dẫn tới một node câu hỏi, không được dẫn thẳng tới kết luận.");

            WalkAndValidate(groupNodeId, 1, new HashSet<int>());
        }
    }

    private void WalkAndValidate(int nodeId, int questionCount, HashSet<int> currentPath)
    {
        if (!currentPath.Add(nodeId))
            throw new InvalidOperationException($"Phát hiện vòng lặp trong cây chẩn đoán tại NodeID {nodeId}. Hãy sửa liên kết trong SQL.");

        try
        {
            var step = Get(nodeId);
            if (IsTerminal(nodeId))
            {
                if (questionCount < MinimumQuestionNodesBeforeResult)
                {
                    throw new InvalidOperationException(
                        $"Nhánh chẩn đoán dẫn tới kết luận ở quá sớm (mới có {questionCount} node câu hỏi). " +
                        $"Mỗi nhánh cần ít nhất {MinimumQuestionNodesBeforeResult} node câu hỏi trước khi kết luận.");
                }
                return;
            }

            // A disabled result may leave one question without choices. The form reports that
            // local data issue to the user; it should not prevent all other groups from loading.
            foreach (var nextId in step.Next)
            {
                var next = Get(nextId);
                WalkAndValidate(nextId, questionCount + (next.NodeType == "Q" ? 1 : 0), currentPath);
            }
        }
        finally
        {
            currentPath.Remove(nodeId);
        }
    }

    private sealed class NodeBuilder
    {
        public int NodeID { get; init; }
        public string NodeType { get; init; } = "Q";
        public string Question { get; init; } = string.Empty;
        public string? Diagnosis { get; init; }
        public string? Action { get; init; }
        public bool IsRoot { get; init; }
        public List<(int SortOrder, string Text, int NextNodeID)> Options { get; } = new();
    }
}
