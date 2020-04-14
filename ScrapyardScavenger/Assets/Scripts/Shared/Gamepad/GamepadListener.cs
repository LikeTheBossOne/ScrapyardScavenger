using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GamepadListener : MonoBehaviour
{
	[SerializeField]
	public GameObject[] validButtons;
	public GameObject backButton;
	public GameObject specialButton;
	public int rowValue;

	public ScrollRect scrollRect;
	public RectTransform contentPanel;

	private GameObject currentButton;
	private int selectIndex = 0;

	private float delay = 0.0f;


	// Start is called before the first frame update
    void Start()
    {
		ChangeCurrentButton(validButtons[selectIndex]);
    }

    // Update is called once per frame
    void Update()
    {
		if (gameObject.activeSelf && Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0] != "") {
			if (currentButton == null) {
				ChangeCurrentButton(validButtons[selectIndex]);
			}
			if (Input.GetKeyDown("joystick button 0")) {
				if (currentButton.GetComponent<Button>().interactable) {
					currentButton.GetComponent<Button>().onClick.Invoke();
				}
			} else if (Input.GetKeyDown("joystick button 1")) {
				backButton.GetComponent<Button>().onClick.Invoke();
			} 
			if (Input.GetKeyDown("joystick button 3")) {
				specialButton.GetComponent<Button>().onClick.Invoke();
			}
			if (Math.Abs(Input.GetAxis("Horizontal")) < 0.1 && Math.Abs(Input.GetAxis("Vertical")) < 0.1) {
				delay = 0.0f;
			}

			if (Input.GetAxis("Horizontal") == 1) {
				changeSelectedIndex(1);
			}
			if (Input.GetAxis("Horizontal") == -1) {
				changeSelectedIndex(-1);
			}
			if (Input.GetAxis("Vertical") == 1) {
				changeSelectedIndex(-1 * rowValue);
			}
			if (Input.GetAxis("Vertical") == -1) {
				changeSelectedIndex(rowValue);
			}
			ChangeCurrentButton(validButtons[selectIndex]);
		}
		if ((Input.GetJoystickNames().Length == 0 || Input.GetJoystickNames()[0] == "") && currentButton != null) {
			Destroy(currentButton.GetComponent<UnityEngine.UI.Outline>());
			currentButton = null;
		}
    }

	public void ChangeCurrentButton(GameObject newButton) {
		if (newButton == null || newButton == currentButton || Input.GetJoystickNames().Length == 0) return;
		if (currentButton != null) {
			Destroy(currentButton.GetComponent<UnityEngine.UI.Outline>());
		}
		currentButton = newButton;
		currentButton.AddComponent<UnityEngine.UI.Outline>();
		currentButton.GetComponent<UnityEngine.UI.Outline>().effectDistance = new Vector2(2, -2);
		currentButton.GetComponent<UnityEngine.UI.Outline>().effectColor = new Color(255, 255, 0);

		if (scrollRect && contentPanel) {
			RectTransform target = newButton.GetComponent<RectTransform>();
			Vector2 objPosition = (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
			float scrollHeight = scrollHeight = scrollRect.GetComponent<RectTransform>().rect.height;
			float objHeight = target.rect.height;
			if (target.position.y != 0.0f) {
				if (objPosition.y > scrollHeight / 2)
				{
					contentPanel.localPosition = new Vector2(contentPanel.localPosition.x,
						contentPanel.localPosition.y - objHeight);
				}
				if (objPosition.y < -scrollHeight / 2)
				{
					contentPanel.localPosition = new Vector2(contentPanel.localPosition.x,
						contentPanel.localPosition.y + objHeight);
				}
			}
		}
	}

	private void changeSelectedIndex (int offset) {
		if (Time.time > delay) {
			delay = Time.time + 0.3f;
			int old = selectIndex;
			selectIndex += offset;
			try {
				GameObject temp = validButtons[selectIndex];
			} catch (Exception e) {
				selectIndex = old;
			}
		}
	}

	public void SetValidButtons(List<GameObject> objList, int rValue = 0) {
		validButtons = new GameObject[objList.Count];
		for (int i = 0; i < objList.Count; i++) {
			validButtons[i] = objList[i];
		}
		rowValue = rValue;
		Start();
	}
}
