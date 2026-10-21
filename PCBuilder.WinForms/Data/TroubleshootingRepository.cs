using Microsoft.Data.SqlClient;
using System.Data;

namespace PCBuilder.Data;

public sealed record TroubleshootingCategory(int NodeID, string Name);

public sealed record TroubleshootingIssue(
    int NodeID,
    int CategoryNodeID,
    string CategoryName,
    string Question,
    string Diagnosis,
    string Action,
    bool IsActive);

/// <summary>
/// CRUD for terminal result nodes in a multi-step decision tree. Search traverses
/// all descendants of each top-level group; adding/moving a result attaches it to
/// a final question node in that group instead of short-circuiting the decision tree.
/// </summary>
public sealed class TroubleshootingRepository
{
    public List<TroubleshootingCategory> GetCategories()
    {
        const string sql = @"
SELECT category.NodeID, rootOption.OptionText AS CategoryName
FROM dbo.TroubleshootingNodes AS rootNode
JOIN dbo.TroubleshootingOptions AS rootOption ON rootOption.NodeID = rootNode.NodeID
JOIN dbo.TroubleshootingNodes AS category ON category.NodeID = rootOption.NextNodeID
WHERE rootNode.IsRoot = 1
  AND rootNode.NodeType = 'Q'
  AND rootNode.IsActive = 1
  AND category.NodeType = 'Q'
  AND category.IsActive = 1
ORDER BY rootOption.SortOrder;";

        using var connection = Db.CreateConnection();
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        var result = new List<TroubleshootingCategory>();
        while (reader.Read())
            result.Add(new TroubleshootingCategory(
                Convert.ToInt32(reader["NodeID"]),
                Convert.ToString(reader["CategoryName"]) ?? string.Empty));
        return result;
    }

    public List<TroubleshootingIssue> Search(string? searchText = null, int? categoryNodeId = null)
    {
        const string sql = @"
;WITH Categories AS
(
    SELECT category.NodeID AS CategoryNodeID,
           rootOption.OptionText AS CategoryName,
           rootOption.SortOrder AS CategorySort
    FROM dbo.TroubleshootingNodes AS rootNode
    JOIN dbo.TroubleshootingOptions AS rootOption ON rootOption.NodeID = rootNode.NodeID
    JOIN dbo.TroubleshootingNodes AS category ON category.NodeID = rootOption.NextNodeID
    WHERE rootNode.IsRoot = 1 AND rootNode.NodeType = 'Q' AND rootNode.IsActive = 1
      AND category.NodeType = 'Q' AND category.IsActive = 1
), Reachable AS
(
    SELECT c.CategoryNodeID, c.CategoryName, c.CategorySort,
           c.CategoryNodeID AS NodeID,
           CAST('/' + CONVERT(VARCHAR(20), c.CategoryNodeID) + '/' AS VARCHAR(MAX)) AS NodePath,
           0 AS Depth
    FROM Categories AS c

    UNION ALL

    SELECT r.CategoryNodeID, r.CategoryName, r.CategorySort,
           destination.NodeID,
           CAST(r.NodePath + CONVERT(VARCHAR(20), destination.NodeID) + '/' AS VARCHAR(MAX)),
           r.Depth + 1
    FROM Reachable AS r
    JOIN dbo.TroubleshootingOptions AS edge ON edge.NodeID = r.NodeID
    JOIN dbo.TroubleshootingNodes AS destination ON destination.NodeID = edge.NextNodeID
      AND (destination.NodeType = 'R' OR destination.IsActive = 1)
    WHERE r.Depth < 25
      AND CHARINDEX('/' + CONVERT(VARCHAR(20), destination.NodeID) + '/', r.NodePath) = 0
), IssueGroups AS
(
    SELECT DISTINCT r.CategoryNodeID, r.CategoryName, r.CategorySort, n.NodeID AS IssueNodeID
    FROM Reachable AS r
    JOIN dbo.TroubleshootingNodes AS n ON n.NodeID = r.NodeID AND n.NodeType = 'R'
)
SELECT issue.NodeID,
       groups.CategoryNodeID,
       groups.CategoryName,
       issue.Question,
       issue.Diagnosis,
       issue.[Action],
       issue.IsActive
FROM IssueGroups AS groups
JOIN dbo.TroubleshootingNodes AS issue ON issue.NodeID = groups.IssueNodeID
WHERE (@Search = N'' OR issue.Question LIKE N'%' + @Search + N'%'
       OR issue.Diagnosis LIKE N'%' + @Search + N'%'
       OR issue.[Action] LIKE N'%' + @Search + N'%')
  AND (@CategoryNodeID IS NULL OR groups.CategoryNodeID = @CategoryNodeID)
ORDER BY groups.CategorySort, issue.Question
OPTION (MAXRECURSION 100);";

        using var connection = Db.CreateConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Search", SqlDbType.NVarChar, 1000).Value = (searchText ?? string.Empty).Trim();
        command.Parameters.Add("@CategoryNodeID", SqlDbType.Int).Value = categoryNodeId.HasValue ? categoryNodeId.Value : DBNull.Value;
        connection.Open();
        using var reader = command.ExecuteReader();
        var result = new List<TroubleshootingIssue>();
        while (reader.Read())
        {
            result.Add(new TroubleshootingIssue(
                Convert.ToInt32(reader["NodeID"]),
                Convert.ToInt32(reader["CategoryNodeID"]),
                Convert.ToString(reader["CategoryName"]) ?? string.Empty,
                Convert.ToString(reader["Question"]) ?? string.Empty,
                Convert.ToString(reader["Diagnosis"]) ?? string.Empty,
                Convert.ToString(reader["Action"]) ?? string.Empty,
                Convert.ToBoolean(reader["IsActive"])));
        }
        return result;
    }

