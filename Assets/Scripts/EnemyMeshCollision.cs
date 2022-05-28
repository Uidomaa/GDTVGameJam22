using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeshCollision : MonoBehaviour
{
    [SerializeField] private SpawnedUnitController unitOwner;
    
    public void Die()
    {
        unitOwner.Die();
    }
}
