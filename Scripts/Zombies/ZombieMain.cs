using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

public class Zombie
{
	public int theZombieType { get; set; }
	public string name { get; set; }
	public string introduce { get; set; }
	public string info { get; set; }
	public string GetDisplayName()
	{
		return $"{theZombieType}：{name}";
	}
}

public enum ZombieType
{
	Normal,
	/// <summary>
	/// 植物僵尸
	/// </summary>
	Planbie,
	/// <summary>
	/// 旅行僵尸
	/// </summary>
	Travel,
	Others,
}

public class ZombieManager
{
	public TreeItem root;
	public List<Zombie> zombies = new();
	public Dictionary<ZombieType, TreeItem> ZombieTypeToCategories = new();
	public Dictionary<Zombie, TreeItem> Normal = new();
	public Dictionary<Zombie, TreeItem> Planbie = new();
	public Dictionary<Zombie, TreeItem> Travel = new();
	public Dictionary<Zombie, TreeItem> Others = new();
	/// <summary>
	/// 添加僵尸-TreeItem键值对
	/// </summary>
	/// <param name="zombie">要添加的僵尸</param>
	/// <param name="item">要添加的TreeItem</param>
	public void AddZombieItemPair(Zombie zombie, TreeItem item)
	{
		switch (GetZombieType(zombie))
		{
			case ZombieType.Normal:
				Normal.Add(zombie, item);
				break;
			case ZombieType.Planbie:
				Planbie.Add(zombie, item);
				break;
			case ZombieType.Travel:
				Travel.Add(zombie, item);
				break;
			case ZombieType.Others:
				Others.Add(zombie, item);
				break;
		}
	}
	public ZombieType GetZombieType(Zombie zombie)
	{
		int type = zombie.theZombieType;
		if (type == 105 || type == 110 || type == 215)
		{
			// 盲盒僵尸-105
			// 黄金盲盒-110
			// 钻石盲盒-215
			// MAGIC NUMBERS!!!!!!!!!!!
			return ZombieType.Others;
		}
		else if (type < 100)
		{
			return ZombieType.Normal;
		}
		else if (type < 200)
		{
			return ZombieType.Planbie;
		}
		else
		{
			return ZombieType.Travel;
		}
	}
	/// <summary>
	/// 获取TreeItem对应的僵尸
	/// </summary>
	/// <param name="item">要获取的TreeItem</param>
	/// <returns>对应的僵尸</returns>
	public Zombie GetZombie(TreeItem item)
	{
		if (Normal.ContainsValue(item))
		{
			return Normal.FirstOrDefault(x => x.Value == item).Key;
		}
		else if (Planbie.ContainsValue(item))
		{
			return Planbie.FirstOrDefault(x => x.Value == item).Key;
		}
		else if (Travel.ContainsValue(item))
		{
			return Travel.FirstOrDefault(x => x.Value == item).Key;
		}
		else if (Others.ContainsValue(item))
		{
			return Others.FirstOrDefault(x => x.Value == item).Key;
		}
		else
		{
			return null;
		}
	}
	/// <summary>
	/// 获取僵尸类型对应的TreeItem字典
	/// </summary>
	/// <param name="zombie"> 要获取的僵尸</param>
	/// <returns> 僵尸类型对应的TreeItem字典</returns>
	public Dictionary<Zombie, TreeItem> GetCategoryDict(Zombie zombie)
	{
		switch (GetZombieType(zombie))
		{
			case ZombieType.Normal:
				return Normal;
			case ZombieType.Planbie:
				return Planbie;
			case ZombieType.Travel:
				return Travel;
			case ZombieType.Others:
				return Others;
			default:
				return null;
		}
	}
	/// <summary>
	/// 获取TreeItem对应的僵尸类型对应的TreeItem字典
	/// </summary>
	/// <param name="item">要获取的TreeItem</param>
	/// <returns> 对应的僵尸类型对应的TreeItem字典</returns>
	public Dictionary<Zombie, TreeItem> GetCategoryDict(TreeItem item)
	{
		return GetCategoryDict(GetZombie(item));
	}
	/// <summary>
	/// 移除僵尸
	/// </summary>
	/// <param name="zombie">要移除的僵尸</param>
	public void RemoveZombie(Zombie zombie)
	{
		zombies.Remove(zombie);
		GetCategoryDict(zombie)[zombie].Visible = false;
		GetCategoryDict(zombie).Remove(zombie);
	}
	/// <summary>
	/// 移除TreeItem对应的僵尸
	/// </summary>
	/// <param name="item">要移除的TreeItem</param>
	public void RemoveZombie(TreeItem item)
	{
		RemoveZombie(GetZombie(item));
	}
	/// <summary>
	/// 清空所有僵尸
	/// </summary>
	public void Clear()
	{
		zombies.Clear();
		Normal.Clear();
		Planbie.Clear();
		Travel.Clear();
		Others.Clear();
		ZombieTypeToCategories.Clear();
	}
}

public partial class ZombieMain : Control
{
	public class ZombieString
	{
		public List<Zombie> zombies = new();
		public ZombieString(List<Zombie> zombies)
		{
			this.zombies = zombies;
		}
	}
	static public ZombieManager zombieManager = new();
	static public List<Zombie> savedZombies = new();
	static public string filePath = "";
	public override void _Ready()
	{
		FileDialog fileDialog = GetNode<FileDialog>("%打开");
		fileDialog.FileSelected += OnFileSelected;
	}
	static public void CreateErrorWindow(Node parent, string message)
	{
		Debug.WriteLine($"创建错误窗口：{message}");
		AcceptDialog Tips = new()
		{
			Name = "Tips",
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
	public void OnFileSelected(string path)
	{
		if (path.Contains("Lawn") && path.Contains("Zombie"))
		{
			CreateErrorWindow(GetTree().Root, "请选择正确的LawnStrings.json文件！");
			return;
		}
		else if (path.Contains("Lawn"))
		{
			SceneChanger.ChangeToPlant(GetTree().Root, path);
			return;
		}
		else if (!path.Contains("Zombie"))
		{

			CreateErrorWindow(GetTree().Root, "请选择正确的json文件！");
			return;
		}

		if (zombieManager.zombies.Count > 0)
		{
			zombieManager.Clear();
		}
		filePath = path;
		Debug.WriteLine($"读取json文件中...{path.Replace("\\res:", "")}");
		zombieManager.zombies = JsonConvert.DeserializeObject<ZombieString>(File.ReadAllText(path.Replace("\\res:", ""))).zombies;
		savedZombies = JsonConvert.DeserializeObject<ZombieString>(File.ReadAllText(path.Replace("\\res:", ""))).zombies;
		Debug.WriteLine($"读取json文件完成！");
		Debug.WriteLine($"僵尸数量：{zombieManager.zombies.Count}");
		Debug.WriteLine($"首位僵尸验证：{zombieManager.zombies[0].GetDisplayName()}");
		Debug.WriteLine($"初始化树结构中...");
		GetNode<ZombieJsonTree>("%Tree").InitializeTree(zombieManager.zombies);
	}
}
