using Godot;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

public partial class ZombieToolsManager : HBoxContainer
{
	[Signal]
	public delegate void DeletedEventHandler();
	TextureButton 新建;
	TextureButton 删除;
	TextureButton 应用;
	TextureButton 重置;
	TextureButton 新建并复制;
	ZombieJsonTree tree;
	ZombieEditorManager Editor;
	AudioStreamPlayer Click;

	public override void _Ready()
	{
		Click = GetNode<AudioStreamPlayer>("%GraveClick");
		新建 = GetNode<TextureButton>("./新建");
		删除 = GetNode<TextureButton>("./删除");
		应用 = GetNode<TextureButton>("./应用");
		重置 = GetNode<TextureButton>("./重置");
		新建并复制 = GetNode<TextureButton>("./新建并复制");
		tree = GetNode<ZombieJsonTree>("%Tree");
		Editor = GetNode<ZombieEditorManager>("/root/ZombieMain/Inside/VBox/WorkPlace/Editor");
		ConfirmationDialog 确认删除 = GetNode<ConfirmationDialog>("./确认删除");
		ConfirmationDialog 确认重置 = GetNode<ConfirmationDialog>("./确认重置");
		新建.Pressed += NewPressed;
		删除.Pressed += () => { 确认删除.PopupCentered(); };
		确认删除.Confirmed += DeleteConfirmed;
		应用.Pressed += ApplyPressed;
		重置.Pressed += () => { 确认重置.PopupCentered(); };
		确认重置.Confirmed += ResetConfirmed;
		新建并复制.Pressed += CopyAndNewPressed;
	}

