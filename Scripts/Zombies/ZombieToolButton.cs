using Godot;
using System;

public partial class ZombieToolButton : TextureButton
{
	Label label;
	AudioStreamPlayer Click;
	public override void _Ready()
	{
		label = GetNode<Label>("./Label");
		Click = GetNode<AudioStreamPlayer>("%Click");
		MouseEntered += OnMouseEnter;
		MouseExited += OnMouseExit;
	}
	public void OnMouseEnter()
	{
		label.Modulate = Color.Color8(0x38, 0xf2, 0x46);
		Click.Play();
	}
	public void OnMouseExit()
	{
		label.Modulate = Color.Color8(0xff, 0xff, 0xff);
	}
}
