namespace PCBuilder.Forms;
partial class BuildForm
{
 private System.ComponentModel.IContainer? components; private TextBox _budget=null!,_search=null!; private ComboBox _purpose=null!; private DataGridView _catalog=null!,_selected=null!; private Label _total=null!; private RichTextBox _results=null!;

    private FlowLayoutPanel top=null!; private SplitContainer split=null!; private Panel bottom=null!; private Panel totalPanel=null!; private Button btnSearch=null!; private Button btnAdd=null!; private Button btnRemove=null!; private Button btnCheck=null!; private Button btnRecommend=null!; private Button btnSave=null!;
    private void InitializeComponent()
    {
        _budget = new TextBox();
        _search = new TextBox();
        _purpose = new ComboBox();
        _catalog = new DataGridView();
        _selected = new DataGridView();
        _total = new Label();
        _results = new RichTextBox();
        top = new FlowLayoutPanel();
        btnSearch = new Button();
        btnAdd = new Button();
        btnRemove = new Button();
        btnCheck = new Button();
        btnRecommend = new Button();
        btnSave = new Button();
        split = new SplitContainer();
        bottom = new Panel();
        totalPanel = new Panel();
        ((System.ComponentModel.ISupportInitialize)_catalog).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_selected).BeginInit();
        top.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)split).BeginInit();
        split.Panel1.SuspendLayout();
        split.Panel2.SuspendLayout();
        split.SuspendLayout();
        bottom.SuspendLayout();
        totalPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _budget
        // 
        _budget.Location = new Point(517, 7);
        _budget.Name = "_budget";
        _budget.PlaceholderText = "Ngân sách VND";
        _budget.Size = new Size(120, 27);
        _budget.TabIndex = 5;
        // 
        // _search
        // 
        _search.Location = new Point(7, 7);
        _search.Name = "_search";
        _search.PlaceholderText = "Tìm linh kiện";
        _search.Size = new Size(180, 27);
        _search.TabIndex = 0;
        // 
        // _purpose
        // 
        _purpose.DropDownStyle = ComboBoxStyle.DropDownList;
        _purpose.Location = new Point(643, 7);
        _purpose.Name = "_purpose";
        _purpose.Size = new Size(145, 28);
        _purpose.TabIndex = 6;
        // 
        // _catalog
        // 
        _catalog.AllowUserToAddRows = false;
        _catalog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _catalog.ColumnHeadersHeight = 29;
        _catalog.Dock = DockStyle.Fill;
        _catalog.Location = new Point(0, 0);
        _catalog.MultiSelect = false;
        _catalog.Name = "_catalog";
        _catalog.ReadOnly = true;
        _catalog.RowHeadersWidth = 51;
        _catalog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _catalog.Size = new Size(939, 281);
        _catalog.TabIndex = 0;
        // 
        // _selected
        // 
        _selected.AllowUserToAddRows = false;
        _selected.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _selected.ColumnHeadersHeight = 29;
        _selected.Dock = DockStyle.Fill;
        _selected.Location = new Point(0, 0);
        _selected.MultiSelect = false;
        _selected.Name = "_selected";
        _selected.ReadOnly = true;
        _selected.RowHeadersWidth = 51;
        _selected.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _selected.Size = new Size(222, 281);
        _selected.TabIndex = 0;
        // 
        // _total
        // 
        _total.AutoSize = true;
        _total.Location = new Point(8, 6);
        _total.Name = "_total";
        _total.Size = new Size(0, 20);
        _total.TabIndex = 0;
        // 
        // _results
        // 
        _results.Dock = DockStyle.Fill;
        _results.Location = new Point(0, 30);
        _results.Name = "_results";
        _results.ReadOnly = true;
        _results.Size = new Size(1165, 160);
        _results.TabIndex = 0;
        _results.Text = "";
        // 
        // top
        // 
        top.Controls.Add(_search);
        top.Controls.Add(btnSearch);
        top.Controls.Add(btnAdd);
        top.Controls.Add(btnRemove);
        top.Controls.Add(btnCheck);
        top.Controls.Add(_budget);
        top.Controls.Add(_purpose);
        top.Controls.Add(btnRecommend);
        top.Controls.Add(btnSave);
        top.Dock = DockStyle.Top;
        top.Location = new Point(0, 0);
        top.Name = "top";
        top.Padding = new Padding(4);
        top.Size = new Size(1165, 82);
        top.TabIndex = 2;
        // 
        // btnSearch
        // 
        btnSearch.Location = new Point(193, 7);
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(75, 23);
        btnSearch.TabIndex = 1;
        btnSearch.Text = "Tìm";
        btnSearch.Click += SearchClick;
        // 
        // btnAdd
        // 
        btnAdd.Location = new Point(274, 7);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(75, 23);
        btnAdd.TabIndex = 2;
        btnAdd.Text = "Thêm đã chọn";
        btnAdd.Click += AddClick;
        // 
        // btnRemove
        // 
        btnRemove.Location = new Point(355, 7);
        btnRemove.Name = "btnRemove";
        btnRemove.Size = new Size(75, 23);
        btnRemove.TabIndex = 3;
        btnRemove.Text = "Bỏ khỏi cấu hình";
        btnRemove.Click += RemoveClick;
        // 
        // btnCheck
        // 
        btnCheck.Location = new Point(436, 7);
        btnCheck.Name = "btnCheck";
        btnCheck.Size = new Size(75, 23);
        btnCheck.TabIndex = 4;
        btnCheck.Text = "Kiểm tra tương thích";
        btnCheck.Click += CheckClick;
        // 
        // btnRecommend
        // 
        btnRecommend.Location = new Point(794, 7);
        btnRecommend.Name = "btnRecommend";
        btnRecommend.Size = new Size(75, 23);
        btnRecommend.TabIndex = 7;
        btnRecommend.Text = "Gợi ý ngân sách";
        btnRecommend.Click += RecommendClick;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(875, 7);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(75, 23);
        btnSave.TabIndex = 8;
        btnSave.Text = "Lưu cấu hình";
        btnSave.Click += SaveClick;
        // 
        // split
        // 
        split.Dock = DockStyle.Fill;
        split.Location = new Point(0, 82);
        split.Name = "split";
        // 
        // split.Panel1
        // 
        split.Panel1.Controls.Add(_catalog);
        // 
        // split.Panel2
        // 
        split.Panel2.Controls.Add(_selected);
        split.Size = new Size(1165, 281);
        split.SplitterDistance = 939;
        split.TabIndex = 0;
        // 
        // bottom
        // 
        bottom.Controls.Add(_results);
        bottom.Controls.Add(totalPanel);
        bottom.Dock = DockStyle.Bottom;
        bottom.Location = new Point(0, 363);
        bottom.Name = "bottom";
        bottom.Size = new Size(1165, 190);
        bottom.TabIndex = 1;
        // 
        // totalPanel
        // 
        totalPanel.Controls.Add(_total);
        totalPanel.Dock = DockStyle.Top;
        totalPanel.Location = new Point(0, 0);
        totalPanel.Name = "totalPanel";
        totalPanel.Size = new Size(1165, 30);
        totalPanel.TabIndex = 1;
        // 
        // BuildForm
        // 
        BackColor = Color.FromArgb(245, 247, 251);
        ClientSize = new Size(1165, 553);
        Controls.Add(split);
        Controls.Add(bottom);
        Controls.Add(top);
        Name = "BuildForm";
        Text = "Build PC & gợi ý cấu hình";
        ((System.ComponentModel.ISupportInitialize)_catalog).EndInit();
        ((System.ComponentModel.ISupportInitialize)_selected).EndInit();
        top.ResumeLayout(false);
        top.PerformLayout();
        split.Panel1.ResumeLayout(false);
        split.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)split).EndInit();
        split.ResumeLayout(false);
        bottom.ResumeLayout(false);
        totalPanel.ResumeLayout(false);
        totalPanel.PerformLayout();
        ResumeLayout(false);
    }
    protected override void Dispose(bool disposing){if(disposing)components?.Dispose();base.Dispose(disposing);}
}
