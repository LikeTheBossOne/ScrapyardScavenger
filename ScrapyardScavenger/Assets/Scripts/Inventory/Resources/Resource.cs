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


public class ResourcePersistent {
	private Resource resource;
	private int count;

	public Resource Resource { get { return this.resource; } }
	public int Count { get { return this.count; } }

	public ResourcePersistent(Resource r, int c) {
		this.resource = r;
		this.count = c;
	}
}