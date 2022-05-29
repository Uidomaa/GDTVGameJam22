using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [SerializeField] private Light sunlight;
    
    private DeathSourceController deathSource;
    
    private void Awake()
    {
        Instance = this;
        sunlight.intensity = 1f;
    }

    public void RegisterDeathSource(DeathSourceController newDS)
    {
        deathSource = newDS;
    }
    
    public void TreeGrown(int treeID)
    {
        deathSource.IgniteTorch(treeID);
        sunlight.intensity += 0.25f;
    }

    public void TriggerWin()
    {
        //TODO
        Debug.Log("You win! :tada:");
        //TODO Kill all enemies
        //TODO Change Wall material to green
        
    }
}
