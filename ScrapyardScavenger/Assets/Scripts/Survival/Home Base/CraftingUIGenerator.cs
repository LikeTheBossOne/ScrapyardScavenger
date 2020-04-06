using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUIGenerator : MonoBehaviour
{
	private List<CraftableObject> possibleCrafts;
	private List<CraftableObject> impossibleCrafts;
	public GameObject controller;
	public GameObject craftablePrefab;
	public GameObject container;
	public GameObject padListener;
	private List<GameObject> cItems = new List<GameObject>();

	[SerializeField]
	public CraftableObject[] craftables;

	// Start is called before the first frame update
    void Start()
    {
		possibleCrafts = new List<CraftableObject>();
		impossibleCrafts = new List<CraftableObject>();
		controller = GameObject.FindGameObjectsWithTag("GameController")[0];
    }

	public void GenerateListOfCraftables()
	{
		Start();
		foreach (GameObject g in cItems) {
			Destroy(g);
		}
		cItems.Clear();
		HashSet<Resource> resourceSet = controller.GetComponent<BaseDataManager>().getResourceSet();

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
			} else {
				impossibleCrafts.Add(craftable);
			}
		}

		foreach (CraftableObject co in possibleCrafts) {
			GameObject temp = Instantiate(craftablePrefab) as GameObject;
			temp.GetComponent<CraftableItemLoader>().setCraftableObject(co);
			temp.transform.parent = container.transform;
			temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			cItems.Add(temp);
		}
		foreach (CraftableObject co in impossibleCrafts) {
			GameObject temp = Instantiate(craftablePrefab) as GameObject;
			temp.GetComponent<Button>().interactable = false;
			temp.GetComponent<CraftableItemLoader>().setCraftableObject(co);
			temp.transform.parent = container.transform;
			temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			cItems.Add(temp);
		}
		padListener.GetComponent<GamepadListener>().SetValidButtons(cItems, 1);
	}

	private int getCount(Resource re)
	{
		List<ResourcePersistent> currentResources = controller.GetComponent<BaseDataManager>().getResources();
		foreach (ResourcePersistent rp in currentResources) {
			if (rp.Resource == re) {
				return rp.Count;
			}
		}
		return -1;
	}

	public void Craft(CraftableObject c) {
		/*
		HashSet<Resource> resourceSet = controller.GetComponent<BaseDataManager>().getResourceSet();
		foreach (ResourceAmount r in c.recipe.resources) {
			if (!resourceSet.Contains(r.item) || getCount(r.item) < r.amount) {
				return;
			}
		}
		*/

		foreach (ResourceAmount r in c.recipe.resources) {
			controller.GetComponent<BaseDataManager>().RemoveResourceFromStorage(r.item, r.amount);
		}
		if (c is Item)
		{
			controller.GetComponent<BaseDataManager>().itemCounts[c.id]++;
		}
		else if (c is Weapon)
		{
			controller.GetComponent<BaseDataManager>().weaponCounts[c.id]++;
		}
		else if (c is Armor)
		{
			controller.GetComponent<BaseDataManager>().armorCounts[c.id]++;
		}
	}

}
