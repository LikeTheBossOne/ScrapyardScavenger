using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerManager : MonoBehaviour
{
    public List<Transform> players { get; private set; }
    private void Awake()
    {
        players = new List<Transform>();
    }
    
    public void register(Transform adding)
    {
        players.Add(adding);
    }
}
