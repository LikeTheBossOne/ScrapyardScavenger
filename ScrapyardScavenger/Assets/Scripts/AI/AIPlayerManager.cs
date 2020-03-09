using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class AIPlayerManager : MonoBehaviour
{
    public List<RectTransform> players;

    private void Awake()
    {
        players = new List<RectTransform>();
    }

    public void Register(RectTransform adding)
    {
        players.Add(adding);
    }

    void Update()
    {
        foreach (var p in players)
        {
            if (p == null) players.Remove(p);
        }
    }
}
