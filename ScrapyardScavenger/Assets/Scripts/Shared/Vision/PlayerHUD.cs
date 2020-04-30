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

    [Tooltip("UI Text to display Player's Ammo Count")]
    [SerializeField]
	private Text playerAmmoCount;

	[Tooltip("UI Text to display Crosshair")]
	[SerializeField]
	private Text playerCrosshair;

	[Tooltip("UI Slider to display TRUCK Health")]
	[SerializeField]
	private Slider truckHealthSlider;

	private Text WaveCounter;

	private Coroutine hitMarkerCoroutine;

    #endregion

	void Start() {
		playerHealthSlider = GameObject.FindWithTag("Health").GetComponent<Slider>();
        playerAmmoCount = GameObject.FindWithTag("AmmoCount").GetComponent<Text>();
		playerCrosshair = GameObject.FindWithTag("crosshair").GetComponent<Text>();
		truckHealthSlider = GameObject.FindWithTag("TruckHealth").GetComponent<Slider>();
		WaveCounter = GameObject.Find("WaveCounter").GetComponent<Text>();

		// The photon view is mine check is necessary here, otherwise everyone's health bar will be reset
		if (!photonView.IsMine) return;
		playerHealthSlider.value = 100;
		truckHealthSlider.value = 750;

        Gun startGun = GetComponent<PlayerControllerLoader>().inGameDataManager.getCurrentEquipment() as Gun;
        if (startGun != null)
        {
            AmmoChanged(startGun.baseClipSize, startGun.baseClipSize);
		}
    }

	void Update()
	{
		// If not me, don't update!
		if (!photonView.IsMine) return;
    }

	IEnumerator HitMarker()
	{
		playerCrosshair.fontSize = 40;
		yield return new WaitForSeconds(0.3f); 
		playerCrosshair.fontSize = 24;
	}

	#region Public Methods

	public Slider getHealthSlider() {
		return this.playerHealthSlider;
	}

	public void takeDamage(float dmg) {
		if (getHealthSlider().value > 0)
			playerHealthSlider.value -= dmg;
	}

	//Get's the Truck health slider
	public Slider getTruckHealthSlider()
	{
		return this.truckHealthSlider;
	}

	//The truck health bar updates
	[PunRPC]
	public void TruckTakeDamage(float dmg)
	{
		if (photonView.IsMine)
        {
			if (getTruckHealthSlider().value > 0)
				truckHealthSlider.value -= dmg;
		}
		
	}

	public void heal(float healAmt) {
		if (photonView.IsMine)
		{
			playerHealthSlider.value += healAmt;
		}
	}

    public void AmmoChanged(int ammoCount, int maxAmmo)
    {
        if (photonView.IsMine)
        {
            playerAmmoCount.text = $"Ammo: {ammoCount}/{maxAmmo}";
		}
    }

	public void crossHairReloading()
	{
		if (photonView.IsMine)
			playerCrosshair.text = "";
	}

	public void crossHairReloaded()
	{
		if (photonView.IsMine)
			playerCrosshair.text = "+";
	}

	public void hitCrossHair()
	{
		if (photonView.IsMine)
			hitMarkerCoroutine = StartCoroutine(HitMarker());
	}

	[PunRPC]
	public void UpdateWaveInUI(int wave)
    {
		if (photonView.IsMine)
        {
			StartCoroutine(FadeOutRoutine(wave));
			//WaveCounter.text = "" + wave;
        }
    }

	private IEnumerator FadeOutRoutine(int wave)
	{
		//Text text = GetComponent<Text>();
		Color originalColor = WaveCounter.color;
		float fadeOutTime = 1.5f;
		for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime)
		{
			WaveCounter.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t / fadeOutTime));
			yield return null;
		}
		WaveCounter.text = "" + wave;
		NotificationSystem.Instance.Notify(new Notification($"Wave {wave} has started!", NotificationType.Neutral));
		StartCoroutine(FadeInRoutine(originalColor));
	}

	private IEnumerator FadeInRoutine(Color visibleColor)
	{
		//Text text = GetComponent<Text>();
		Color clearColor = WaveCounter.color;
		float fadeInTime = 1.5f;
		for (float t = 0.01f; t < fadeInTime; t += Time.deltaTime)
		{
			WaveCounter.color = Color.Lerp(clearColor, visibleColor, Mathf.Min(1, t / fadeInTime));
			yield return null;
		}
	}

	#endregion
}
