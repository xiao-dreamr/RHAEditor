using Godot;

public partial class PlantEditorManager : VBoxContainer
{
	public PlantGenericEdit seedTypeEdit;
	public PlantGenericEdit nameEdit;
	public PlantGenericEdit introduceEdit;
	public PlantGenericEdit costEdit;
	public PlantInfoDeliver infos;
	public override void _Ready()
	{
		seedTypeEdit = GetNode<PlantGenericEdit>("./seedType/seedTypeEdit");
		nameEdit = GetNode<PlantGenericEdit>("./name/nameEdit");
		introduceEdit = GetNode<PlantGenericEdit>("./introduce/introduceEdit");
		costEdit = GetNode<PlantGenericEdit>("./cost/costEdit");
		infos = GetNode<PlantInfoDeliver>("./info/Infos");
		PlantJsonTree tree = GetNode<PlantJsonTree>("/root/PlantMain/Inside/VBox/WorkPlace/JsonTree/Tree");
		tree.PlantSelected += OnPlantSelected;
	}
	public void OnPlantSelected(int seedType)
	{
		// 选中植物后，将其信息填入相应的编辑框中
		Plant plant = PlantMain.PlantManager.plants.Find(x => x.seedType == seedType);
		seedTypeEdit.Text = seedType.ToString();
		seedTypeEdit.OnEditFinished();
		nameEdit.Text = plant.name;
		nameEdit.OnEditFinished();
		introduceEdit.Text = plant.introduce;
		introduceEdit.OnEditFinished();
		costEdit.Text = plant.cost.ToString();
		costEdit.OnEditFinished();
		infos.DiliverInfo(seedType);
	}
}
