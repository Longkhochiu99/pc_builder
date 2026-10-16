using PCBuilder.Data; using PCBuilder.Models;
namespace PCBuilder.Forms;
internal sealed partial class ComponentEditForm : Form
{
 private readonly ComponentRepository _repo; private readonly ComponentItem? _item;
 public ComponentEditForm():this(new ComponentRepository(),null,true){}
 public ComponentEditForm(ComponentRepository repo,ComponentItem? item):this(repo,item,false){}
 private ComponentEditForm(ComponentRepository repo,ComponentItem? item,bool designMode){_repo=repo;_item=item;InitializeComponent();Text=item is null?"Thêm linh kiện":"Sửa linh kiện";if(designMode)return;_category.DataSource=_repo.GetCategories();_category.DisplayMember="CategoryName";_category.ValueMember="CategoryID";_brand.DataSource=_repo.GetBrands();_brand.DisplayMember="BrandName";_brand.ValueMember="BrandID";if(item is not null){_category.SelectedValue=item.CategoryID;_brand.SelectedValue=item.BrandID;_model.Text=item.ModelName;_sku.Text=item.SKU??"";_price.Value=Math.Min(_price.Maximum,item.Price);_stock.Value=item.StockQuantity;_active.Checked=item.IsActive;}}
 private void Save(object? sender,EventArgs e){if(string.IsNullOrWhiteSpace(_model.Text)||_category.SelectedValue is null||_brand.SelectedValue is null){MessageBox.Show("Nhập tên model, chọn danh mục và hãng.");return;}var c=new ComponentItem{ComponentID=_item?.ComponentID??0,CategoryID=Convert.ToInt32(_category.SelectedValue),BrandID=Convert.ToInt32(_brand.SelectedValue),ModelName=_model.Text.Trim(),SKU=string.IsNullOrWhiteSpace(_sku.Text)?null:_sku.Text.Trim(),Price=_price.Value,StockQuantity=(int)_stock.Value,IsActive=_active.Checked};try{_repo.Save(c);DialogResult=DialogResult.OK;Close();}catch(Exception ex){MessageBox.Show("Lưu thất bại: "+ex.Message);}}
}
