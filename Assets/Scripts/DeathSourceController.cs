using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeathSourceController : Spawner
{
    [SerializeField] private float deathRadius = 3;
    [SerializeField] private ParticleSystem fieldPS;
    [SerializeField] private ParticleSystem boundsPS;
    [SerializeField] private TorchController[] torches;
    [SerializeField] private CinemachineVirtualCamera vCam;

    private SphereCollider col;

    private void Start()
    {
        GameManager.Instance.RegisterDeathSource(this);
        col = GetComponent<SphereCollider>();
        UpdateDeathRadius();
        //Debug
        StartCoroutine(SpawnUnits());
    }

    public void IgniteTorch(int torchID)
    {
        if (torches.Length > torchID)
        {
            StartCoroutine(IgniteTorchSequence(torchID));
        }
    }

    private IEnumerator IgniteTorchSequence(int torchID)
    {
        // Move camera here to watch it change state
        vCam.Priority = 10;
        yield return new WaitForSeconds(1.5f);
        ShrinkDeathRadius();
        AudioController.Instance.PlayTorchLit();
        yield return new WaitForSeconds(0.1f);
        torches[torchID].ChangeState(true);
        yield return new WaitForSeconds(2f);
        vCam.Priority = 0;
    }

    private void ShrinkDeathRadius(float shrinkAmount = 1f)
    {
        deathRadius -= shrinkAmount;
        UpdateDeathRadius();
        
        // Check for game win
        if (deathRadius <= 1)
        {
            GameManager.Instance.TriggerWin();
        }
    }

    private void UpdateDeathRadius()
    {
        // Get fieldPS size
        var fieldShape = fieldPS.shape;
        fieldShape.radius = deathRadius;
        // Get boundsPS size
        var boundsShape = boundsPS.shape;
        boundsShape.radius = deathRadius;
        col.radius = deathRadius;
    }
}
