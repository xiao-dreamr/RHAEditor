using System.IO;
using System.Linq;
using Godot;
using Newtonsoft.Json;

public partial class PlantFileManager : PopupMenu
{
	public override void _Ready()
	{
		IndexPressed += (long index) =>
		{
			// index0 = 打开
			// index1 = 保存
			// index2 = 另存为
			switch (index)
			{
				case 0:
					OnOpenFolder();
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
	public void OnOpenFolder()
	{
		GetNode<FileDialog>("/root/PlantMain/Inside/VBox/MenuBar/打开").Visible = true;
	}
	public void OnSave()
	{
		if (PlantMain.filePath == "")
		{
			PlantMain.CreateErrorWindow(this, "先新建或打开文件！");
			return;
		}
		string JsonString = JsonConvert.SerializeObject(
					new PlantMain.LawnString(PlantMain.PlantManager.plants.OrderBy(p => p.seedType).ToList())
					, Formatting.Indented);
		File.WriteAllText(PlantMain.filePath, JsonString);
		PlantMain.savedPlants = JsonConvert
			.DeserializeObject<PlantMain.LawnString>(
				File.ReadAllText(
					PlantMain.filePath.Replace("\\res:", "")
					)
			).plants; //重读保存的植物数据
	}
	public void OnSaveAs()
	{
		FileDialog 另存为 = GetNode<FileDialog>("/root/PlantMain/Inside/VBox/MenuBar/另存为");
		另存为.FileSelected += (path) =>
		{
			string JsonString = JsonConvert
				.SerializeObject(
					new PlantMain.LawnString(PlantMain.PlantManager.plants.OrderBy(p => p.seedType).ToList())
					, Formatting.Indented
				);
			PlantMain.savedPlants = JsonConvert
			.DeserializeObject<PlantMain.LawnString>(
				File.ReadAllText(
					path.Replace("\\res:", "")
					)
			).plants; //重读保存的植物数据
			File.WriteAllText(path, JsonString);
			PlantMain.filePath = path;
		};
		另存为.CurrentPath = PlantMain.filePath;
		另存为.AddFilter("*.json", "JSON 文件");
		另存为.Visible = true;
	}
}
