using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Linq;


public enum PlantType
{
	Vanilla,
	Special,
	Travel,
	Fusion

}
public class Plant
{
	public int seedType { get; set; }
	public string name { get; set; } = string.Empty;
	public string introduce { get; set; } = string.Empty;
	public string info { get; set; } = string.Empty;
	public string cost { get; set; } = string.Empty;
	public Plant(int seedType = 0)
	{
		this.seedType = seedType;
	}
	public string GetDisplayName()
	{
		return $"{seedType}：{name}";
	}

}

public class PlantsManager
{

	public TreeItem root;
	public List<Plant> plants = new();
	/// <summary>
	/// 将枚举转换为TreeItem的字典
	/// </summary>
	public Dictionary<PlantType, TreeItem> PlantTypeToCategroiesItem = new();
	public Dictionary<Plant, TreeItem> Vanilla = new();
	public Dictionary<Plant, TreeItem> Special = new();
	public Dictionary<Plant, TreeItem> Travel = new();
	public Dictionary<Plant, TreeItem> Fusion = new();
	/// <summary>
	/// 将植物添加到对应字典和树中，若已有则自动跳过
	/// </summary>
	/// <param name="plant">要添加的植物</param>
	/// <param name="tree">要添加到的树</param>
	public void AddPlantItemPair(Plant plant, TreeItem item)
	{
		switch (GetPlantType(plant))
		{
			case PlantType.Vanilla:
				Vanilla.Add(plant, item);
				break;
			case PlantType.Special:
				Special.Add(plant, item);
				break;
			case PlantType.Travel:
				Travel.Add(plant, item);
				break;
			case PlantType.Fusion:
				Fusion.Add(plant, item);
				break;
		}
	}
	public PlantType GetPlantType(Plant plant)
	{
		if (plant.seedType < 200)
		{
			return PlantType.Vanilla;
		}
		else if (plant.seedType < 900)
		{
			return PlantType.Special;
		}
		else if (plant.seedType < 1000)
		{
			return PlantType.Travel;
		}
		else
		{
			return PlantType.Fusion;
		}
	}
	/// <summary>
	/// 返回该plant所在的字典
	/// </summary>
	/// <param name="plant">要查询的植物</param>
	/// <returns>该plant所在的字典</returns>
	public Dictionary<Plant, TreeItem> GetCategoryDict(Plant plant)
	{
		switch (GetPlantType(plant))
		{
			case PlantType.Vanilla:
				return Vanilla;
			case PlantType.Special:
				return Special;
			case PlantType.Travel:
				return Travel;
			case PlantType.Fusion:
				return Fusion;
			default:
				return null;
		}
	}
	/// <summary>
	/// 获取TreeItem对应的分类字典
	/// </summary>
	/// <param name="item">TreeItem</param>
	/// <returns>TreeItem对应的分类字典</returns>
	public Dictionary<Plant, TreeItem> GetCategoryDict(TreeItem item)
	{
		switch (GetPlantType(GetPlant(item)))
		{
			case PlantType.Vanilla:
				return Vanilla;
			case PlantType.Special:
				return Special;
			case PlantType.Travel:
				return Travel;
			case PlantType.Fusion:
				return Fusion;
			default:
				return null;
		}
	}
	/// <summary>
	/// 获取TreeItem对应的植物，没有则Null
	/// </summary>
	/// <param name="item">TreeItem</param>
	/// <returns>TreeItem对应的植物，没有则Null</returns>
	public Plant GetPlant(TreeItem item)
	{
		if (Vanilla.ContainsValue(item))
		{
			return Vanilla.FirstOrDefault(x => x.Value == item).Key;
		}
		else if (Special.ContainsValue(item))
		{
			return Special.FirstOrDefault(x => x.Value == item).Key;
		}
		else if (Travel.ContainsValue(item))
		{
			return Travel.FirstOrDefault(x => x.Value == item).Key;
		}
		else if (Fusion.ContainsValue(item))
		{
			return Fusion.FirstOrDefault(x => x.Value == item).Key;
		}
		else
		{
			return null;
		}
	}
	/// <summary>
	/// 从树和字典中删除植物及TreeItem
	/// </summary>
	/// <param name="item">要删除的TreeItem</param>
	public void RemovePlant(TreeItem item)
	{
		plants.Remove(GetPlant(item));
		item.Visible = false;
		GetCategoryDict(item).Remove(GetPlant(item));
	}
	/// <summary>
	/// 从树和字典中删除植物及TreeItem
	/// </summary>
	/// <param name="plant">要删除的植物</param>
	public void RemovePlant(Plant plant)
	{
		plants.Remove(plant);
		GetCategoryDict(plant)[plant].Visible = false;
		GetCategoryDict(plant).Remove(plant);
	}
	/// <summary>
	/// 清空所有字典
	/// </summary>
	public void Clear()
	{
		Vanilla.Clear();
		Special.Clear();
		Travel.Clear();
		Fusion.Clear();
		plants.Clear();
		PlantTypeToCategroiesItem.Clear();
	}
}
public partial class PlantMain : Control
{
	public class LawnString
	{
		public List<Plant> plants = new();
		public LawnString(List<Plant> plants_)
		{
			plants = plants_;
		}
	}
	/// <summary>
	/// 作为“重置”时使用的plants，除保存时，其余情况不更改
	/// </summary>
	static public List<Plant> savedPlants = new();
	static public PlantsManager PlantManager = new();
	static public string filePath = "";
	static public void CreateErrorWindow(Node parent, string message)
	{
		AcceptDialog Tips = new()
		{
			Title = "笨蛋！",
			DialogText = message,
			OkButtonText = "了解"
		};
		parent.AddChild(Tips);
		Tips.PopupCentered();
		Tips.Show();
		Tips.Confirmed += Tips.QueueFree;
		return;
	}
	public override void _Ready()
	{
		FileDialog fileDialog = GetNode<FileDialog>("./Inside/VBox/MenuBar/打开");
		fileDialog.FileSelected += OnFileSelected;
	}
	public void OnFileSelected(string path)
	{
		if (path.Contains("Lawn") && path.Contains("Zombie"))
		{
			CreateErrorWindow(GetTree().Root, "请选择正确的LawnStrings.json文件！");
			return;
		}
		else if (path.Contains("Zombie"))
		{
			SceneChanger.ChangeToZombie(GetTree().Root, path);
			return;
		}
		else if (!path.Contains("Lawn"))
		{
			CreateErrorWindow(GetTree().Root, "请选择正确的LawnStrings.json文件！");
			return;
		}
		if (PlantManager.plants.Count > 0)
		{
			PlantManager.Clear();
		}
		filePath = path;
		Debug.WriteLine($"读取json文件中...{path.Replace("\\res:", "")}");
		PlantManager.Clear();
		PlantManager.plants = JsonConvert.DeserializeObject<LawnString>(File.ReadAllText(path.Replace("\\res:", ""))).plants;
		savedPlants = JsonConvert.DeserializeObject<LawnString>(File.ReadAllText(path.Replace("\\res:", ""))).plants;
		GetNode<PlantJsonTree>("/root/PlantMain/Inside/VBox/WorkPlace/JsonTree/Tree").InitializeTree(PlantManager.plants);
	}
}
