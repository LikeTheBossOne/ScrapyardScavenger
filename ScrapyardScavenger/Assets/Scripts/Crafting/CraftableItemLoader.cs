using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftableItemLoader : MonoBehaviour
{
	private CraftableObject craft;
	public Text craftItemText;
	public Image craftItemImage;
	public Image craftReq1Image;
	public Text craftReq1Count;
	public Image craftReq2Image;
	public Text craftReq2Count;
	public Image craftReq3Image;
	public Text craftReq3Count;

	private bool loaded;

	// Start is called before the first frame update
    void Start()	// Probably change to onEnabled if more than 1 object doesn't load
    {
		loaded = false;
    }

	public void setCraftableObject(CraftableObject c)
	{
		craft = c;
	}

    // Update is called once per frame
    void Update()
    {
		if (!loaded && craft != null) {
			craftItemText.text = craft.name;
			craftItemImage.sprite = craft.icon;

			loaded = true;
		}
    }

	public void Craft()
	{
		GameObject generator = GameObject.FindGameObjectsWithTag("crafter")[0];
		generator.GetComponent<CraftingUIGenerator>().Craft(craft);
		generator.GetComponent<CraftingUIGenerator>().GenerateListOfCraftables();
	}
}
