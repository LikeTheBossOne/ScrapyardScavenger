using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private List<Resource> resources;
    private List<CraftableObject> crafts;
    private CraftingRecipe medpackRecipe;

    public int capacity;

    #region Singleton

    public static InventoryManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    void Start()
    {
        resources = new List<Resource>();
        crafts = new List<CraftableObject>();

        // Initialize Recipes
        medpackRecipe = Resources.Load<CraftingRecipe>("Recipes/ItemRecipes/MedpackRecipe");
        Debug.Log("Howdy?: " + medpackRecipe.howdy);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            // create a Gauze
            Debug.Log("Adding a Gauze");
            Gauze g = Resources.Load<Gauze>("Resources/Gauze");
            AddResource(g);
            Debug.Log("Gauze count: " + ResourceCount(g));
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            // create a Disinfectant
            Debug.Log("Adding a Disinfectant");
            Disinfectant d = Resources.Load<Disinfectant>("Resources/Disinfectant");
            AddResource(d);
            Debug.Log("Disinfectant count: " + ResourceCount(d));
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            // craft a medpack
            Debug.Log("Attempting to craft a medpack");
            medpackRecipe.Craft(this);
            PrintCrafts();
            PrintResources();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            // use a medpack!
            // so find a craft that is a medpack
            bool found = false;
            for (int i = 0; i < crafts.Count; i++)
            {
                if (crafts[i].name == "Medpack")
                {
                    // then call .Use(this) on it
                    Debug.Log("Using a medpack!");
                    found = true;
                    ((Medpack) crafts[i]).Use(this);
                    break;
                }
            }
            if (!found)
            {
                Debug.Log("No medpack found so could not use....");
            }
            
            
        }
    }

    public bool AddResource(Resource resource)
    {
        if (IsFull())
        {
            return false;
        }
        resources.Add(resource);
        return true;
    }

    public bool RemoveResource(Resource resource)
    {
        resources.Remove(resource);
        return true;
    }

    public int ResourceCount(Resource resource)
    {
        int count = 0;
        for (int i = 0; i < resources.Count; i++)
        {
            if (resources[i].name == resource.name)
            {
                count++;
            }
        }
        return count;
    }

    public bool ContainsResource(Resource resource)
    {
        for (int i = 0; i < resources.Count; i++)
        {
            if (resources[i].name == resource.name)
            {
                return true;
            }
        }
        return false;
    }
    
    public bool IsFull()
    {
        // not implemented yet
        return false;
    }

    public bool AddCraft(CraftableObject craft)
    {
        crafts.Add(craft);
        return true;
    }

    public void PrintCrafts()
    {
        Debug.Log("Printing all crafted objects");
        foreach (CraftableObject craft in crafts)
        {
            Debug.Log(craft.name);
        }
    }

    public void PrintResources()
    {
        Debug.Log("Printing all resources");
        foreach (Resource res in resources)
        {
            Debug.Log(res.name);
        }
    }

    public bool RemoveCraft(CraftableObject craft)
    {
        crafts.Remove(craft);
        return true;
    }

}
