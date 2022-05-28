using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedUnitController : MonoBehaviour
{
    [SerializeField] private Alignment alignment = Alignment.Ally;
    [SerializeField] private float acceleration = 1500f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float rotationSpeed = 300f;
    [SerializeField] private Transform mesh;
    [SerializeField] private ParticleSystem deathPS;

    private Rigidbody rb;
    private float horizontal = 0f;
    private float vertical = 0f;
    private bool hasTarget = false;
    private Spawner spawner;
    private GroundPieceController targetGroundPiece;

    private enum Alignment
    {
        Ally,
        Enemy
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (deathPS != null)
        {
            deathPS.gameObject.SetActive(false);
        }
    }

    public void Setup(Spawner spawner)
    {
        this.spawner = spawner;
    }

    private void GetNewTarget()
    {
        targetGroundPiece = GroundPieceManager.GetRandomGroundPiece(alignment == Alignment.Ally);
        hasTarget = targetGroundPiece != null;
    }
    
    private void Update()
    {
        if (hasTarget)
        {
            if (alignment == Alignment.Ally && targetGroundPiece.IsAlive() ||
                alignment == Alignment.Enemy && targetGroundPiece.IsDead())
            {
                GetNewTarget();
            }
        }
        else
        {
            GetNewTarget();
        }
        
        Rotate();
    }

    private void Rotate()
    {
        var velocity = rb.velocity;
        if (velocity == Vector3.zero) { return; }
        
        var lookDirection = Quaternion.LookRotation(velocity, Vector3.up);
        mesh.rotation = Quaternion.RotateTowards(mesh.rotation, lookDirection, rotationSpeed * Time.deltaTime);
    }
    
    private void FixedUpdate()
    {
        if (!hasTarget) { return; }
        
        GetDirection();
        Move();
    }

    private void GetDirection()
    {
        var targetDirection = targetGroundPiece.transform.position - transform.position;
        targetDirection.y = 0f;
        targetDirection = targetDirection.normalized;
        horizontal = targetDirection.x;
        vertical = targetDirection.z;
    }

    private void Move()
    {
        if (horizontal != 0f || vertical != 0f)
        {
            var mult = Time.deltaTime * acceleration;
            var newPosition = new Vector3(horizontal * mult, 0f, vertical * mult);
            rb.AddForce(newPosition, ForceMode.Acceleration);
        }
        if (rb.velocity.sqrMagnitude > (maxSpeed * maxSpeed))
        {
            var newMove = rb.velocity.normalized;
            newMove *= maxSpeed;
            rb.velocity = newMove;
        }
        horizontal = 0f;
        vertical = 0f;
    }

    public void Die()
    {
        // VFX
        deathPS.transform.SetParent(null);
        deathPS.gameObject.SetActive(true);
        deathPS.Play();
        // SFX
        AudioController.Instance.PlayUnitDied(transform.position);
        // Kill
        spawner.UnRegisterUnit(this);
        Destroy(gameObject);
    }
}
