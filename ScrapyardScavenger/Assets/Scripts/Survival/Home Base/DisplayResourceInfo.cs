using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class DisplayResourceInfo : MonoBehaviour
{
	public GameObject controller;
	public Image image;
	public Text countText;
	public int slotIndex;

	private ResourcePersistent rp;

    // Start is called before the first frame update
    void Start()
    {
		this.GetComponent<Button>().onClick.AddListener(DisplayInfoOnStorage);
    }

    // Update is called once per frame
    void Update()
    {
		if (controller != null) {
			try {
				rp = controller.GetComponent<BaseDataManager>().getResources()[slotIndex];
			} catch (Exception e) {
				// The slot has nothing in it yet
				rp = null;
			}
		}
		if (rp != null) {
			image.sprite = rp.Resource.icon;
			countText.text = rp.Count.ToString();
		} else {
			image.sprite = null;
			countText.text = "";
		}
		if (image.sprite != null) {
			Color slotColor = image.color;
			slotColor.a = 1.0f;
			image.color = slotColor;
		} else {
			Color slotColor = image.color;
			slotColor.a = 0.0f;
			image.color = slotColor;
		}
    }

	void OnEnable() {
		if (controller == null)
		{
			foreach (GameObject obj in GameObject.FindGameObjectsWithTag("GameController"))
			{
				if (obj.GetPhotonView().IsMine)
				{
					controller = obj;
					break;
				}
			}
		}
	}

	void DisplayInfoOnStorage() {
		GameObject img = GameObject.FindGameObjectWithTag("StorageItemImg");
		GameObject name = GameObject.FindGameObjectWithTag("StorageItemName");
		GameObject desc = GameObject.FindGameObjectWithTag("StorageItemDesc");

		if (img != null && image.sprite != null) {
			img.GetComponent<Image>().sprite = image.sprite;
			name.GetComponent<Text>().text = rp.Resource.name;
			desc.GetComponent<Text>().text = rp.Resource.description;
		}
	}
}
