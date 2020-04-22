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
	public GameObject controlsCanvas;
	public GameObject controlScreenCanvas;
	public GameObject tutorialScreenCanvas;
	public GameObject equipPopup;
	public GameObject generator;
	public GameObject eqScreenListener;

	public int playerCount;

	public void switchToHomeBase() {
		if (!equipPopup.activeSelf) {
			homeBaseCanvas.SetActive(true);
			storageCanvas.SetActive(false);
			equipmentCanvas.SetActive(false);
			craftingCanvas.SetActive(false);
			skillCanvas.SetActive(false);
			controlsCanvas.SetActive(false);
		}
	}

	public void switchToStorage() {
		homeBaseCanvas.SetActive(false);
		storageCanvas.SetActive(true);
		equipmentCanvas.SetActive(false);
		craftingCanvas.SetActive(false);
		skillCanvas.SetActive(false);
		controlsCanvas.SetActive(false);
    
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
		controlsCanvas.SetActive(false);
	}

	public void switchToEquipment() {
		homeBaseCanvas.SetActive(false);
		storageCanvas.SetActive(false);
		equipmentCanvas.SetActive(true);
		craftingCanvas.SetActive(false);
		skillCanvas.SetActive(false);
		controlsCanvas.SetActive(false);
	}

	public void switchToSkills() {
		homeBaseCanvas.SetActive(false);
		storageCanvas.SetActive(false);
		equipmentCanvas.SetActive(false);
		craftingCanvas.SetActive(false);
		skillCanvas.SetActive(true);
		controlsCanvas.SetActive(false);
	}

	public void switchToTutorial() {
		homeBaseCanvas.SetActive(false);
		storageCanvas.SetActive(false);
		equipmentCanvas.SetActive(false);
		craftingCanvas.SetActive(false);
		skillCanvas.SetActive(false);
		controlsCanvas.SetActive(true);
		SwitchToControlScreen();
	}

	public void OpenEquipUI(int slotIdx)
	{
		if (!equipPopup.activeSelf) {
			equipPopup.SetActive(true);
			eqScreenListener.SetActive(false);
			generator.GetComponent<EquipPopupGenerator>().GenerateEquipment(slotIdx);
		}
	}

	public void CloseEquipUI()
	{
		equipPopup.SetActive(false);
		eqScreenListener.SetActive(true);
	}

	public void SwitchToControlScreen() {
		controlScreenCanvas.SetActive(true);
		tutorialScreenCanvas.SetActive(false);
	}

	public void SwitchToTutorialScreen() {
		controlScreenCanvas.SetActive(false);
		tutorialScreenCanvas.SetActive(true);
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
