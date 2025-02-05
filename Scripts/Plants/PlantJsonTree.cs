using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class PlantJsonTree : Tree
{
	[Signal]
	public delegate void PlantSelectedEventHandler(int seedType);
	// Called when the node enters the scene tree for the first time.
	AudioStreamPlayer Click;
	public override void _Ready()
	{
		GetNode<LineEdit>("/root/PlantMain/Inside/VBox/WorkPlace/JsonTree/Search").TextChanged += OnSearch;
		ItemSelected += OnItemSelected;
		Click = GetNode<AudioStreamPlayer>("%Click");
	}
	public void OnSearch(string keyword)
	{
		if (keyword.Contains('@'))
		{
			keyword = keyword[1..];
			// 全匹配
			RedisplayTree(PlantMain.PlantManager.plants.FindAll(plant => plant.introduce.Contains(keyword) || plant.info.Contains(keyword) || plant.cost.Contains(keyword) || plant.name.Contains(keyword) || plant.seedType.ToString().Contains(keyword)));
			return;
		}
		// 只匹配序号和名称
		RedisplayTree(PlantMain.PlantManager.plants.FindAll(plant => plant.name.Contains(keyword) || plant.seedType.ToString().Contains(keyword)));
	}
	public void OnItemSelected()
	{
		try
		{
			Debug.WriteLine(GetSelected().GetText(0).Split('：')[0]);
		}
		catch (Exception e)
		{
			// 大概率是选到“融合”这类不符合格式的了，忽略即可
			Debug.WriteLine(e.Message);
		}
		// 信号只发送“：”前的部分，即种子编号
		EmitSignal(SignalName.PlantSelected, int.Parse(GetSelected().GetText(0).Split('：')[0]));
		Click.Play();
	}
	public void InitializeTree(List<Plant> plants)
	{
		Clear();
		TreeItem root = CreateItem();
		root.SetText(0, "植物");
		TreeItem vanilla = CreateItem(root);
		vanilla.SetText(0, "原版");
		TreeItem special = CreateItem(root);
		special.SetText(0, "特殊");
		TreeItem travel = CreateItem(root);
		travel.SetText(0, "旅行");
		TreeItem fusion = CreateItem(root);
		fusion.SetText(0, "融合");
		PlantMain.PlantManager.root = root;
		PlantMain.PlantManager.PlantTypeToCategroiesItem.Add(PlantType.Vanilla, vanilla);
		PlantMain.PlantManager.PlantTypeToCategroiesItem.Add(PlantType.Special, special);
		PlantMain.PlantManager.PlantTypeToCategroiesItem.Add(PlantType.Travel, travel);
		PlantMain.PlantManager.PlantTypeToCategroiesItem.Add(PlantType.Fusion, fusion);
		ConstructTree(plants);
	}
	/// <summary>
	/// 从视觉上删除所有Item，但实际上只是隐藏了它们，以便于重新构造
	/// </summary>
	public void InvisiableAllTree()
	{
		if (GetRoot() is null)
		{
			PlantMain.CreateErrorWindow(this, "先打开一个LawnStrings.json文件！");
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
	/// 将plants都添加到Tree中，并根据种子类型分类，且若已构建过，则不会删除原有的Item
	/// </summary>
	/// <param name="plants">要添加的植物列表</param>
	public void ConstructTree(List<Plant> plants)
	{
		foreach (Plant plant in plants)
		{
			AddItem(plant);
		}
		// 调整大小
		//CustomMinimumSize = new Vector2(0, (Main.PlantManager.plants.Count + 5) * 34 + 10);
	}
	/// <summary>
	/// 添加一个plant，并返回得到的TreeItem，若已存在则为已存在的TreeItem
	/// </summary>
	/// <param name="plant">要添加的植物</param>
	/// <returns>得到的TreeItem，若已存在则为已存在的TreeItem</returns>
	public void AddItem(Plant plant)
	{
		if (PlantMain.PlantManager.GetCategoryDict(plant).ContainsKey(plant))
		{
			// 若存在则跳过
			return;
		}
		TreeItem item = default; //即null
		switch (PlantMain.PlantManager.GetPlantType(plant))
		{
			case PlantType.Vanilla:
				item = CreateItem(PlantMain.PlantManager.PlantTypeToCategroiesItem[PlantType.Vanilla]);
				break;
			case PlantType.Special:
				item = CreateItem(PlantMain.PlantManager.PlantTypeToCategroiesItem[PlantType.Special]);
				break;
			case PlantType.Travel:
				item = CreateItem(PlantMain.PlantManager.PlantTypeToCategroiesItem[PlantType.Travel]);
				break;
			case PlantType.Fusion:
				item = CreateItem(PlantMain.PlantManager.PlantTypeToCategroiesItem[PlantType.Fusion]);
				break;
		}
		item.SetText(0, plant.GetDisplayName());
		PlantMain.PlantManager.AddPlantItemPair(plant, item);
		return;
	}

	/// <summary>
	/// 在现有树的基础上进行Visiable筛选，不会添加新的植物
	/// </summary>
	/// <param name="plants"></param>
	public void RedisplayTree(List<Plant> plants)
	{
		InvisiableAllTree();
		foreach (Plant plant in plants)
		{
			PlantMain.PlantManager.GetCategoryDict(plant)[plant].SetText(0, plant.GetDisplayName());
			PlantMain.PlantManager.GetCategoryDict(plant)[plant].Visible = true;
			PlantMain.PlantManager.GetCategoryDict(plant)[plant].GetParent().Visible = true;
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
