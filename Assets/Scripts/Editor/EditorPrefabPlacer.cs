using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorPrefabPlacer
{
    [MenuItem("Tools/Place Prefabs/Place 1x1 Rock")]
    private static void Place1X1Rock()
    {
        var prefabName = "";
        var randomNum = Random.Range(0, 3);
        switch (randomNum)
        {
            case 0:
                prefabName = "Rock1_1";
                break;
            case 1:
                prefabName = "Rock1_2";
                break;
            case 2:
            default:
                prefabName = "Rock1_3";
                break;
        }
        SpawnPrefabAtRandomPosition($"Assets/Prefabs/{prefabName}.prefab", 20, 0.5f);
    }
    
    [MenuItem("Tools/Place Prefabs/Place 2x2 Rock")]
    private static void Place2X2Rock()
    {
        SpawnPrefabAtRandomPosition("Assets/Prefabs/Rock2_1.prefab", 19, 0f);
    }

    private static void SpawnPrefabAtRandomPosition(string prefabPath, int gridRadius, float offset)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"{prefabPath} is not a valid prefab");
            return;
        }

        var newPosition = Vector3.zero;
        newPosition.x = Random.Range(-gridRadius, gridRadius) + offset;
        newPosition.z = Random.Range(-gridRadius, gridRadius) + offset;
        
        var newRock = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        newRock.transform.position = newPosition;
        Undo.RegisterCreatedObjectUndo(newRock, "New rock added");
    }
}
