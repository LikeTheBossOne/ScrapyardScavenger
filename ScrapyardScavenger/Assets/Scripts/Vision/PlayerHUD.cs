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
		playerHealthSlider.value = 100;
	}

	void Update()
	{
		// If not me, don't update!
		if (!photonView.IsMine) return;

		// Example of health changes; remove later
		if (getHealthSlider().value > 0)
			takeDamage(0.1f);

	}

	#region Public Methods

	public Slider getHealthSlider() {
		return this.playerHealthSlider;
	}

	public void takeDamage(float dmg) {
		playerHealthSlider.value -= dmg;
	}

	public void heal(float healAmt) {
		playerHealthSlider.value += healAmt;
	}



	#endregion
}
