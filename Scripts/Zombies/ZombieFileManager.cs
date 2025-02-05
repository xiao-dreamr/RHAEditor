using Godot;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

public partial class ZombieFileManager : PopupMenu
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		IndexPressed += (long index) =>
		{
			switch (index)
			{
				case 0:
					OnOpenPressed();
					break;
				case 1:
					OnSave();
					break;
				case 2:
					OnSaveAs();
					break;
			}
		};
	}
	public void OnOpenPressed()
	{
		GetNode<FileDialog>("%打开").Visible = true;
	}
	public void OnSave()
	{
		if (ZombieMain.filePath == "")
		{
			ZombieMain.CreateErrorWindow(GetTree().Root, "先新建或打开文件！");
			return;
		}
		string JsonString = JsonConvert.SerializeObject(
					new ZombieMain.ZombieString(ZombieMain.zombieManager.zombies.OrderBy(p => p.theZombieType).ToList())
					, Formatting.Indented);
		File.WriteAllText(ZombieMain.filePath, JsonString);
		ZombieMain.savedZombies = JsonConvert
			.DeserializeObject<ZombieMain.ZombieString>(
				File.ReadAllText(
					ZombieMain.filePath.Replace("\\res:", "")
					)
			).zombies; //重读保存的植物数据
	}
	public void OnSaveAs()
	{
		FileDialog 另存为 = GetNode<FileDialog>("%另存为");
		另存为.FileSelected += (path) =>
		{
			string JsonString = JsonConvert
				.SerializeObject(
					new ZombieMain.ZombieString(ZombieMain.zombieManager.zombies.OrderBy(p => p.theZombieType).ToList())
					, Formatting.Indented
				);
			ZombieMain.savedZombies = JsonConvert
			.DeserializeObject<ZombieMain.ZombieString>(
				File.ReadAllText(
					path.Replace("\\res:", "")
					)
			).zombies; //重读保存的植物数据
			File.WriteAllText(path, JsonString);
			ZombieMain.filePath = path;
		};
		另存为.CurrentPath = ZombieMain.filePath;
		另存为.AddFilter("*.json", "JSON 文件");
		另存为.Visible = true;
	}
}
