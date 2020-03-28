using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DisplayItemInfo : MonoBehaviour
{
	public GameObject pController;
	private Item item;
	public Text itemText;
	public Image itemImage;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (pController != null) {
			item = (Item)pController.GetComponent<BaseDataManager>().getEquipment()[4];
			if (item != null)
			{
				itemText.text = item.name;
				itemImage.sprite = item.icon;
			} else {
				itemText.text = "Item";
				itemImage.sprite = null;
			}

			if (itemImage.sprite != null) {
				Color slotColor = itemImage.color;
				slotColor.a = 1.0f;
				itemImage.color = slotColor;
			} else {
				Color slotColor = itemImage.color;
				slotColor.a = 0.0f;
				itemImage.color = slotColor;
			}
		}
	}

	void OnEnable()
	{
		if (pController == null)
		{
			foreach (GameObject obj in GameObject.FindGameObjectsWithTag("GameController"))
			{
				if (obj.GetPhotonView().IsMine)
				{
					pController = obj;
					break;
				}
			}
		}
	}
}
