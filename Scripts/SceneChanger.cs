using Godot;
using System;
using System.ComponentModel;

public partial class SceneChanger : Control
{
	static public ZombieMain zombieMain;
	static public PlantMain plantMain;
	static public AnimationPlayer animationPlayer;
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("%LogoAnim");
		zombieMain = (ZombieMain)ResourceLoader.Load<PackedScene>("res://zombie.tscn").Instantiate();
		plantMain = (PlantMain)ResourceLoader.Load<PackedScene>("res://plant.tscn").Instantiate();
		animationPlayer.Play("Logo");
	}
	public void _AddASceneManually()
	{
		GetTree().Root.AddChild(plantMain);
	}
	public void Initialize()
	{
		CallDeferred(nameof(_AddASceneManually));
	}
	public void ChangeToZombie(Node parent = null)
	{
		if (parent is null)
		{
			parent = GetTree().Root;
		}
		parent.AddChild(zombieMain);
		parent.RemoveChild(plantMain);
	}
	static public void ChangeToZombie(Node parent, string path)
	{
		parent.AddChild(zombieMain);
		parent.RemoveChild(plantMain);
		zombieMain.OnFileSelected(path);
	}
	public void ChangeToPlant(Node parent = null)
	{
		if (parent is null)
		{
			parent = GetTree().Root;
		}
		parent.AddChild(plantMain);
		parent.RemoveChild(zombieMain);
	}
	static public void ChangeToPlant(Node parent, string path)
	{
		parent.AddChild(plantMain);
		parent.RemoveChild(zombieMain);
		plantMain.OnFileSelected(path);
	}
}
