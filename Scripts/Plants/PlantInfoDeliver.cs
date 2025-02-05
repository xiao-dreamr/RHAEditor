using Godot;
using System;
using System.Linq;

public partial class PlantInfoDeliver : VBoxContainer
{
	public PlantGenericEdit 简介;
	public PlantGenericEdit 特点;
	public PlantGenericEdit 韧性;
	public PlantGenericEdit 伤害;
	public PlantGenericEdit 其他;
	public PlantGenericEdit 配方;
	public override void _Ready()
	{
		简介 = GetNode<PlantGenericEdit>("./简介");
		特点 = GetNode<PlantGenericEdit>("./特点");
		韧性 = GetNode<PlantGenericEdit>("./韧性");
		伤害 = GetNode<PlantGenericEdit>("./伤害");
		其他 = GetNode<PlantGenericEdit>("./其他");
		配方 = GetNode<PlantGenericEdit>("./融合配方");
	}
	public void DiliverInfo(int seedType)
	{
		string info = PlantMain.PlantManager.plants.Find(p => p.seedType == seedType).info;
		string[] infos = info.Split('\n', StringSplitOptions.RemoveEmptyEntries);
		string others = string.Empty;
		特点.Text = string.Empty;
		韧性.Text = string.Empty;
		伤害.Text = string.Empty;
		配方.Text = string.Empty;
		简介.Text = string.Empty;
		if (infos.Length == 0)
		{
			简介.Display();
			其他.Display();
			特点.Display();
			韧性.Display();
			伤害.Display();
			配方.Display();
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

			else if (s.Contains("融合配方："))
			{
				配方.Text = s;

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
			配方.Display();
		}
	}
}
