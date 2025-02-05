using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class LinkedButtonManager : HFlowContainer
{
	[Export]
	int maxButtonCount = 6;
	AudioStreamPlayer Click;
	PlantJsonTree tree;
	List<string> Tags = new(){
		"冰","火焰","樱","金","银","流明","磁","光","毁","魅","阳","铁","神","橄榄","桩","坚","三线","射手","海","水草","小喷","大喷","胆","蒜","西","窝","南","菜","玉米","黄油","桃","三叶","仙","盆","土","嘴","伞","地"
	};
	public override void _Ready()
	{
		tree = GetNode<PlantJsonTree>("/root/PlantMain/Inside/VBox/WorkPlace/JsonTree/Tree");
		Click = GetNode<AudioStreamPlayer>("%Click");
		tree.PlantSelected += OnPlantSelected;
	}
	public void OnPlantSelected(int seedType)
	{
		foreach (LinkedButton button in GetChildren().Cast<LinkedButton>())
		{
			// 先清空按钮
			button.QueueFree();
		}
		Plant plant = PlantMain.PlantManager.plants.Find(x => x.seedType == seedType);
		List<TreeItem> linkedPlantsItems = new();
		if (plant.info.Contains("融合配方："))
		{
			string[] linkedPlantNames = plant.info.Split("融合配方：</color>")[1].Split("</color>")[0].Replace("<color=red>", "").Replace("（无序）", "").Split("+");
			// 提取融合配方
			foreach (string linkedPlantName in linkedPlantNames)
			{
				//Debug.WriteLine("融合植物其一：" + linkedPlantName);
				Plant linkedPlant = PlantMain.PlantManager.plants.Find(x => x.name == linkedPlantName);
				if (linkedPlant is null || tree.SearchItem(linkedPlant.GetDisplayName()) is null)
				{
					continue;
				}
				linkedPlantsItems.Add(tree.SearchItem(linkedPlant.GetDisplayName()));
			}
		}
		// 显示关联植物
		foreach (string tag in Tags)
		{
			if (plant.name.Replace("向日葵", "阳").Replace("阳光", "阳").Replace("神", "大嘴").Replace("灯", "光").Contains(tag))
			{
				foreach (Plant p in PlantMain.PlantManager.plants.Where(x => x.name.Contains(tag) && x.seedType != seedType))
				{
					if (tree.SearchItem(p.GetDisplayName()) is null)
					{
						continue;
					}
					linkedPlantsItems.Add(tree.SearchItem(p.GetDisplayName()));
				}
			}
		}
		maxButtonCount = (int)Math.Min(Size.X * Size.Y / 6000, linkedPlantsItems.Count);
		linkedPlantsItems = linkedPlantsItems.DistinctBy(x => x.GetText(0)).Take(maxButtonCount).ToList(); // 去重
																										   // 显示按钮
		foreach (TreeItem item in linkedPlantsItems)
		{
			// 再新建
			LinkedButton button = new LinkedButton(item, Click);
			AddChild(button);
		}

	}
}
