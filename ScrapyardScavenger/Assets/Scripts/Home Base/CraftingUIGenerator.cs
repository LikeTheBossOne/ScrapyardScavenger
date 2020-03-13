using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUIGenerator : MonoBehaviour
{
	private List<CraftableObject> possibleCrafts;
	public GameObject controller;

	public CraftableObject chainArmor;
	//private CraftableObject[] craftables = new CraftableObject[] {new ChainmailArmor(), new LeatherArmor(), new MetalArmor(), new EnergyDrink(), new Medpack()};

	// Start is called before the first frame update
    void Start()
    {
		possibleCrafts = new List<CraftableObject>();
		controller = GameObject.FindGameObjectsWithTag("GameController")[0];
    }

	public void GenerateListOfCraftables()
	{
		possibleCrafts.Clear();
		List<ResourcePersistent> currentResources = controller.GetComponent<EquipmentManager>().getResources();
		HashSet<Resource> resourceSet = controller.GetComponent<EquipmentManager>().getResourceSet();


	}

}
