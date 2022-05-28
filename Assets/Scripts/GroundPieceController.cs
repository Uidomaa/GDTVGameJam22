using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GroundPieceController : MonoBehaviour
{
    [SerializeField] private float life = 0f;
    [SerializeField] private LifeState lifeState = LifeState.Dead;
    [SerializeField] private float aliveThreshold = 0.75f;
    [SerializeField] private float deadThreshold = 0.25f;
    [SerializeField] private float flipSpeed = 1f;
    [SerializeField] private bool shouldRandomRotateOnStart = true;
    
    [Header("References")]
    [SerializeField] private Transform triangle;
    [SerializeField] private MeshRenderer[] meshRenderers;

    private float deathSourceDistance;
    private Quaternion deadRotation;
    private Quaternion aliveRotation;
    private float progress = 0f;
    private List<Action<GroundPieceController, LifeState>> listeners = new();

    public enum LifeState
    {
        Dead,
        Alive,
        BecomingDead,
        BecomingAlive
    }
    
    private void Awake()
    {
        GroundPieceManager.RegisterGroundPiece(this);
        if (shouldRandomRotateOnStart)
        {
            transform.Rotate(Vector3.up, 90 * Random.Range(0, 3));
        }
        deadRotation = triangle.rotation;
        var aliveEuler = deadRotation.eulerAngles;
        aliveEuler.x = 89f;
        aliveRotation = Quaternion.Euler(aliveEuler);
        flipSpeed = Random.Range(flipSpeed * 0.75f, flipSpeed * 1.25f);
    }

    private void Update()
    {
        switch (lifeState)
        {
            case LifeState.Dead:
                if (life > aliveThreshold)
                {
                    lifeState = LifeState.BecomingAlive;
                    progress = 0f;
                    AudioController.Instance.PlayPaperFlipAudio(true, transform.position);
                }
                break;
            case LifeState.Alive:
                if (life < deadThreshold)
                {
                    lifeState = LifeState.BecomingDead;
                    progress = 0f;
                    AudioController.Instance.PlayPaperFlipAudio(false, transform.position);
                }
                break;
            case LifeState.BecomingDead:
                Flip(LifeState.Dead, aliveRotation, deadRotation);
                break;
            case LifeState.BecomingAlive:
                Flip(LifeState.Alive, deadRotation, aliveRotation);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Flip(LifeState targetState, Quaternion fromRot, Quaternion toRot)
    {
        triangle.rotation = Quaternion.Slerp(fromRot, toRot, progress);
        progress += Time.deltaTime * flipSpeed;
        if (progress > 1f)
        {
            lifeState = targetState;
            foreach (var action in listeners)
            {
                action?.Invoke(this, lifeState);
            }
        }
    }

    public void RegisterListener(Action<GroundPieceController, LifeState> callback)
    {
        listeners.Add(callback);
    }

    public bool IsAlive()
    {
        return lifeState == LifeState.Alive;
    }

    public bool IsDead()
    {
        return lifeState == LifeState.Dead;
    }

    private float GetDistanceToDeathSource(Vector3 deathSourcePosition)
    {
        if (deathSourceDistance <= 0)
        {
            deathSourceDistance = Vector3.Distance(transform.position, deathSourcePosition);
        }
        return deathSourceDistance;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            life += 0.05f;
        }
        else if (other.CompareTag("Enemy"))
        {
            life -= 0.05f;
        }
        else if (other.CompareTag("DeathSource"))
        {
            //Scale strength based on distance from death source (should always be greater than 0)
            life -= 0.1f / GetDistanceToDeathSource(other.transform.position);
        }
        life = Mathf.Clamp01(life);
    }
}
