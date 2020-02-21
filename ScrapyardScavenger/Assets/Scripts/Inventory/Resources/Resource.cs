using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Inventory/Resource")]

public class Resource : Collectable
{
    public int id;
    public string description;
    public bool showInInventory = true;
    public Sprite icon = null;
    public ResourceType type;
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
