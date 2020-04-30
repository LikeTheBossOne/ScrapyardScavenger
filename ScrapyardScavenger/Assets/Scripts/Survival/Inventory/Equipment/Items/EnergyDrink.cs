using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/**
 * Used by the player to increase defense temporarily
 */
[CreateAssetMenu(fileName = "New Item", menuName = "Items/EnergyDrink")]
public class EnergyDrink : Item
{
    public int seconds;

    public override void Use(InGameDataManager manager)
    {
		InGamePlayerManager pManager = FindObjectOfType<InGamePlayerManager>();
		GameObject myPlayer = null;
		foreach (var player in pManager.players)
		{
			if (player.GetPhotonView().IsMine)
			{
				myPlayer = player;
			}
		}
		// use energy drink
        myPlayer.GetComponent<PlayerMotor>().Energize(seconds);
		NotificationSystem.Instance.Notify(new Notification("Used Energy Drink", NotificationType.Good));

        // remove this from the manager?
		manager.currentItem = null;
    }
}
