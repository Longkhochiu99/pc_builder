namespace PCBuilder.Forms;
partial class ComponentEditForm
{
 private System.ComponentModel.IContainer? components; private ComboBox _category=null!,_brand=null!; private TextBox _model=null!,_sku=null!; private NumericUpDown _price=null!,_stock=null!; private CheckBox _active=null!;

    private TableLayoutPanel t=null!; private Button save=null!; private Label lblCategory=null!,lblBrand=null!,lblModel=null!,lblSku=null!,lblPrice=null!,lblStock=null!,lblActive=null!;
    private void InitializeComponent()
    {
        _category = new ComboBox();
        _brand = new ComboBox();
        _model = new TextBox();
        _sku = new TextBox();
        _price = new NumericUpDown();
        _stock = new NumericUpDown();
        _active = new CheckBox();
        t = new TableLayoutPanel();
        lblCategory = new Label();
        lblBrand = new Label();
        lblModel = new Label();
        lblSku = new Label();
        lblPrice = new Label();
        lblStock = new Label();
        lblActive = new Label();
        save = new Button();
        ((System.ComponentModel.ISupportInitialize)_price).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_stock).BeginInit();
        t.SuspendLayout();
        SuspendLayout();
        // 
        // _category
        // 
        _category.Dock = DockStyle.Fill;
        _category.Location = new Point(134, 19);
        _category.Name = "_category";
        _category.Size = new Size(299, 28);
        _category.TabIndex = 1;
        // 
        // _brand
        // 
        _brand.Dock = DockStyle.Fill;
        _brand.Location = new Point(134, 57);
        _brand.Name = "_brand";
        _brand.Size = new Size(299, 28);
        _brand.TabIndex = 3;
        // 
        // _model
        // 
        _model.Dock = DockStyle.Fill;
        _model.Location = new Point(134, 95);
        _model.Name = "_model";
        _model.Size = new Size(299, 27);
        _model.TabIndex = 5;
        // 
        // _sku
        // 
        _sku.Dock = DockStyle.Fill;
        _sku.Location = new Point(134, 133);
        _sku.Name = "_sku";
        _sku.Size = new Size(299, 27);
        _sku.TabIndex = 7;
        // 
        // _price
        // 
        _price.DecimalPlaces = 2;
        _price.Dock = DockStyle.Fill;
        _price.Location = new Point(134, 171);
        _price.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
        _price.Name = "_price";
        _price.Size = new Size(299, 27);
        _price.TabIndex = 9;
        _price.ThousandsSeparator = true;
        // 
        // _stock
        // 
        _stock.Dock = DockStyle.Fill;
        _stock.Location = new Point(134, 209);
        _stock.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        _stock.Name = "_stock";
        _stock.Size = new Size(299, 27);
        _stock.TabIndex = 11;
        // 
        // _active
        // 
        _active.Checked = true;
        _active.CheckState = CheckState.Checked;
        _active.Dock = DockStyle.Fill;
        _active.Location = new Point(134, 247);
        _active.Name = "_active";
        _active.Size = new Size(299, 32);
        _active.TabIndex = 13;
        _active.Text = "Đang kinh doanh";
        // 
        // t
        // 
        t.AutoScroll = true;
        t.ColumnCount = 2;
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115F));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        t.Controls.Add(lblCategory, 0, 0);
        t.Controls.Add(_category, 1, 0);
        t.Controls.Add(lblBrand, 0, 1);
        t.Controls.Add(_brand, 1, 1);
        t.Controls.Add(lblModel, 0, 2);
        t.Controls.Add(_model, 1, 2);
        t.Controls.Add(lblSku, 0, 3);
        t.Controls.Add(_sku, 1, 3);
        t.Controls.Add(lblPrice, 0, 4);
        t.Controls.Add(_price, 1, 4);
        t.Controls.Add(lblStock, 0, 5);
        t.Controls.Add(_stock, 1, 5);
        t.Controls.Add(lblActive, 0, 6);
        t.Controls.Add(_active, 1, 6);
        t.Controls.Add(save, 1, 7);
        t.Dock = DockStyle.Fill;
        t.Location = new Point(0, 0);
        t.Name = "t";
        t.Padding = new Padding(16);
        t.RowCount = 8;
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        t.Size = new Size(452, 343);
        t.TabIndex = 0;
        // 
        // lblCategory
        // 
        lblCategory.Anchor = AnchorStyles.Left;
        lblCategory.AutoSize = true;
        lblCategory.Location = new Point(19, 25);
        lblCategory.Name = "lblCategory";
        lblCategory.Size = new Size(76, 20);
        lblCategory.TabIndex = 0;
        lblCategory.Text = "Danh mục";
        // 
        // lblBrand
        // 
        lblBrand.Anchor = AnchorStyles.Left;
        lblBrand.AutoSize = true;
        lblBrand.Location = new Point(19, 63);
        lblBrand.Name = "lblBrand";
        lblBrand.Size = new Size(45, 20);
        lblBrand.TabIndex = 2;
        lblBrand.Text = "Hãng";
        // 
        // lblModel
        // 
        lblModel.Anchor = AnchorStyles.Left;
        lblModel.AutoSize = true;
        lblModel.Location = new Point(19, 101);
        lblModel.Name = "lblModel";
        lblModel.Size = new Size(79, 20);
        lblModel.TabIndex = 4;
        lblModel.Text = "Tên model";
        // 
        // lblSku
        // 
        lblSku.Anchor = AnchorStyles.Left;
        lblSku.AutoSize = true;
        lblSku.Location = new Point(19, 139);
        lblSku.Name = "lblSku";
        lblSku.Size = new Size(36, 20);
        lblSku.TabIndex = 6;
        lblSku.Text = "SKU";
        // 
        // lblPrice
        // 
        lblPrice.Anchor = AnchorStyles.Left;
        lblPrice.AutoSize = true;
        lblPrice.Location = new Point(19, 177);
        lblPrice.Name = "lblPrice";
        lblPrice.Size = new Size(76, 20);
        lblPrice.TabIndex = 8;
        lblPrice.Text = "Giá (VND)";
        // 
        // lblStock
        // 
        lblStock.Anchor = AnchorStyles.Left;
        lblStock.AutoSize = true;
        lblStock.Location = new Point(19, 215);
        lblStock.Name = "lblStock";
        lblStock.Size = new Size(62, 20);
        lblStock.TabIndex = 10;
        lblStock.Text = "Tồn kho";
        // 
        // lblActive
        // 
        lblActive.Anchor = AnchorStyles.Left;
        lblActive.AutoSize = true;
        lblActive.Location = new Point(19, 253);
        lblActive.Name = "lblActive";
        lblActive.Size = new Size(75, 20);
        lblActive.TabIndex = 12;
        lblActive.Text = "Trạng thái";
        // 
        // save
        // 
        save.Location = new Point(134, 285);
        save.Name = "save";
        save.Size = new Size(100, 32);
        save.TabIndex = 14;
        save.Text = "Lưu";
        save.Click += Save;
        // 
        // ComponentEditForm
        // 
        ClientSize = new Size(452, 343);
        Controls.Add(t);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "ComponentEditForm";
        StartPosition = FormStartPosition.CenterParent;
        ((System.ComponentModel.ISupportInitialize)_price).EndInit();
        ((System.ComponentModel.ISupportInitialize)_stock).EndInit();
        t.ResumeLayout(false);
        t.PerformLayout();
        ResumeLayout(false);
    }
    protected override void Dispose(bool disposing){if(disposing)components?.Dispose();base.Dispose(disposing);}
}
