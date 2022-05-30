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
    private int torchesLit = 0;
    private CinemachineBasicMultiChannelPerlin virtualCamNoiseComponent;

    private void Start()
    {
        GameManager.Instance.RegisterDeathSource(this);
        col = GetComponent<SphereCollider>();
        virtualCamNoiseComponent = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        UpdateDeathRadius();
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
        AudioController.Instance.PlayRumble(1f);
        yield return new WaitForSeconds(1.5f);
        AudioController.Instance.PlayTorchLit();
        ShrinkDeathRadius();
        yield return new WaitForSeconds(0.1f);
        torches[torchID].ChangeState(true);
        torchesLit++;
        // Play outro sequence if death defeated
        if (torchesLit >= torches.Length)
        {
            StartCoroutine(DeathSourceDefeated());
            yield break;
        }
        yield return new WaitForSeconds(2f);
        // Return to player
        AudioController.Instance.StopRumble(1f);
        vCam.Priority = 0;
    }

    private void ShrinkDeathRadius(float shrinkAmount = 1f)
    {
        deathRadius -= shrinkAmount;
        UpdateDeathRadius();
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

    private IEnumerator DeathSourceDefeated()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.Outro);
        // Increase shake
        virtualCamNoiseComponent.m_AmplitudeGain *= 2f;
        virtualCamNoiseComponent.m_FrequencyGain *= 2f;
        // Shrink death radius to 0
        ShrinkDeathRadius(deathRadius);
        // Kill all enemies
        shouldSpawnUnits = false;
        for (var index = spawnedUnits.Count - 1; index >= 0; index--)
        {
            var spawnedUnit = spawnedUnits[index];
            spawnedUnit.Die();
        }
        AudioController.Instance.FadeRumblePitch(-0.5f, 3f);
        AudioController.Instance.StopBGM(3f);
        yield return new WaitForSeconds(3f);
        AudioController.Instance.StopRumble(0f);
        // Stop noise
        virtualCamNoiseComponent.m_AmplitudeGain *= 0f;
        virtualCamNoiseComponent.m_FrequencyGain *= 0f;

        GameManager.Instance.TriggerWin();
    }
}
