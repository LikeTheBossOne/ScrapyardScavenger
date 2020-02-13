using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Inventory/Resource")]
public class Resource : ScriptableObject
{
    public int id;
    public string name;
    public string description;
    public bool showInInventory = true;
    public Sprite icon = null;

    void Start()
    {
        
    }

    public void Use()
    {

    }
}

public enum ResourceType
{
    ArmSleeve = 0,
    BeltStrap,
    Disinfectant,
    Gauze,
    GunBarrel,
    GunStock,
    Gunpowder,
    Handle,
    Leather,
    MetalBox,
    PlasticBottle,
    RustedCoil,
    SafetyPin,
    ShoulderPlate,
    SugarPill,
    WoodenPlank,
    SIZE
}
