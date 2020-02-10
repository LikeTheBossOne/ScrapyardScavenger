using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerStats : MonoBehaviour
{
    public float health { get; private set; }
    public float damage { get; private set; }
    public int status { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        health = 200;
        damage = 20;
        status = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onDamage(float damage, GameObject damager, int atkStatus)
    {
        health = health - damage;
        if (atkStatus > 0)
        {
            status = atkStatus;
        }
    }
}
