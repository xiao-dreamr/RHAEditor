using Godot;
using System;
using System.Diagnostics;

public partial class LinkedButton : Button
{
	/// <summary>
	/// 目标tree item
	/// </summary>
	TreeItem target;
	PlantJsonTree tree;
	AudioStreamPlayer Click;
	public LinkedButton(TreeItem target, AudioStreamPlayer Click) : base()
	{
		this.Click = Click;
		this.target = target;
		Text = target.GetText(0).Split('：')[1];
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tree = GetNode<PlantJsonTree>("/root/PlantMain/Inside/VBox/WorkPlace/JsonTree/Tree");
		Pressed += () =>
		{
			if (!target.Visible)
			{
				target.Visible = true;
			}
			tree.SetSelected(target, 0);
			tree.ScrollToItem(target);
			Click.Play();
		};
	}
}
