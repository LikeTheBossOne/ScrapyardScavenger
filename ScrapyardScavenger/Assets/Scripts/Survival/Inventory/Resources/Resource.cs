using System;
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
	public string imageSlotName = null;

    public ResourceType type;
}

#region Enums

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

[Flags]
public enum ResourceTypeBitwise
{
    None = 0,
    ArmSleeve = 1 << 0,
    BeltStrap = 1 << 1,
    Disinfectant = 1 << 2,
    Gauze = 1 << 3,
    GunBarrel = 1 << 4,
    GunStock = 1 << 5,
    Gunpowder = 1 << 6,
    Handle = 1 << 7,
    Leather = 1 << 8,
    MetalBox = 1 << 9,
    PlasticBottle = 1 << 10,
    RustedCoil = 1 << 11,
    SafetyPin = 1 << 12,
    ShoulderPlate = 1 << 13,
    SugarPill = 1 << 14,
    WoodenPlank = 1 << 15,
    MAX = 1 << 16,
}

#endregion

public class ResourcePersistent {
	private Resource resource;
	private int count;

	public Resource Resource { get { return this.resource; } }
	public int Count { get { return this.count; } set { this.count = value; } }

	public ResourcePersistent(Resource r, int c) {
		this.resource = r;
		this.count = c;
	}
}