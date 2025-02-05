using Godot;
using System;
using System.Linq;

public partial class ZombieInfoDeliver : VBoxContainer
{
	public ZombieGenericEditor 简介;
	public ZombieGenericEditor 伤害;
	public ZombieGenericEditor 韧性;
	public ZombieGenericEditor 特点;
	public ZombieGenericEditor 其他;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		简介 = GetNode<ZombieGenericEditor>("./简介");
		伤害 = GetNode<ZombieGenericEditor>("./伤害");
		韧性 = GetNode<ZombieGenericEditor>("./韧性");
		特点 = GetNode<ZombieGenericEditor>("./特点");
		其他 = GetNode<ZombieGenericEditor>("./其他");
	}
	public void DeliverInfo(string info)
	{
		string[] infos = Array.Empty<string>();
		if (info != null && info.Length > 0)
		{
			infos = info.Split('\n', StringSplitOptions.RemoveEmptyEntries);
		}
		string others = string.Empty;
		特点.Text = string.Empty;
		韧性.Text = string.Empty;
		伤害.Text = string.Empty;
		简介.Text = string.Empty;
		if (infos.Length == 0)
		{
			简介.Display();
			其他.Display();
			特点.Display();
			韧性.Display();
			伤害.Display();
			return;
		}
		简介.Text = infos[0];
		简介.Display();
		foreach (string s in infos.Skip(1))
		{
			if (s.Contains("特点："))
			{
				特点.Text = s;

			}
			else if (s.Contains("韧性："))
			{
				韧性.Text = s;

			}
			else if (s.Contains("伤害："))
			{
				伤害.Text = s;

			}
			else
			{
				others += s + "\n";
			}
			其他.Text = others.TrimEnd('\n');
			其他.Display();
			特点.Display();
			韧性.Display();
			伤害.Display();
		}
	}
}
