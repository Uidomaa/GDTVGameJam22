using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchController : MonoBehaviour
{
    [SerializeField] private ParticleSystem deathPS;
    [SerializeField] private ParticleSystem lifePS;

    private void Start()
    {
        ChangeState(false);
    }

    public void ChangeState(bool isLife)
    {
        deathPS.gameObject.SetActive(!isLife);
        lifePS.gameObject.SetActive(isLife);
    }
}
