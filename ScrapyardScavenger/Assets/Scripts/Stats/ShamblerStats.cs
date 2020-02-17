using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShamblerStats : Stats
{
    
    public float damage { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        health = 10;
        damage = 10;
        status = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    void onDamage(int damage, GameObject damager, int atkStatus)
    {
        health = health - damage;
        if (atkStatus > 0)
        {
            status = atkStatus;
        }
        GetComponentInParent<ShamblerDetection>().gotShot(damager);
    }
}
