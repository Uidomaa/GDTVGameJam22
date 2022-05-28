using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private int maxUnits = 4;
    [SerializeField] private Vector2 spawnDelay;
    [SerializeField] private GameObject spawnUnitPrefab;
    
    protected List<SpawnedUnitController> spawnedUnits = new List<SpawnedUnitController>();
    
    protected IEnumerator SpawnUnits()
    {
        while (enabled)
        {
            if (spawnedUnits.Count < maxUnits)
            {
                var newUnit = Instantiate(spawnUnitPrefab, transform.position, spawnUnitPrefab.transform.rotation).GetComponent<SpawnedUnitController>();
                newUnit.Setup(this);
                RegisterNewUnit(newUnit);
                yield return new WaitForSeconds(Random.Range(spawnDelay.x, spawnDelay.y));
            }
            else
            {
                yield return new WaitForSeconds(spawnDelay.x);
            }
        }
    }

    private void RegisterNewUnit(SpawnedUnitController newUnit)
    {
        spawnedUnits.Add(newUnit);
    }
    
    public void UnRegisterUnit(SpawnedUnitController unit)
    {
        spawnedUnits.Remove(unit);
    }
}
