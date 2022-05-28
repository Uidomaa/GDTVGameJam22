using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotateOnAwake : MonoBehaviour
{
    private void Awake()
    {
        transform.Rotate(Vector3.up, 90 * UnityEngine.Random.Range(0, 3));
    }
}
