using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ReadyScreenUIObserver : MonoBehaviourPun
{
    public GameObject readyButton;
    public GameObject notReadyButton;


    public void ReadyUp()
    {
        readyButton.SetActive(false);
        notReadyButton.SetActive(true);
        photonView.RPC("PlayerReadied", RpcTarget.All);
    }

    public void NotReadyPressed()
    {
        readyButton.SetActive(true);
        notReadyButton.SetActive(false);
        photonView.RPC("PlayerNotReadied", RpcTarget.All);
    }

    public void ExitGame()
    {
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
