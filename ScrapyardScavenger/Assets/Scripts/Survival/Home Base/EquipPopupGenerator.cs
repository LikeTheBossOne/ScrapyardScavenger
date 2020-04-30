using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class EquipPopupGenerator : MonoBehaviour
{
	private GameObject pController;
	private int idxFocus;
	private List<GameObject> generated;
	private CraftableObject co;

	public GameObject container;
	public GameObject equipPrefab;
	public Image eImage;
	public Text eName;
	public Text eDesc;
	public Text effect1;
	public Text effect2;
	public GameObject unEqButton;
	public GameObject noneText;

	public GameObject padListener;

	// Start is called before the first frame update
    void Start()
    {
		
    }

	void Update()
	{
		if (pController != null && idxFocus >= 0 && idxFocus < 5) {
			co = pController.GetComponent<BaseDataManager>().equipment[idxFocus];
			if(co != null) {
				eImage.sprite = co.icon;
				Color slotColor = eImage.color;
				slotColor.a = 1.0f;
				eImage.color = slotColor;
				eName.text = co.name;
				eDesc.text = co.description;
				switch (idxFocus) {
					case 0:
					goto case 1;
					case 1:
					effect1.text = "Power: " + (co as Gun).baseDamage.ToString();
					effect2.text = "Ammo: " + (co as Gun).baseClipSize.ToString();
					break;
					case 3:
					effect1.text = "Power: " + (co as Grenade).baseDamage.ToString();
					effect2.text = "Boom Time: " + (co as Grenade).baseDetonationTime.ToString() + " sec";
					break;
					case 2:
					effect1.text = "Power: " + (co as Melee).baseDamage.ToString();
					effect2.text = "";
					break;
					default:
					break; // switch statements in C# are stupid
				}
				unEqButton.SetActive(true);
			} else {
				eImage.sprite = null;
				Color slotColor = eImage.color;
				slotColor.a = 0.0f;
				eImage.color = slotColor;
				eName.text = "Nothing equipped";
				eDesc.text = "";
				effect1.text = "";
				effect2.text = "";
				unEqButton.SetActive(false);
			}
		} else if (pController != null && idxFocus == 5) {
			co = null;
			Armor a = pController.GetComponent<BaseDataManager>().equippedArmor;
			if(a != null) {
				eImage.sprite = a.icon;
				Color slotColor = eImage.color;
				slotColor.a = 1.0f;
				eImage.color = slotColor;
				eName.text = a.name;
				eDesc.text = a.description;
				effect1.text = "";
				effect2.text = "";
				unEqButton.SetActive(true);
			} else {
				eImage.sprite = null;
				Color slotColor = eImage.color;
				slotColor.a = 0.0f;
				eImage.color = slotColor;
				eName.text = "Nothing equipped";
				eDesc.text = "";
				effect1.text = "";
				effect2.text = "";
				unEqButton.SetActive(false);
			}
		}
	}

	public void GenerateEquipment(int option)
	{
		noneText.SetActive(false);
		idxFocus = option;
		if (generated == null) {
			generated = new List<GameObject>();
		}
		foreach (GameObject g in generated) {
			Destroy(g);
		}
		generated.Clear();
		int[] counts = null;
		switch(option){
			case 0:
			goto case 1;
			break;
			case 1:
			List<Gun> equippableGuns = new List<Gun>();
			counts = pController.GetComponent<BaseDataManager>().weaponCounts;
			for (int i = 0; i < counts.Length; i++) {
				if (counts[i] > 0 && pController.GetComponent<BaseDataManager>().weapons[i] is Gun) {
					equippableGuns.Add(pController.GetComponent<BaseDataManager>().weapons[i] as Gun);
				}
			}
			if (equippableGuns.Count > 0) {
				foreach (Gun gun in equippableGuns) {
					GameObject temp = Instantiate(equipPrefab) as GameObject;
					temp.GetComponent<EquipLoader>().img.sprite = gun.icon;
					temp.GetComponent<EquipLoader>().name.text = gun.name;
					temp.GetComponent<EquipLoader>().desc.text = gun.description;
					temp.GetComponent<EquipLoader>().effect1Text.text = "Power: " + gun.baseDamage.ToString();
					temp.GetComponent<EquipLoader>().effect2Text.text = "Ammo: " + gun.baseClipSize.ToString();
					temp.GetComponent<EquipLoader>().count.text = "x" + counts[gun.id].ToString();
					temp.GetComponent<Button>().onClick.AddListener(() => EquipToPlayer(gun));
					temp.transform.parent = container.transform;
					temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
					generated.Add(temp);
				}
			} else {
				noneText.SetActive(true);
				noneText.GetComponent<Text>().text = "No guns available";
			}
			break;
			case 3:
			List<Grenade> equippableGrenades = new List<Grenade>();
			counts = pController.GetComponent<BaseDataManager>().weaponCounts;
			for (int i = 0; i < counts.Length; i++) {
				if (counts[i] > 0 && pController.GetComponent<BaseDataManager>().weapons[i] is Grenade) {
					equippableGrenades.Add(pController.GetComponent<BaseDataManager>().weapons[i] as Grenade);
				}
			}
			if (equippableGrenades.Count > 0) {
				foreach (Grenade gre in equippableGrenades) {
					GameObject temp = Instantiate(equipPrefab) as GameObject;
					temp.GetComponent<EquipLoader>().img.sprite = gre.icon;
					temp.GetComponent<EquipLoader>().name.text = gre.name;
					temp.GetComponent<EquipLoader>().desc.text = gre.description;
					temp.GetComponent<EquipLoader>().effect1Text.text = "Power: " + gre.baseDamage.ToString();
					temp.GetComponent<EquipLoader>().effect2Text.text = "Boom Time: " + gre.baseDetonationTime.ToString();
					temp.GetComponent<EquipLoader>().count.text = "x" + counts[gre.id].ToString();
					temp.GetComponent<Button>().onClick.AddListener(() => EquipToPlayer(gre));
					temp.transform.parent = container.transform;
					temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
					generated.Add(temp);
				}
			} else {
				noneText.SetActive(true);
				noneText.GetComponent<Text>().text = "No grenades available";
			}
			break;
			case 2:
			List<Melee> equippableMelees = new List<Melee>();
			counts = pController.GetComponent<BaseDataManager>().weaponCounts;
			for (int i = 0; i < counts.Length; i++) {
				if (counts[i] > 0 && pController.GetComponent<BaseDataManager>().weapons[i] is Melee) {
					equippableMelees.Add(pController.GetComponent<BaseDataManager>().weapons[i] as Melee);
				}
			}
			if (equippableMelees.Count > 0) {
				foreach (Melee mel in equippableMelees) {
					GameObject temp = Instantiate(equipPrefab) as GameObject;
					temp.GetComponent<EquipLoader>().img.sprite = mel.icon;
					temp.GetComponent<EquipLoader>().name.text = mel.name;
					temp.GetComponent<EquipLoader>().desc.text = mel.description;
					temp.GetComponent<EquipLoader>().effect1Text.text = "Power: " + mel.baseDamage.ToString();
					temp.GetComponent<EquipLoader>().effect2Text.text = "";
					temp.GetComponent<EquipLoader>().count.text = "x" + counts[mel.id].ToString();
					temp.GetComponent<Button>().onClick.AddListener(() => EquipToPlayer(mel));
					temp.transform.parent = container.transform;
					temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
					generated.Add(temp);
				}
			} else {
				noneText.SetActive(true);
				noneText.GetComponent<Text>().text = "No melee weapons available";
			}
			break;
			case 4:
			List<Item> equippableItems = new List<Item>();
			counts = pController.GetComponent<BaseDataManager>().itemCounts;
			for (int i = 0; i < counts.Length; i++) {
				if (counts[i] > 0) {
					equippableItems.Add(pController.GetComponent<BaseDataManager>().items[i]);
				}
			}
			if (equippableItems.Count > 0) {
				foreach (Item it in equippableItems) {
					GameObject temp = Instantiate(equipPrefab) as GameObject;
					temp.GetComponent<EquipLoader>().img.sprite = it.icon;
					temp.GetComponent<EquipLoader>().name.text = it.name;
					temp.GetComponent<EquipLoader>().desc.text = it.description;
					temp.GetComponent<EquipLoader>().effect1Text.text = "";
					temp.GetComponent<EquipLoader>().effect2Text.text = "";
					temp.GetComponent<EquipLoader>().count.text = "x" + counts[it.id].ToString();
					temp.GetComponent<Button>().onClick.AddListener(() => EquipToPlayer(it));
					temp.transform.parent = container.transform;
					temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
					generated.Add(temp);
				}
			} else {
				noneText.SetActive(true);
				noneText.GetComponent<Text>().text = "No items available";
			}
			break;
			case 5:
			List<Armor> equippableArmors = new List<Armor>();
			counts = pController.GetComponent<BaseDataManager>().armorCounts;
			for (int i = 0; i < counts.Length; i++) {
				if (counts[i] > 0) {
					equippableArmors.Add(pController.GetComponent<BaseDataManager>().armors[i]);
				}
			}
			if (equippableArmors.Count > 0) {
				foreach (Armor a in equippableArmors) {
					GameObject temp = Instantiate(equipPrefab) as GameObject;
					temp.GetComponent<EquipLoader>().img.sprite = a.icon;
					temp.GetComponent<EquipLoader>().name.text = a.name;
					temp.GetComponent<EquipLoader>().desc.text = a.description;
					temp.GetComponent<EquipLoader>().effect1Text.text = "";
					temp.GetComponent<EquipLoader>().effect2Text.text = "";
					temp.GetComponent<EquipLoader>().count.text = "x" + counts[a.id].ToString();
					temp.GetComponent<Button>().onClick.AddListener(() => EquipToPlayer(a));
					temp.transform.parent = container.transform;
					temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
					generated.Add(temp);
				}
			} else {
				noneText.SetActive(true);
				noneText.GetComponent<Text>().text = "No armor available";
			}
			break;
		}
		padListener.GetComponent<GamepadListener>().SetValidButtons(generated, 1);
	}

	public void Unequip()
	{
		if (co is Equipment) {
			Equipment[] currEquipment = pController.GetComponent<BaseDataManager>().equipment;
			int idxToRemove = Array.IndexOf(currEquipment, co);
			currEquipment[idxToRemove] = null;
		} else {
			pController.GetComponent<BaseDataManager>().equippedArmor = null;
		}
	}

	private void EquipToPlayer(CraftableObject c)
	{
		if (c is Equipment) {
			Equipment[] currEquipment = pController.GetComponent<BaseDataManager>().equipment;
			if (Array.Exists(currEquipment, element => element == c)) {
				int idxToRemove = Array.IndexOf(currEquipment, c);
				currEquipment[idxToRemove] = null;
			}
			currEquipment[idxFocus] = c as Equipment;
		} else {
			pController.GetComponent<BaseDataManager>().equippedArmor = c as Armor;
		}
	}

	private GameObject getController() {
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("GameController"))
		{
			if (obj.GetPhotonView().IsMine)
			{
				return obj;
			}
		}
		return null;
	}

	void OnEnable(){
		pController = getController();
	}

}
