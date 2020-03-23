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
	void Start()
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

			switch(craft.recipe.resources.Count){
				case 3:
					craftReq3Image.sprite = craft.recipe.resources[2].item.icon;
					craftReq3Count.text = craft.recipe.resources[2].amount.ToString();
					goto case 2;	// Idk why C# can't allow for cascading cases like in any other language
				case 2:
					craftReq2Image.sprite = craft.recipe.resources[1].item.icon;
					craftReq2Count.text = craft.recipe.resources[1].amount.ToString();
					goto case 1;
				case 1:
					craftReq1Image.sprite = craft.recipe.resources[0].item.icon;
					craftReq1Count.text = craft.recipe.resources[0].amount.ToString();
					break;		// C# switch statements are dumb; apparently not having this line breaks the code
			}

			RenderImage(craftReq1Image);
			RenderImage(craftReq2Image);
			RenderImage(craftReq3Image);

			loaded = true;
		}
    }

	public void Craft()
	{
		GameObject generator = GameObject.FindGameObjectsWithTag("crafter")[0];
		generator.GetComponent<CraftingUIGenerator>().Craft(craft);
		generator.GetComponent<CraftingUIGenerator>().GenerateListOfCraftables();
	}

	private void RenderImage(Image i)
	{
		if (i.sprite == null) {
			Color slotColor = i.color;
			slotColor.a = 0.0f;
			i.color = slotColor;
		} else {
			Color slotColor = i.color;
			slotColor.a = 1.0f;
			i.color = slotColor;
		}
	}
}
