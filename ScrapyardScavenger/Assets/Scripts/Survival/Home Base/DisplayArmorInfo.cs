using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DisplayArmorInfo : MonoBehaviour
{
	public GameObject pController;
	private Armor armor;
	public Text armorText;
	public Image armorImage;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (pController != null) {
			armor = (Armor)pController.GetComponent<BaseDataManager>().equippedArmor;
			if (armor != null)
			{
				armorText.text = armor.name;
				armorImage.sprite = armor.icon;
			} else {
				armorText.text = "Armor";
				armorImage.sprite = null;
			}

			if (armorImage.sprite != null) {
				Color slotColor = armorImage.color;
				slotColor.a = 1.0f;
				armorImage.color = slotColor;
			} else {
				Color slotColor = armorImage.color;
				slotColor.a = 0.0f;
				armorImage.color = slotColor;
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
