using Godot;
using System;

public partial class PlantSceneChanger : TextureButton
{
	AudioStreamPlayer Click;
	public override void _Ready()
	{
		Click = GetNode<AudioStreamPlayer>("../Click");
		Pressed += () =>
		{
			SceneChanger.animationPlayer.Play("ChangeToZombie", customSpeed: 2f);
		};
	}
}
