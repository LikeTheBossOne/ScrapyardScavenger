using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{

    #region Singleton

    public static Settings Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    public float homebaseVolume;

    // Start is called before the first frame update
    void Start()
    {
        homebaseVolume = .05f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setHomebaseVolume(float value)
    {
        homebaseVolume = value;
    }

    public float getHomebaseVolume()
    {
        return homebaseVolume;
    }
}