	public void NewPressed()
	{
		Click.Play();
		if (tree.GetRoot() is null)
		{
			ZombieMain.CreateErrorWindow(GetTree().Root, "请先打开ZombieStrings.json文件！");
			return;
		}
		int id;
		if (Editor.theZombieTypeEditor.Text.Length == 0)
		{
			// 未选择僵尸时，新建在最后
			id = ZombieMain.zombieManager.zombies.Last().theZombieType + 1;
		}
		else
		{
			// 总是新建在当前大类下的最后一个僵尸后面
			id = ZombieMain.zombieManager.GetCategoryDict(tree.GetSelected()).Last().Key.theZombieType + 1;
		}

		Zombie newZombie = new()
		{
			theZombieType = id
		};
		ZombieMain.zombieManager.zombies.Add(newZombie);
		tree.ConstructTree(ZombieMain.zombieManager.zombies);
		EmitSignal(SignalName.Deleted); // 先删除现有内容，但不删除zombie
		ZombieMain.zombieManager.zombies = ZombieMain.zombieManager.zombies.OrderBy(x => x.theZombieType).ToList(); //排序
		Editor.theZombieTypeEditor.Text = newZombie.theZombieType.ToString();
		// 以下是为了选中刚新建的僵尸并滚动到屏幕中
#nullable enable
		TreeItem? selected = tree.SearchItem(newZombie.GetDisplayName());
		Debug.WriteLine("新增僵尸  " + newZombie.GetDisplayName());
		if (selected is not null)
		{
			tree.SetSelected(selected, 0);
			tree.ScrollToItem(selected);
		}
#nullable restore

		return;
	}
	public void CopyAndNewPressed()
	{
		Click.Play();
		if (tree.GetRoot() is null)
		{
			ZombieMain.CreateErrorWindow(GetTree().Root, "请先打开ZombieStrings.json文件！");
			return;
		}
		else if (Editor.theZombieTypeEditor.Text.Length == 0)
		{
			ZombieMain.CreateErrorWindow(GetTree().Root, "请先选中一个僵尸，\n使theZombieType项不为空！");
			return;
		}
		Zombie selectedZombie = ZombieMain.zombieManager.zombies.Find(x => x.theZombieType == Editor.theZombieTypeEditor.Text.ToInt());
		int id = ZombieMain.zombieManager
			.GetCategoryDict(tree.GetSelected()).Last().Key.theZombieType + 1; // 总是新建在当前大类下的最后一个僵尸后面
																			   // 比如当在“0 普通僵尸”的界面按新建时，会在“原版”分类下的最后一个僵尸“45 蹦极僵尸”后面新建第46个空白僵尸;
		EmitSignal(SignalName.Deleted); // 先删除现有内容，但不删除zombie
		Zombie newZombie = DeepCopyzombie(selectedZombie); // 从选中的僵尸复制属性
		newZombie.theZombieType = id; // 保留新僵尸id不改变
		newZombie.name = selectedZombie.name + "-复制"; // 给新僵尸加上“复制”后缀
		ZombieMain.zombieManager.zombies.Add(newZombie);
		tree.ConstructTree(ZombieMain.zombieManager.zombies);
		ZombieMain.zombieManager.zombies = ZombieMain.zombieManager.zombies.OrderBy(x => x.theZombieType).ToList(); //排序
		Editor.theZombieTypeEditor.Text = newZombie.theZombieType.ToString();
#nullable enable
		TreeItem? selected = tree.SearchItem(newZombie.GetDisplayName());
		Debug.WriteLine("新增僵尸  " + newZombie.GetDisplayName());
		if (selected is not null)
		{
			tree.SetSelected(selected, 0);
			tree.ScrollToItem(selected);
		}
#nullable restore

		return;
	}
	public void DeleteConfirmed()
	{
		Click.Play();
		if (tree.GetRoot() is null)
		{
			ZombieMain.CreateErrorWindow(GetTree().Root, "请先打开ZombieStrings.json文件！");
			return;
		}
		else if (Editor.theZombieTypeEditor.Text.Length == 0)
		{
			ZombieMain.CreateErrorWindow(GetTree().Root, "请先选中一个僵尸，\n使theZombieType项不为空！");
			return;
		}
		Zombie z = ZombieMain.zombieManager.zombies.Find(x => x.theZombieType == Editor.theZombieTypeEditor.Text.ToInt());
		if (z is not null)
		{
			ZombieMain.zombieManager.RemoveZombie(z);
			tree.ConstructTree(ZombieMain.zombieManager.zombies);
		}
		EmitSignal(SignalName.Deleted);

		return;
	}
	public void ApplyPressed()
	{
		Click.Play();
		if (tree.GetRoot() is null)
		{
			ZombieMain.CreateErrorWindow(GetTree().Root, "请先打开ZombieStrings.json文件！");
			return;
		}
		else if (Editor.theZombieTypeEditor.Text.Length == 0)
		{
			ZombieMain.CreateErrorWindow(GetTree().Root, "请先选中一个僵尸，\n使theZombieType项不为空！");
			return;
		}
		Zombie z = ZombieMain.zombieManager.zombies.Find(x => x.theZombieType == Editor.theZombieTypeEditor.Text.Replace("<color=#3D1400>", "").Replace("</color>", "").ToInt());
		if (z is null)
		{
			z = new()
			{
				theZombieType = Editor.theZombieTypeEditor.Text.Replace("<color=#3D1400>", "").Replace("</color>", "").ToInt()
			};
			ZombieMain.zombieManager.zombies.Add(z);
			tree.ConstructTree(ZombieMain.zombieManager.zombies);
		}
		// 设置僵尸信息
		z.name = Editor.nameEditor.Text.Replace("<color=#3D1400>", "").Replace("</color>", "");
		z.theZombieType = Editor.theZombieTypeEditor.Text.Replace("<color=#3D1400>", "").Replace("</color>", "").ToInt();
		z.introduce = Editor.introduceEditor.Text; //
		z.info = new Regex(@"((?:\r?\n){3,})|(?:\r?\n){2}")
			.Replace($"{Editor.infos.简介.Text}\n{Editor.infos.韧性.Text}\n{Editor.infos.伤害.Text}\n{Editor.infos.特点.Text}\n{Editor.infos.其他.Text}", "\n")
			.TrimEnd('\n'); // 这样可以过滤掉连续的\n
		z.info = new Regex("\n").Replace(z.info, "\n\n", 1);
		tree.RedisplayTree(ZombieMain.zombieManager.zombies);
		tree.SetSelected(tree.SearchItem(z.GetDisplayName()), 0); //选中刚修改的僵尸

		return;
	}
	public void ResetConfirmed()
	{
		Click.Play();
		if (tree.GetRoot() is null)
		{
			ZombieMain.CreateErrorWindow(GetTree().Root, "请先打开ZombieStrings.json文件！");
			return;
		}
		else if (Editor.theZombieTypeEditor.Text.Length == 0)
		{
			ZombieMain.CreateErrorWindow(GetTree().Root, "请先选中一个僵尸，\n使theZombieType项不为空！");
			return;
		}
		Zombie savedZombie = ZombieMain.savedZombies.Find(x => x.theZombieType == Editor.theZombieTypeEditor.Text.ToInt()); //先获取保存的僵尸
		if (savedZombie is not null)
		{
			Zombie editingZombie = ZombieMain.zombieManager.zombies.Where(x => x.theZombieType == savedZombie.theZombieType).First();
			editingZombie.name = savedZombie.name;
			editingZombie.introduce = savedZombie.introduce;
			editingZombie.info = savedZombie.info;
			Editor.OnZombieSelected(savedZombie.theZombieType); //刷新界面
			ZombieMain.zombieManager.zombies = ZombieMain.zombieManager.zombies.OrderBy(x => x.theZombieType).ToList(); //排序
			tree.RedisplayTree(ZombieMain.zombieManager.zombies); //刷新树形结构
		}

		return;
	}
	public static Zombie DeepCopyzombie(Zombie zombie)
	{
		Zombie z = (Zombie)Activator.CreateInstance(typeof(Zombie));
		PropertyInfo[] PI = typeof(Zombie).GetProperties();
		for (int i = 0; i < PI.Length; i++)
		{
			PropertyInfo P = PI[i];
			P.SetValue(z, P.GetValue(zombie));
		}
		return z;
	}
}
