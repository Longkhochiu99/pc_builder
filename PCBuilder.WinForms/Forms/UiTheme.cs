namespace PCBuilder.Forms;

/// <summary>Shared visual system for the WinForms application. No third-party UI package required.</summary>
internal static class UiTheme
{
    public static readonly Color Background = Color.FromArgb(245, 247, 251);
    public static readonly Color Surface = Color.White;
    public static readonly Color Sidebar = Color.FromArgb(19, 29, 48);
    public static readonly Color Primary = Color.FromArgb(72, 105, 255);
    public static readonly Color PrimaryHover = Color.FromArgb(53, 83, 224);
    public static readonly Color Text = Color.FromArgb(31, 41, 55);
    public static readonly Color Muted = Color.FromArgb(107, 114, 128);
    public static readonly Color Border = Color.FromArgb(226, 232, 240);
    public static readonly Font BodyFont = new("Segoe UI", 9.5F);
    public static readonly Font HeadingFont = new("Segoe UI Semibold", 19F, FontStyle.Bold);

    public static void Apply(Control root)
    {
        root.Font = BodyFont;
        root.BackColor = Background;
        root.ForeColor = Text;
        foreach (Control control in root.Controls) ApplyControl(control);
    }

    private static void ApplyControl(Control c)
    {
        c.Font = BodyFont;
        if (c is DataGridView grid)
        {
            grid.BackgroundColor = Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = Border;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(239, 243, 250);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Text;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            grid.ColumnHeadersHeight = 40;
            grid.DefaultCellStyle.BackColor = Surface;
            grid.DefaultCellStyle.ForeColor = Text;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(226, 233, 255);
            grid.DefaultCellStyle.SelectionForeColor = Text;
            grid.RowTemplate.Height = 34;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 254);
        }
        else if (c is Button b)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = Primary;
            b.ForeColor = Color.White;
            b.Cursor = Cursors.Hand;
            b.Height = Math.Max(b.Height, 36);
            b.Padding = new Padding(10, 2, 10, 2);
        }
        else if (c is TextBox t)
        {
            t.BorderStyle = BorderStyle.FixedSingle;
            t.BackColor = Surface;
            t.ForeColor = Text;
            t.Font = new Font("Segoe UI", 10F);
        }
        else if (c is ComboBox combo)
        {
            combo.FlatStyle = FlatStyle.Flat;
            combo.BackColor = Surface;
            combo.ForeColor = Text;
        }
        else if (c is RichTextBox rich)
        {
            rich.BorderStyle = BorderStyle.None;
            rich.BackColor = Surface;
            rich.ForeColor = Text;
        }
        else if (c is Panel or TableLayoutPanel or FlowLayoutPanel or SplitContainer)
        {
            if (c.BackColor == Control.DefaultBackColor) c.BackColor = Background;
        }
        foreach (Control child in c.Controls) ApplyControl(child);
    }

    public static Button MakeButton(string text, bool secondary = false)
    {
        var b = new Button { Text = text, AutoSize = true, MinimumSize = new Size(100, 40), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Padding = new Padding(12, 5, 12, 5), Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold) };
        b.FlatAppearance.BorderSize = secondary ? 1 : 0;
        b.FlatAppearance.BorderColor = Border;
        b.BackColor = secondary ? Surface : Primary;
        b.ForeColor = secondary ? Text : Color.White;
        return b;
    }

    public static Panel MakeCard()
        => new() { BackColor = Surface, Padding = new Padding(20), Margin = new Padding(0, 0, 14, 14) };
}
