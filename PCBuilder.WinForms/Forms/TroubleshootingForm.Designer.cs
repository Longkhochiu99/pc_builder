namespace PCBuilder.Forms;
partial class TroubleshootingForm
{
 private System.ComponentModel.IContainer? components; private Label _question=null!; private FlowLayoutPanel _answers=null!; private RichTextBox _diagnosis=null!; private Button _back=null!,_restart=null!;

    private FlowLayoutPanel bottom=null!;
 private void InitializeComponent(){components=new System.ComponentModel.Container();_question=new Label();_answers=new FlowLayoutPanel();_diagnosis=new RichTextBox();_back=new Button();_restart=new Button();bottom=new FlowLayoutPanel();SuspendLayout();Text="Trợ lý chẩn đoán sự cố";Dock=DockStyle.Fill;BackColor=UiTheme.Background;_question.Dock=DockStyle.Top;_question.Height=100;_question.Font=new Font("Segoe UI",15,FontStyle.Bold);_question.TextAlign=ContentAlignment.MiddleLeft;_answers.Dock=DockStyle.Fill;_answers.FlowDirection=FlowDirection.TopDown;_answers.WrapContents=false;_answers.AutoScroll=true;_diagnosis.Dock=DockStyle.Fill;_diagnosis.ReadOnly=true;_diagnosis.Visible=false;_back.Text="Quay lại";_back.Width=100;_back.Enabled=false;_restart.Text="Bắt đầu lại";_restart.Width=110;bottom.Dock=DockStyle.Bottom;bottom.Height=48;bottom.Controls.AddRange(new Control[]{_back,_restart});Controls.Add(_answers);Controls.Add(_diagnosis);Controls.Add(_question);Controls.Add(bottom);ResumeLayout(false);}
 protected override void Dispose(bool disposing){if(disposing)components?.Dispose();base.Dispose(disposing);}
}