    public void Save(TroubleshootingIssue issue)
    {
        if (issue.CategoryNodeID <= 0) throw new ArgumentException("Hãy chọn nhóm sự cố.", nameof(issue));
        if (string.IsNullOrWhiteSpace(issue.Question)) throw new ArgumentException("Tên lỗi/triệu chứng không được để trống.", nameof(issue));
        if (issue.Question.Trim().Length > 300) throw new ArgumentException("Tên lỗi/triệu chứng không được dài quá 300 ký tự.", nameof(issue));
        if (string.IsNullOrWhiteSpace(issue.Diagnosis)) throw new ArgumentException("Kết luận không được để trống.", nameof(issue));
        if (string.IsNullOrWhiteSpace(issue.Action)) throw new ArgumentException("Hướng khắc phục không được để trống.", nameof(issue));

        using var connection = Db.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var oldCategoryId = issue.NodeID == 0
                ? 0
                : FindCategoryForResult(connection, transaction, issue.NodeID);

            int nodeId;
            if (issue.NodeID == 0)
            {
                const string insertNode = @"
INSERT INTO dbo.TroubleshootingNodes (NodeType, Question, Diagnosis, [Action], IsRoot, IsActive)
VALUES ('R', @Question, @Diagnosis, @Action, 0, @IsActive);
SELECT CAST(SCOPE_IDENTITY() AS INT);";
                using var command = new SqlCommand(insertNode, connection, transaction);
                AddIssueParameters(command, issue);
                nodeId = Convert.ToInt32(command.ExecuteScalar());
            }
            else
            {
                const string updateNode = @"
UPDATE dbo.TroubleshootingNodes
SET Question = @Question,
    Diagnosis = @Diagnosis,
    [Action] = @Action,
    IsActive = @IsActive
WHERE NodeID = @NodeID AND NodeType = 'R' AND IsRoot = 0;";
                using var command = new SqlCommand(updateNode, connection, transaction);
                AddIssueParameters(command, issue);
                command.Parameters.Add("@NodeID", SqlDbType.Int).Value = issue.NodeID;
                if (command.ExecuteNonQuery() != 1)
                    throw new InvalidOperationException("Không tìm thấy lỗi cần sửa, hoặc node được chọn không phải node kết quả.");
                nodeId = issue.NodeID;
            }

            // Editing an issue in the same group updates its content only. This preserves
            // the authored answer text at the final question. New/moved issues are attached
            // to an active final question in the selected group, never directly to the group.
            var shouldReattach = issue.NodeID == 0 || oldCategoryId != issue.CategoryNodeID;
            if (shouldReattach)
            {
                using (var deleteEdge = new SqlCommand(
                           "DELETE FROM dbo.TroubleshootingOptions WHERE NextNodeID = @NodeID", connection, transaction))
                {
                    deleteEdge.Parameters.Add("@NodeID", SqlDbType.Int).Value = nodeId;
                    deleteEdge.ExecuteNonQuery();
                }

                var parentQuestionNodeId = FindFinalQuestionNode(connection, transaction, issue.CategoryNodeID);
                if (parentQuestionNodeId <= 0)
                    throw new InvalidOperationException("Không tìm thấy node câu hỏi cuối trong nhóm được chọn. Hãy kiểm tra cây quyết định trước khi thêm/chuyển lỗi.");

                const string insertEdge = @"
DECLARE @NextSort INT;
SELECT @NextSort = ISNULL(MAX(CONVERT(INT, SortOrder)), 0) + 1
FROM dbo.TroubleshootingOptions WHERE NodeID = @ParentNodeID;
IF @NextSort > 32767 THROW 50001, N'Node này đã có quá nhiều lựa chọn.', 1;
INSERT INTO dbo.TroubleshootingOptions (NodeID, OptionText, NextNodeID, SortOrder)
VALUES (@ParentNodeID, @OptionText, @NodeID, CONVERT(SMALLINT, @NextSort));";
                using var edge = new SqlCommand(insertEdge, connection, transaction);
                edge.Parameters.Add("@ParentNodeID", SqlDbType.Int).Value = parentQuestionNodeId;
                edge.Parameters.Add("@OptionText", SqlDbType.NVarChar, 300).Value = issue.Question.Trim();
                edge.Parameters.Add("@NodeID", SqlDbType.Int).Value = nodeId;
                edge.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void Delete(int nodeId)
    {
        using var connection = Db.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            using (var deleteEdges = new SqlCommand(@"
DELETE FROM dbo.TroubleshootingOptions
WHERE NextNodeID = @NodeID OR NodeID = @NodeID;", connection, transaction))
            {
                deleteEdges.Parameters.Add("@NodeID", SqlDbType.Int).Value = nodeId;
                deleteEdges.ExecuteNonQuery();
            }

            using (var deleteNode = new SqlCommand(
                "DELETE FROM dbo.TroubleshootingNodes WHERE NodeID = @NodeID AND NodeType = 'R' AND IsRoot = 0;",
                connection, transaction))
            {
                deleteNode.Parameters.Add("@NodeID", SqlDbType.Int).Value = nodeId;
                if (deleteNode.ExecuteNonQuery() != 1)
                    throw new InvalidOperationException("Không tìm thấy lỗi cần xóa, hoặc node được chọn không phải node kết quả.");
            }
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static int FindCategoryForResult(SqlConnection connection, SqlTransaction transaction, int resultNodeId)
    {
        const string sql = @"
;WITH Categories AS
(
    SELECT category.NodeID AS CategoryNodeID
    FROM dbo.TroubleshootingNodes AS rootNode
    JOIN dbo.TroubleshootingOptions AS rootOption ON rootOption.NodeID = rootNode.NodeID
    JOIN dbo.TroubleshootingNodes AS category ON category.NodeID = rootOption.NextNodeID
    WHERE rootNode.IsRoot = 1 AND rootNode.IsActive = 1 AND category.NodeType = 'Q' AND category.IsActive = 1
), Reachable AS
(
    SELECT c.CategoryNodeID, c.CategoryNodeID AS NodeID,
           CAST('/' + CONVERT(VARCHAR(20), c.CategoryNodeID) + '/' AS VARCHAR(MAX)) AS NodePath,
           0 AS Depth
    FROM Categories AS c
    UNION ALL
    SELECT r.CategoryNodeID, destination.NodeID,
           CAST(r.NodePath + CONVERT(VARCHAR(20), destination.NodeID) + '/' AS VARCHAR(MAX)),
           r.Depth + 1
    FROM Reachable AS r
    JOIN dbo.TroubleshootingOptions AS edge ON edge.NodeID = r.NodeID
    JOIN dbo.TroubleshootingNodes AS destination ON destination.NodeID = edge.NextNodeID
      AND (destination.NodeType = 'R' OR destination.IsActive = 1)
    WHERE r.Depth < 25
      AND CHARINDEX('/' + CONVERT(VARCHAR(20), destination.NodeID) + '/', r.NodePath) = 0
)
SELECT TOP (1) CategoryNodeID
FROM Reachable
WHERE NodeID = @ResultNodeID
ORDER BY CategoryNodeID
OPTION (MAXRECURSION 100);";
        using var command = new SqlCommand(sql, connection, transaction);
        command.Parameters.Add("@ResultNodeID", SqlDbType.Int).Value = resultNodeId;
        var value = command.ExecuteScalar();
        return value is null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
    }

    private static int FindFinalQuestionNode(SqlConnection connection, SqlTransaction transaction, int categoryNodeId)
    {
        const string sql = @"
;WITH Reachable AS
(
    SELECT category.NodeID AS CategoryNodeID, category.NodeID AS NodeID,
           CAST('/' + CONVERT(VARCHAR(20), category.NodeID) + '/' AS VARCHAR(MAX)) AS NodePath,
           0 AS Depth
    FROM dbo.TroubleshootingNodes AS category
    WHERE category.NodeID = @CategoryNodeID AND category.NodeType = 'Q' AND category.IsActive = 1
    UNION ALL
    SELECT r.CategoryNodeID, destination.NodeID,
           CAST(r.NodePath + CONVERT(VARCHAR(20), destination.NodeID) + '/' AS VARCHAR(MAX)),
           r.Depth + 1
    FROM Reachable AS r
    JOIN dbo.TroubleshootingOptions AS edge ON edge.NodeID = r.NodeID
    JOIN dbo.TroubleshootingNodes AS destination ON destination.NodeID = edge.NextNodeID
      AND destination.NodeType = 'Q' AND destination.IsActive = 1
    WHERE r.Depth < 25
      AND CHARINDEX('/' + CONVERT(VARCHAR(20), destination.NodeID) + '/', r.NodePath) = 0
)
SELECT TOP (1) q.NodeID
FROM Reachable AS reach
JOIN dbo.TroubleshootingNodes AS q ON q.NodeID = reach.NodeID AND q.NodeType = 'Q' AND q.IsActive = 1
WHERE EXISTS (SELECT 1 FROM dbo.TroubleshootingOptions AS e WHERE e.NodeID = q.NodeID)
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.TroubleshootingOptions AS e
      JOIN dbo.TroubleshootingNodes AS child ON child.NodeID = e.NextNodeID
      WHERE e.NodeID = q.NodeID AND child.NodeType = 'Q' AND child.IsActive = 1
  )
ORDER BY (SELECT COUNT(*) FROM dbo.TroubleshootingOptions AS edgeCount WHERE edgeCount.NodeID = q.NodeID) ASC,
         reach.Depth DESC, q.NodeID
OPTION (MAXRECURSION 100);";
        using var command = new SqlCommand(sql, connection, transaction);
        command.Parameters.Add("@CategoryNodeID", SqlDbType.Int).Value = categoryNodeId;
        var value = command.ExecuteScalar();
        return value is null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
    }

    private static void AddIssueParameters(SqlCommand command, TroubleshootingIssue issue)
    {
        command.Parameters.Add("@Question", SqlDbType.NVarChar, 1000).Value = issue.Question.Trim();
        command.Parameters.Add("@Diagnosis", SqlDbType.NVarChar, 2000).Value = issue.Diagnosis.Trim();
        command.Parameters.Add("@Action", SqlDbType.NVarChar, 4000).Value = issue.Action.Trim();
        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = issue.IsActive;
    }
}
