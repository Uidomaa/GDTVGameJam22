using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class TreeController : Spawner
{
    [SerializeField] private int treeID = 0;
    [SerializeField] private float tileRadius = 1.5f;
    [SerializeField] private LayerMask tileLayer;
    [SerializeField] private Transform treeTransform;
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private AudioSource treeAudioSource;
    [SerializeField] private AudioClip growClip;
    [SerializeField] private AudioClip sproingClip;


    private bool hasGrown = false;
    private int numSurroundingTiles;
    private int surroundingTilesAlive = 0;

    private void Start()
    {
        GetSurroundingTiles();
        treeTransform.localScale = Vector3.zero;
    }

    private void GetSurroundingTiles()
    {
        //Get tiles around tree to determine how many are required in order to grow
        Collider[] results = new Collider[16];
        var numCols = Physics.OverlapSphereNonAlloc(transform.position, tileRadius, results, tileLayer, QueryTriggerInteraction.Collide);
        for (var index = 0; index < numCols; index++)
        {
            var col = results[index];
            // Debug.Log(col.transform.name);
            GroundPieceController groundPiece = col.gameObject.GetComponent<GroundPieceController>();
            if (groundPiece != null)
            {
                groundPiece.RegisterListener(GroundTileFlipped);
                numSurroundingTiles++;
            }
        }
    }
    
    private void GroundTileFlipped(GroundPieceController piece, GroundPieceController.LifeState lifeState)
    {
        if (hasGrown) { return; }
        
        surroundingTilesAlive += lifeState == GroundPieceController.LifeState.Alive ? 1 : -1;
        Debug.Assert(surroundingTilesAlive >= 0, "Surrounding Tiles alive must be greater than 0!");
        if (surroundingTilesAlive == numSurroundingTiles)
        {
            Grow();
        }
    }

    private void Grow()
    {
        hasGrown = true;
        AudioController.Instance.PlayTreeGrow1(treeAudioSource, growClip);
        Vector3 initialScale = new Vector3(0.05f, 0.05f, 1f);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(treeTransform.DOScale(initialScale, 3f).SetEase(Ease.OutExpo).OnComplete(GrowPhase2));
        mySequence.Append(treeTransform.DOScale(Vector3.one, 0.3f)).OnComplete(HasGrown);
        //Camera
        virtualCam.Priority = 10;
    }

    private void GrowPhase2()
    {
        impulseSource.GenerateImpulse(0.3f);
        AudioController.Instance.PlayTreeGrow2(treeAudioSource, sproingClip);
    }

    private void HasGrown()
    {
        virtualCam.Priority = 0;
        GameManager.Instance.TreeGrown(treeID);
        StartCoroutine(SpawnUnits());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, tileRadius);
    }
}
