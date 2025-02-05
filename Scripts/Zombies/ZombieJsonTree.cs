using Godot;
using GodotPlugins.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class ZombieJsonTree : Tree
{
	[Signal]
	public delegate void OnZombieSelectedEventHandler(int zombieType);
	AudioStreamPlayer Click;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<LineEdit>("../Search").TextChanged += OnSearch;
		Click = GetNode<AudioStreamPlayer>("%Click");
		ItemSelected += OnSelected;
	}
	public void OnSearch(string keyword)
	{
		if (keyword.Contains('@'))
		{
			keyword = keyword[1..];
			// 全匹配
			RedisplayTree(ZombieMain.zombieManager.zombies.FindAll(zombie => zombie.introduce.Contains(keyword) || zombie.info.Contains(keyword) || zombie.name.Contains(keyword) || zombie.theZombieType.ToString().Contains(keyword)));
			return;
		}
		// 只匹配序号和名称
		RedisplayTree(ZombieMain.zombieManager.zombies.FindAll(zombie => zombie.name.Contains(keyword) || zombie.theZombieType.ToString().Contains(keyword)));
	}
	public void OnSelected()
	{
		int zombieType;
		try
		{
			zombieType = int.Parse(GetSelected().GetText(0).Split('：')[0]);
		}
		catch (Exception e)
		{
			zombieType = 0;
			Debug.WriteLine(e.Message);
		}
		EmitSignal(SignalName.OnZombieSelected, zombieType);
		Click.Play();
		return;
	}
	public void InitializeTree(List<Zombie> zombies)
	{
		Clear();
		TreeItem root = CreateItem();
		root.SetText(0, "僵尸");
		TreeItem Normal = CreateItem(root);
		Normal.SetText(0, "普通僵尸");
		TreeItem Planbie = CreateItem(root);
		Planbie.SetText(0, "植物僵尸");
		TreeItem Travel = CreateItem(root);
		Travel.SetText(0, "旅行僵尸");
		TreeItem Others = CreateItem(root);
		Others.SetText(0, "其他");
		ZombieMain.zombieManager.root = root;
		ZombieMain.zombieManager.ZombieTypeToCategories[ZombieType.Normal] = Normal;
		ZombieMain.zombieManager.ZombieTypeToCategories[ZombieType.Planbie] = Planbie;
		ZombieMain.zombieManager.ZombieTypeToCategories[ZombieType.Travel] = Travel;
		ZombieMain.zombieManager.ZombieTypeToCategories[ZombieType.Others] = Others;
		ConstructTree(zombies);
	}
	public void ConstructTree(List<Zombie> zombies)
	{
		Debug.WriteLine("ConstructTree");
		foreach (Zombie zombie in zombies)
		{
			AddItem(zombie);
		}
	}
	public void AddItem(Zombie zombie)
	{
		if (ZombieMain.zombieManager.GetCategoryDict(zombie).ContainsKey(zombie))
		{
			return;
		}
		TreeItem newItem = default;
		switch (ZombieMain.zombieManager.GetZombieType(zombie))
		{
			case ZombieType.Normal:
				newItem = CreateItem(ZombieMain.zombieManager.ZombieTypeToCategories[ZombieType.Normal]);
				break;
			case ZombieType.Planbie:
				newItem = CreateItem(ZombieMain.zombieManager.ZombieTypeToCategories[ZombieType.Planbie]);
				break;
			case ZombieType.Travel:
				newItem = CreateItem(ZombieMain.zombieManager.ZombieTypeToCategories[ZombieType.Travel]);
				break;
			case ZombieType.Others:
				newItem = CreateItem(ZombieMain.zombieManager.ZombieTypeToCategories[ZombieType.Others]);
				break;
			default:
				break;
		}
		newItem.SetText(0, zombie.GetDisplayName());
		ZombieMain.zombieManager.AddZombieItemPair(zombie, newItem);
		return;
	}
	public void InvisiableAllTree()
	{
		if (GetRoot() is null)
		{
			ZombieMain.CreateErrorWindow(GetTree().Root, "先打开一个ZombieStrings.json文件！");
			return;
		}
		foreach (var item in GetRoot().GetChildren())
		{
			foreach (var child in item.GetChildren())
			{
				child.Visible = false;
			}
			item.Visible = false;
		}
	}
	/// <summary>
	/// 在现有树的基础上进行Visiable筛选，不会添加新的植物
	/// </summary>
	/// <param name="plants"></param>
	public void RedisplayTree(List<Zombie> plants)
	{
		InvisiableAllTree();
		foreach (Zombie zombie in plants)
		{
			ZombieMain.zombieManager.GetCategoryDict(zombie)[zombie].SetText(0, zombie.GetDisplayName());
			ZombieMain.zombieManager.GetCategoryDict(zombie)[zombie].Visible = true;
			ZombieMain.zombieManager.GetCategoryDict(zombie)[zombie].GetParent().Visible = true;
		}
	}
#nullable enable
	public TreeItem? SearchItem(string itemText)
	{
		TreeItem? selected = null;
		foreach (var item in GetRoot().GetChildren())
		{
			selected = item.GetChildren().Where(x => x.GetText(0) == itemText).FirstOrDefault();
			if (selected is not null)
			{
				return selected;
			}
		}
		return selected;
	}
}

