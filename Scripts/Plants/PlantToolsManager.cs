using Godot;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

public partial class PlantToolsManager : HBoxContainer
{
	[Signal]
	public delegate void DeletedEventHandler();
	TextureButton 新建;
	TextureButton 删除;
	TextureButton 应用;
	TextureButton 重置;
	TextureButton 新建并复制;
	PlantJsonTree tree;
	PlantEditorManager Editor;
	AudioStreamPlayer Click;

	public override void _Ready()
	{
		Click = GetNode<AudioStreamPlayer>("%Click");
		新建 = GetNode<TextureButton>("./新建");
		删除 = GetNode<TextureButton>("./删除");
		应用 = GetNode<TextureButton>("./应用");
		重置 = GetNode<TextureButton>("./重置");
		新建并复制 = GetNode<TextureButton>("./新建并复制");
		tree = GetNode<PlantJsonTree>("/root/PlantMain/Inside/VBox/WorkPlace/JsonTree/Tree");
		Editor = GetNode<PlantEditorManager>("/root/PlantMain/Inside/VBox/WorkPlace/Editor");
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
		if (tree.GetRoot() is null)
		{
			PlantMain.CreateErrorWindow(this, "请先打开LawnStrings.json文件！");
			return;
		}
		int id;
		if (Editor.seedTypeEdit.Text.Length == 0)
		{
			// 未选择植物时，新建在最后
			id = PlantMain.PlantManager.plants.Last().seedType + 1;
		}
		else
		{
			// 总是新建在当前大类下的最后一株植物后面
			// 比如当在“22 路灯花”的界面按新建时，会在“原版”分类下的最后一株植物“32 西瓜投手”后面新建第33个空白植物
			id = PlantMain.PlantManager.GetCategoryDict(tree.GetSelected()).Count;
		}

		Plant newPlant = new(id);
		PlantMain.PlantManager.plants.Add(newPlant);
		tree.ConstructTree(PlantMain.PlantManager.plants);
		EmitSignal(SignalName.Deleted); // 先删除现有内容，但不删除plant
		PlantMain.PlantManager.plants = PlantMain.PlantManager.plants.OrderBy(x => x.seedType).ToList(); //排序
		Editor.seedTypeEdit.Text = newPlant.seedType.ToString();
		// 以下是为了选中刚新建的植物并滚动到屏幕中
#nullable enable
		TreeItem? selected = tree.SearchItem(newPlant.GetDisplayName());
		Debug.WriteLine("新增植物  " + newPlant.GetDisplayName());
		if (selected is not null)
		{
			tree.SetSelected(selected, 0);
			tree.ScrollToItem(selected);
		}
#nullable restore
		Click.Play();
		return;
	}
	public void CopyAndNewPressed()
	{
		if (tree.GetRoot() is null)
		{
			PlantMain.CreateErrorWindow(this, "请先打开LawnStrings.json文件！");
			return;
		}
		else if (Editor.seedTypeEdit.Text.Length == 0)
		{
			PlantMain.CreateErrorWindow(this, "请先选中一株植物，\n使seedType项不为空！");
			return;
		}
		Plant selectedPlant = PlantMain.PlantManager.plants.Find(x => x.seedType == Editor.seedTypeEdit.Text.ToInt());
		int id = PlantMain.PlantManager
			.GetCategoryDict(tree.GetSelected()).Count; // 总是新建在当前大类下的最后一株植物后面
														// 比如当在“22 路灯花”的界面按新建时，会在“原版”分类下的最后一株植物“32 西瓜投手”后面新建第33个空白植物;
		EmitSignal(SignalName.Deleted); // 先删除现有内容，但不删除plant
		Plant newPlant = DeepCopyPlant(selectedPlant); // 从选中的植物复制属性
		newPlant.seedType = id; // 保留新植物id不改变
		newPlant.name = selectedPlant.name + "-复制"; // 给新植物加上“复制”后缀
		PlantMain.PlantManager.plants.Add(newPlant);
		tree.ConstructTree(PlantMain.PlantManager.plants);
		PlantMain.PlantManager.plants = PlantMain.PlantManager.plants.OrderBy(x => x.seedType).ToList(); //排序
		Editor.seedTypeEdit.Text = newPlant.seedType.ToString();
#nullable enable
		TreeItem? selected = tree.SearchItem(newPlant.GetDisplayName());
		Debug.WriteLine("新增植物  " + newPlant.GetDisplayName());
		if (selected is not null)
		{
			tree.SetSelected(selected, 0);
			tree.ScrollToItem(selected);
		}
#nullable restore
		Click.Play();
		return;
	}
	public void DeleteConfirmed()
	{
		if (tree.GetRoot() is null)
		{
			PlantMain.CreateErrorWindow(this, "请先打开LawnStrings.json文件！");
			return;
		}
		else if (Editor.seedTypeEdit.Text.Length == 0)
		{
			PlantMain.CreateErrorWindow(this, "请先选中一株植物，\n使seedType项不为空！");
			return;
		}
		Plant p = PlantMain.PlantManager.plants.Find(x => x.seedType == Editor.seedTypeEdit.Text.ToInt());
		if (p is not null)
		{
			PlantMain.PlantManager.RemovePlant(p);
			tree.ConstructTree(PlantMain.PlantManager.plants);
		}
		EmitSignal(SignalName.Deleted);
		Click.Play();
		return;
	}
	public void ApplyPressed()
	{
		if (tree.GetRoot() is null)
		{
			PlantMain.CreateErrorWindow(this, "请先打开LawnStrings.json文件！");
			return;
		}
		else if (Editor.seedTypeEdit.Text.Length == 0)
		{
			PlantMain.CreateErrorWindow(this, "请先选中一株植物，\n使seedType项不为空！");
			return;
		}
		Plant p = PlantMain.PlantManager.plants.Find(x => x.seedType == Editor.seedTypeEdit.Text.Replace("<color=#3D1400>", "").Replace("</color>", "").ToInt());
		if (p is null)
		{
			p = new(Editor.seedTypeEdit.Text.Replace("<color=#3D1400>", "").Replace("</color>", "").ToInt());
			PlantMain.PlantManager.plants.Add(p);
			tree.ConstructTree(PlantMain.PlantManager.plants);
		}
		// 设置植物信息
		p.name = Editor.nameEdit.Text.Replace("<color=#3D1400>", "").Replace("</color>", "");
		p.seedType = Editor.seedTypeEdit.Text.Replace("<color=#3D1400>", "").Replace("</color>", "").ToInt();
		p.cost = Editor.costEdit.Text;
		p.introduce = Editor.introduceEdit.Text; //
		p.info = new Regex(@"((?:\r?\n){3,})|(?:\r?\n){2}")
			.Replace($"{Editor.infos.简介.Text}\n{Editor.infos.韧性.Text}\n{Editor.infos.伤害.Text}\n{Editor.infos.特点.Text}\n{Editor.infos.其他.Text}\n{Editor.infos.配方.Text}", "\n")
			.TrimEnd('\n'); // 这样可以过滤掉连续的\n
		p.info = new Regex("\n").Replace(p.info, "\n\n", 1);
		tree.RedisplayTree(PlantMain.PlantManager.plants);
		tree.SetSelected(tree.SearchItem(p.GetDisplayName()), 0); //选中刚修改的植物
		Click.Play();
		return;
	}
	public void ResetConfirmed()
	{
		if (tree.GetRoot() is null)
		{
			PlantMain.CreateErrorWindow(this, "请先打开LawnStrings.json文件！");
			return;
		}
		else if (Editor.seedTypeEdit.Text.Length == 0)
		{
			PlantMain.CreateErrorWindow(this, "请先选中一株植物，\n使seedType项不为空！");
			return;
		}
		Plant savedPlant = PlantMain.savedPlants.Find(x => x.seedType == Editor.seedTypeEdit.Text.ToInt()); //先获取保存的植物
		if (savedPlant is not null)
		{
			Plant editingPlant = PlantMain.PlantManager.plants.Where(x => x.seedType == savedPlant.seedType).First();
			editingPlant.name = savedPlant.name;
			editingPlant.cost = savedPlant.cost;
			editingPlant.introduce = savedPlant.introduce;
			editingPlant.info = savedPlant.info;
			Editor.OnPlantSelected(savedPlant.seedType); //刷新界面
			PlantMain.PlantManager.plants = PlantMain.PlantManager.plants.OrderBy(x => x.seedType).ToList(); //排序
			tree.RedisplayTree(PlantMain.PlantManager.plants); //刷新树形结构
		}
		Click.Play();
		return;
	}
	public static Plant DeepCopyPlant(Plant plant)
	{
		Plant p = (Plant)Activator.CreateInstance(typeof(Plant), 0);
		PropertyInfo[] PI = typeof(Plant).GetProperties();
		for (int i = 0; i < PI.Length; i++)
		{
			PropertyInfo P = PI[i];
			P.SetValue(p, P.GetValue(plant));
		}
		return p;
	}
}
