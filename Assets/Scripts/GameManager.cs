using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    private DeathSourceController deathSource;
    
    private void Awake()
    {
        Instance = this;
    }

    public void RegisterDeathSource(DeathSourceController newDS)
    {
        deathSource = newDS;
    }
    
    public void TreeGrown(int treeID)
    {
        deathSource.IgniteTorch(treeID);
    }

    public void TriggerWin()
    {
        //TODO
        Debug.Log("You win! :tada:");
    }
}
