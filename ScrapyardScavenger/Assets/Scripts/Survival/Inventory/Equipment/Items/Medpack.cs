using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/**
 * Used by the player to recover health
 */
[CreateAssetMenu(fileName = "New Item", menuName = "Items/Medpack")]
public class Medpack : Item
{   
    public override void Use(InGameDataManager manager)
    {
        // use medpack
		InGamePlayerManager pManager = FindObjectOfType<InGamePlayerManager>();
		GameObject myPlayer = null;
		foreach (var player in pManager.players)
		{
			if (player.GetPhotonView().IsMine)
			{
				myPlayer = player;
			}
		}

        //myPlayer.GetPhotonView().RPC("StartHeal", RpcTarget.All);
        float difference = myPlayer.GetComponent<Health>().maxHealth - myPlayer.GetComponent<Health>().currentHealth;
        myPlayer.GetComponent<Health>().Heal((int) difference);

        // change the health in the UI as well
        myPlayer.GetComponent<PlayerHUD>().heal(difference);
        NotificationSystem.Instance.Notify(new Notification("Used Medpack", NotificationType.Good));

        // remove this from the manager
        manager.currentItem = null;
    }
}
