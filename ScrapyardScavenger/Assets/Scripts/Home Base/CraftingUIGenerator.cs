using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUIGenerator : MonoBehaviour
{
	private List<CraftableObject> possibleCrafts;
	public GameObject controller;
	public GameObject craftablePrefab;
	public GameObject container;
	private List<GameObject> cItems = new List<GameObject>();

	public CraftableObject chainArmor;
	public CraftableObject leatherArmor;
	public CraftableObject metalArmor;
	public CraftableObject energyDrink;
	public CraftableObject medpack;
	public CraftableObject arGun;
	public CraftableObject metalRod;
	public CraftableObject woodenBat;
	private CraftableObject[] craftables;

	// Start is called before the first frame update
    void Start()
    {
		possibleCrafts = new List<CraftableObject>();
		craftables = new CraftableObject[] {chainArmor, leatherArmor, metalArmor, energyDrink, medpack, arGun, metalRod, woodenBat};
		controller = GameObject.FindGameObjectsWithTag("GameController")[0];
    }

	public void GenerateListOfCraftables()
	{
		Start();
		foreach (GameObject g in cItems) {
			Destroy(g);
		}
		cItems.Clear();
		HashSet<Resource> resourceSet = controller.GetComponent<EquipmentManager>().getResourceSet();

		foreach (CraftableObject craftable in craftables) {
			CraftingRecipe cr = craftable.recipe;

			bool isCraftable = true;
			foreach (ResourceAmount r in cr.resources) {
				if (!resourceSet.Contains(r.item) || getCount(r.item) < r.amount) {
					isCraftable = false;
				}
			}

			if (isCraftable) {
				possibleCrafts.Add(craftable);
			}
		}

		foreach (CraftableObject co in possibleCrafts) {
			GameObject temp = Instantiate(craftablePrefab) as GameObject;
			temp.GetComponent<CraftableItemLoader>().setCraftableObject(co);
			temp.transform.parent = container.transform;
			temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			cItems.Add(temp);
		}
	}

	private int getCount(Resource re)
	{
		List<ResourcePersistent> currentResources = controller.GetComponent<EquipmentManager>().getResources();
		foreach (ResourcePersistent rp in currentResources) {
			if (rp.Resource == re) {
				return rp.Count;
			}
		}
		return -1;
	}

	public void Craft(CraftableObject c) {
		foreach (ResourceAmount r in c.recipe.resources) {
			controller.GetComponent<EquipmentManager>().RemoveResource(r.item, r.amount);
			controller.GetComponent<InventoryManager>().resourceCounts[r.item.id] -= r.amount;
		}
		if (c is Item)
		{
			controller.GetComponent<InventoryManager>().itemCounts[c.id]++;
		}
		else if (c is Weapon)
		{
			controller.GetComponent<InventoryManager>().weaponCounts[c.id]++;
		}
		else if (c is Armor)
		{
			controller.GetComponent<InventoryManager>().armorCounts[c.id]++;
		}
	}

}
