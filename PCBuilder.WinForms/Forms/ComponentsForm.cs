using PCBuilder.Data; using PCBuilder.Models; using System.Data;
namespace PCBuilder.Forms;
public sealed partial class ComponentsForm : Form
{
    private readonly AppUser _user; private readonly bool _adminMode; private readonly ComponentRepository _repo=new();
    public ComponentsForm():this(new AppUser(0,"designer","Designer","User"),false,true){}
    public ComponentsForm(AppUser user,bool adminMode=false):this(user,adminMode,false){}
    private ComponentsForm(AppUser user,bool adminMode,bool designMode) { _user=user; _adminMode=adminMode&&user.IsAdmin; InitializeComponent(); _btnAdd.Visible=_btnEdit.Visible=_btnDelete.Visible=(designMode || _adminMode); if(designMode)return; try { var categories=_repo.GetCategories(); var all=categories.NewRow(); all["CategoryID"]=DBNull.Value; all["CategoryName"]="Tất cả danh mục"; categories.Rows.InsertAt(all,0); _category.DataSource=categories; _category.DisplayMember="CategoryName"; _category.ValueMember="CategoryID"; } catch {} UiTheme.Apply(this); LoadData(); }
    private int? CategoryId() { if(_category.SelectedValue is DataRowView)return null; return int.TryParse(Convert.ToString(_category.SelectedValue),out var id)&&id>0?id:null; }
    private void LoadData() { try { _grid.DataSource=_repo.Search(_search.Text,CategoryId(),_adminMode); foreach(var name in new[]{"CategoryID","BrandID","IsActive","CategoryCode","Socket","MemoryType","TDP_W","PowerDraw_W","Wattage_W","LengthMM","MaxGPULengthMM","MaxCoolerHeightMM","FormFactor"}) if(_grid.Columns.Contains(name))_grid.Columns[name].Visible=false; } catch(Exception ex){MessageBox.Show("Không tải được linh kiện: "+ex.Message);} }
    private ComponentItem? Selected()=>_grid.CurrentRow?.DataBoundItem as ComponentItem;
    private void SearchClick(object? s,EventArgs e)=>LoadData(); private void AddClick(object? s,EventArgs e)=>Edit(null); private void EditClick(object? s,EventArgs e)=>Edit(Selected());
    private void Edit(ComponentItem? item){try{using var form=new ComponentEditForm(_repo,item);if(form.ShowDialog(this)==DialogResult.OK)LoadData();}catch(Exception ex){MessageBox.Show(ex.Message);}}
    private void DeleteClick(object? s,EventArgs e){var item=Selected();if(item is null){MessageBox.Show("Chọn linh kiện trước.");return;}if(MessageBox.Show($"Ngừng kinh doanh {item.DisplayName}?","Xác nhận",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)!=DialogResult.Yes)return;try{_repo.Delete(item.ComponentID);LoadData();}catch(Exception ex){MessageBox.Show("Không thể cập nhật: "+ex.Message);}}
}
