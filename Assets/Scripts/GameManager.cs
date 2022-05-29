using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [SerializeField] private Light sunlight;
    [SerializeField] private Transform lifeBall;
    [SerializeField] private FadeController fadeController;
    
    private DeathSourceController deathSource;
    
    private void Awake()
    {
        Instance = this;
        sunlight.intensity = 1f;
        lifeBall.localScale = Vector3.zero;
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
        StartCoroutine(PlayOutroSequence());
    }

    private IEnumerator PlayOutroSequence()
    {
        //TODO Change Wall material to green?
        // Flip all remaining tiles to life
        lifeBall.DOScale(60f, 5f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(2f);
        StartCoroutine(fadeController.FadeOverlay(false, 3f, false));
    }
}
