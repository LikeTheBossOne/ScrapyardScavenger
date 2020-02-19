using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class PlayerHUD : MonoBehaviourPunCallbacks
{
	#region Private Fields


	[Tooltip("UI Slider to display Player's Health")]
	[SerializeField]
	private Slider playerHealthSlider;



	#endregion

	void Start() {
		playerHealthSlider = GameObject.FindWithTag("Health").GetComponent<Slider>();

		// The photon view is mine check is necessary here, otherwise everyone's health bar will be reset
		if (!photonView.IsMine) return;
		playerHealthSlider.value = 100;
	}

	void Update()
	{
		// If not me, don't update!
		if (!photonView.IsMine) return;

	}

	#region Public Methods

	public Slider getHealthSlider() {
		return this.playerHealthSlider;
	}

	public void takeDamage(float dmg) {
		if (getHealthSlider().value > 0)
			playerHealthSlider.value -= dmg;
	}

	public void heal(float healAmt) {
		if (photonView.IsMine)
		{
			playerHealthSlider.value += healAmt;
		}
	}



	#endregion
}
