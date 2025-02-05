using Godot;

public partial class PlantGenericEdit : CodeEdit
{
	RichTextLabel rtl;
	PlantToolsManager tools;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tools = GetNode<PlantToolsManager>("/root/PlantMain/Inside/VBox/WorkPlace/Editor/tools");
		rtl = GetNode<RichTextLabel>("./RichTextLabel");
		OnEditFinished();
		FocusEntered += OnRichFocusEntered;
		FocusExited += OnEditFinished;
		tools.Deleted += OnDeletePressed;
		GuttersDrawLineNumbers = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnEditFinished()
	{
		if (Text == PlaceholderText.Replace("*...", "："))
		{
			// 如果是可选项且没输入，也就是只有默认填充的提示，就清空。
			Text = "";
		}
		// 显示富文本标签
		rtl.Visible = true;
		// 若此处有文本，则替换掉<和>符号并显示，否则显示默认
		rtl.Text = Text.Length > 0 ? Text.Replace("<", "[").Replace(">", "]") : PlaceholderText.Replace("<", "[").Replace(">", "]");
	}
	public void OnRichFocusEntered()
	{
		// 隐藏富文本标签
		rtl.Visible = false;
		// CodeEdit控件获得焦点
		if (Text.Length == 0 && PlaceholderText.Contains("*..."))
		{
			// 如果是可选项，就加上诸如“伤害：”的提示
			Text = PlaceholderText.Replace("*...", "：");
			// 光标移到末尾
			SetCaretColumn(Text.Length - 8); // 8是“</color>”的长度
		}
		GrabFocus();
	}
	public void OnDeletePressed()
	{
		Clear();
		rtl.Text = PlaceholderText.Replace("<", "[").Replace(">", "]");
	}

}
