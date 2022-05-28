using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class GroundPieceManager
{
    private static List<GroundPieceController> groundPieces = new List<GroundPieceController>();
    private static List<GroundPieceController> alivePieces = new List<GroundPieceController>();
    private static List<GroundPieceController> deadPieces = new List<GroundPieceController>();

    public static void RegisterGroundPiece(GroundPieceController groundPiece)
    {
        groundPieces.Add(groundPiece);
        deadPieces.Add(groundPiece);
        groundPiece.RegisterListener(ChangedState);
    }

    public static GroundPieceController GetRandomGroundPiece(bool wantsDeadPiece)
    {
        if (wantsDeadPiece && deadPieces.Count > 0)
        {
            return deadPieces[Random.Range(0, deadPieces.Count)];
        }
        
        if (!wantsDeadPiece && alivePieces.Count > 0)
        {
            return alivePieces[Random.Range(0, alivePieces.Count)];
        }
        
        return null;
    }

    private static void ChangedState(GroundPieceController groundPiece, GroundPieceController.LifeState newLifeState)
    {
        if (newLifeState == GroundPieceController.LifeState.Alive)
        {
            deadPieces.Remove(groundPiece);
            alivePieces.Add(groundPiece);
        }
        else if (newLifeState == GroundPieceController.LifeState.Dead)
        {
            alivePieces.Remove(groundPiece);
            deadPieces.Add(groundPiece);
        }
    }
}
