using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class BuyDebris : MonoBehaviourPun
{
    public LayerMask buyableLayer;

    private SkillManager skillManager;

    private Text buyText;

    void Start()
    {
        skillManager = GetComponent<PlayerControllerLoader>().skillManager;

        buyText = GameObject.Find("Buyable Canvas").GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;
        BuyableCheck();
    }

    private void BuyableCheck()
    {
        Transform eyeCam = transform.Find("Cameras/Main Player Cam");
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(eyeCam.position, eyeCam.forward, out hit, 2.5f, buyableLayer))
        {
            Transform buyableObj = hit.collider.transform;
            int cost = buyableObj.parent.GetComponent<Buyable>().cost;

            if (skillManager.CanBuyWithTemp(cost))
            {
				if (Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0] != "") {
					buyText.text = $"Press X to clear Debris\n[Cost: {cost} XP]";
				} else {
					buyText.text = $"Press C to clear Debris\n[Cost: {cost} XP]";
				}

				if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown("joystick button 0"))
                {
                    if (skillManager.BuyWithTemp(cost))
                    {
                        int index = buyableObj.parent.GetSiblingIndex();
                        PersistentBuyableManager.Instance.gameObject.GetPhotonView().RPC("RemoveBuyable", RpcTarget.All, index);
                        // notify the EnemySpawner as well
                        // but FIRST determine which zone this should unlock
                        Zones thisZone;
                        Zones[] possibleZones = buyableObj.parent.GetComponent<Buyable>().ZonesToUnlock;
                        if (possibleZones.Length == 1)
                        {
                            thisZone = possibleZones[0];
                        }
                        else // Length == 2 then
                        {
                            // see which zone this player is in!
                            if (GetComponent<ZoneManager>().GetCurrentZone() == possibleZones[0])
                            {
                                // then do the other one
                                thisZone = possibleZones[1];
                            }
                            else thisZone = possibleZones[0];
                        }


                        GameObject.Find("EnemySpawner").GetPhotonView().RPC("UnlockZone", 
                            RpcTarget.All, (int) thisZone);
                    }
                }
            }
            else
            {
                buyText.text = $"Can't clear Debris!\n[Cost: {cost} XP]";
            }
        }
        else
        {
            buyText.text = "";
        }
    }
}
