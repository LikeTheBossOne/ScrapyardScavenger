using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class DisplayWeaponInfo : MonoBehaviour
{
	public GameObject pController;
	public FaceoffInGameData faceoffData;

	public Weapon weapon;
	public int weaponIndex;
	public Text weaponText;
	public Image weaponImage;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (pController != null) {
			weapon = (Weapon)pController.GetComponent<BaseDataManager>().getEquipment()[weaponIndex];
			if (weapon != null)
			{
				weaponText.text = weapon.name;
				weaponImage.sprite = weapon.icon;
			} else {
				switch (weaponIndex) {
				case 0:
					weaponText.text = "Weapon 1";
					break;
				case 1:
					weaponText.text = "Weapon 2";
					break;
				case 2:
					weaponText.text = "Melee";
					break;
				case 3:
					weaponText.text = "Throwable";
					break;
				}
				weaponImage.sprite = null;
			}

			if (weaponImage.sprite != null) {
				Color slotColor = weaponImage.color;
				slotColor.a = 1.0f;
				weaponImage.color = slotColor;
			} else {
				Color slotColor = weaponImage.color;
				slotColor.a = 0.0f;
				weaponImage.color = slotColor;
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
