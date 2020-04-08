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
				if (Input.GetJoystickNames().Length > 1) {
					buyText.text = $"Press X to clear Debris\n[Cost: {cost} XP]";
				} else {
					buyText.text = $"Press B to clear Debris\n[Cost: {cost} XP]";
				}

				if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown("joystick button 0"))
                {
                    if (skillManager.BuyWithTemp(cost))
                    {
                        int index = buyableObj.parent.GetSiblingIndex();
                        PersistentBuyableManager.Instance.gameObject.GetPhotonView().RPC("RemoveBuyable", RpcTarget.All, index);
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
