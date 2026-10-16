using System.ComponentModel;
using PCBuilder.Services;
namespace PCBuilder.Forms;
public sealed partial class TroubleshootingForm : Form
{
 private readonly TroubleshootingService _service=new(); private readonly Stack<int> _history=new(); private int _current;
 public TroubleshootingForm(){InitializeComponent();_back.Click+=(_,_)=>Back();_restart.Click+=(_,_)=>Restart();UiTheme.Apply(this);if(LicenseManager.UsageMode!=LicenseUsageMode.Designtime)Restart();}
 private void Restart(){_history.Clear();_current=_service.Root;_diagnosis.Visible=false;_answers.Visible=true;Render();}
 private void Render(){var step=_service.Get(_current);_question.Text=step.Question;_answers.Controls.Clear();_diagnosis.Visible=_service.IsTerminal(_current);_answers.Visible=!_diagnosis.Visible;_back.Enabled=_history.Count>0;if(_service.IsTerminal(_current))_diagnosis.Text=$"KẾT LUẬN\n{step.Diagnosis}\n\nHƯỚNG KHẮC PHỤC\n{step.Action}\n\nĐây là hướng dẫn sàng lọc ban đầu, không thay thế kiểm tra trực tiếp của kỹ thuật viên.";else for(int i=0;i<step.Options.Length;i++){int index=i;var b=new Button{Text=step.Options[i],Width=600,Height=46,TextAlign=ContentAlignment.MiddleLeft,Margin=new Padding(4,8,4,4)};b.Click+=(_,_)=>{_history.Push(_current);_current=_service.Follow(_current,index);Render();};_answers.Controls.Add(b);}UiTheme.Apply(_answers);}
 private void Back(){if(_history.Count>0){_current=_history.Pop();Render();}}
}
