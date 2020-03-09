using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class MasterPlayerManager : MonoBehaviour
{
    public List<RectTransform> players;

    private void Awake()
    {
        players = new List<RectTransform>();
    }

    public void Register(RectTransform adding)
    {
        players.Add(adding);
        adding.GetComponent<Death>().OnPlayerDeath += PlayerDied;
    }

    public void PlayerDied(GameObject o)
    {
        players.Remove(o.GetComponent<RectTransform>());
    }
}
