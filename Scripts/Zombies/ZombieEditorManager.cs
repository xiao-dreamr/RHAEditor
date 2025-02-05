using Godot;
using System;

public partial class ZombieEditorManager : VBoxContainer
{
	public ZombieGenericEditor theZombieTypeEditor;
	public ZombieGenericEditor nameEditor;
	public ZombieGenericEditor introduceEditor;
	public ZombieInfoDeliver infos;
	public override void _Ready()
	{
		theZombieTypeEditor = GetNode<ZombieGenericEditor>("./theZombieType/CodeEdit");
		nameEditor = GetNode<ZombieGenericEditor>("./name/CodeEdit");
		introduceEditor = GetNode<ZombieGenericEditor>("./introduce/CodeEdit");
		ZombieJsonTree tree = GetNode<ZombieJsonTree>("%Tree");
		infos = GetNode<ZombieInfoDeliver>("./info/infos");
		tree.OnZombieSelected += OnZombieSelected;
	}
	public void OnZombieSelected(int zombieType)
	{
		Zombie zombie = ZombieMain.zombieManager.zombies.Find(z => z.theZombieType == zombieType);
		if (zombie is null)
		{
			return;
		}
		theZombieTypeEditor.Text = zombieType.ToString();
		nameEditor.Text = zombie.name;
		introduceEditor.Text = zombie.introduce;
		theZombieTypeEditor.OnEditFinished();
		nameEditor.OnEditFinished();
		introduceEditor.OnEditFinished();
		infos.DeliverInfo(zombie.info);
	}

}
