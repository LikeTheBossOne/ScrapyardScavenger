using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerManager : MonoBehaviour
{
    public List<RectTransform> players;
    
    private void Awake()
    {
        players = new List<RectTransform>();
    }
    
    public void register(RectTransform adding)
    {
        players.Add(adding);
    }
}
