using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayWeaponInfo : MonoBehaviour
{
	public GameObject pController;
	private Weapon weapon;
	public int weaponIndex;
	public Text weaponText;
	public Image weaponImage;

    // Start is called before the first frame update
    void Start()
    {
		weapon = (Weapon) pController.GetComponent<EquipmentManager>().getEquipment()[weaponIndex];
    }

    // Update is called once per frame
    void Update()
    {
		weaponText.text = weapon.name;
		weaponImage.sprite = weapon.icon;
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
