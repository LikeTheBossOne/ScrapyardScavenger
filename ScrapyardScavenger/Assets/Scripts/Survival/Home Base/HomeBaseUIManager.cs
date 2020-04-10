using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeBaseUIManager : MonoBehaviour
{
	public GameObject homeBaseCanvas;
	public GameObject storageCanvas;
	public GameObject equipmentCanvas;
	public GameObject craftingCanvas;
	public GameObject skillCanvas;
	public GameObject equipPopup;
	public GameObject generator;

	public int playerCount;

	public void switchToHomeBase() {
		if (!equipPopup.activeSelf) {
			homeBaseCanvas.SetActive(true);
			storageCanvas.SetActive(false);
			equipmentCanvas.SetActive(false);
			craftingCanvas.SetActive(false);
			skillCanvas.SetActive(false);
		}
	}

	public void switchToStorage() {
		homeBaseCanvas.SetActive(false);
		storageCanvas.SetActive(true);
		equipmentCanvas.SetActive(false);
		craftingCanvas.SetActive(false);
		skillCanvas.SetActive(false);
    
		GameObject img = GameObject.FindGameObjectWithTag("StorageItemImg");
		GameObject name = GameObject.FindGameObjectWithTag("StorageItemName");
		GameObject desc = GameObject.FindGameObjectWithTag("StorageItemDesc");
		img.GetComponent<Image>().sprite = null;
		name.GetComponent<Text>().text = "";
		desc.GetComponent<Text>().text = "";
	}

	public void switchToCrafting() {
		homeBaseCanvas.SetActive(false);
		storageCanvas.SetActive(false);
		equipmentCanvas.SetActive(false);
		craftingCanvas.SetActive(true);
		skillCanvas.SetActive(false);
	}

	public void switchToEquipment() {
		homeBaseCanvas.SetActive(false);
		storageCanvas.SetActive(false);
		equipmentCanvas.SetActive(true);
		craftingCanvas.SetActive(false);
		skillCanvas.SetActive(false);
	}

	public void switchToSkills() {
		homeBaseCanvas.SetActive(false);
		storageCanvas.SetActive(false);
		equipmentCanvas.SetActive(false);
		craftingCanvas.SetActive(false);
		skillCanvas.SetActive(true);
	}

	public void OpenEquipUI(int slotIdx)
	{
		if (!equipPopup.activeSelf) {
			equipPopup.SetActive(true);
			generator.GetComponent<EquipPopupGenerator>().GenerateEquipment(slotIdx);
		}
	}

	public void CloseEquipUI()
	{
		equipPopup.SetActive(false);
	}

	public void quitGame() {
		// save any game data here
		#if UNITY_EDITOR
			// Application.Quit() does not work in the editor so
			// UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}

}
