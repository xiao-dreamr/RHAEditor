using Godot;
using System;
using System.Diagnostics;

public partial class Squalour : TextureButton
{
	int ClickedTimes = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += () =>
		{
			ClickedTimes++;
			Scale *= 1.01f;
			if (ClickedTimes >= 22)
			{
				Scale = new Vector2(0.286f, 0.286f);
				Disabled = true;
				Debug.WriteLine("窝红温了！"); //红温
				GetNode<RichTextLabel>("./Mad").Text = "[color=red][b]窝红温了！[/b][/color]";
				Timer timer = new()
				{
					WaitTime = 3, //三秒后解除红温
					OneShot = true
				};
				timer.Timeout += () =>
				{
					ClickedTimes = 0;
					Disabled = false;
					GetNode<RichTextLabel>("./Mad").Text = "[color=#e097ae][b]永不红温！[/b][/color]";
				};
				AddChild(timer);
				timer.Start();
			}
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
