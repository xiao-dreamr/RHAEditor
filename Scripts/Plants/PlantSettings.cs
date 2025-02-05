using Godot;
using System;
using System.Diagnostics;

public partial class PlantSettings : PopupMenu
{
	public override void _Ready()
	{
		IndexPressed += (long id) =>
		{
			switch ((int)id)
			{
				case 0:
					if (IsItemChecked(0))
					{
						// 若点击时已勾选，那么取消勾选，关闭音效
						Debug.WriteLine("关闭音效");
						SetItemChecked(0, false);
						GetNode<AudioStreamPlayer>("%Click").VolumeDb = -1000;
						SceneChanger.zombieMain.GetNode<AudioStreamPlayer>("./Click").VolumeDb = -1000;
						SceneChanger.zombieMain.GetNode<PopupMenu>("./Inside/VBox/MenuBar/设置").SetItemChecked(0, false);
					}
					else
					{
						// 若点击时未勾选，那么勾选，打开音效
						Debug.WriteLine("打开音效");
						SetItemChecked(0, true);
						GetNode<AudioStreamPlayer>("%Click").VolumeDb = -10;
						SceneChanger.zombieMain.GetNode<AudioStreamPlayer>("./Click").VolumeDb = -10;
						SceneChanger.zombieMain.GetNode<PopupMenu>("./Inside/VBox/MenuBar/设置").SetItemChecked(0, true);
					}
					break;
				case 1:
					if (IsItemChecked(1))
					{
						Debug.WriteLine("关闭自动应用");
						SetItemChecked(1, false);
						PlantMain.AutoApply = false;
						ZombieMain.AutoApply = false;
						SceneChanger.zombieMain.GetNode<PopupMenu>("./Inside/VBox/MenuBar/设置").SetItemChecked(0, false);
					}
					else
					{
						Debug.WriteLine("开启自动应用");
						SetItemChecked(1, true);
						PlantMain.AutoApply = true;
						ZombieMain.AutoApply = true;
						SceneChanger.zombieMain.GetNode<PopupMenu>("./Inside/VBox/MenuBar/设置").SetItemChecked(0, true);
					}
					break;
			}
		};
	}
}
