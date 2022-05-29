using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float acceleration = 1500f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float rotationSpeed = 1000f;
    
    [Header("REFERENCES")]
    [SerializeField] private Transform mesh;
    [SerializeField] private CinemachineVirtualCamera virtualCam;

    private Rigidbody rb;
    private float horizontal = 0f;
    private float vertical = 0f;
    private CinemachineBasicMultiChannelPerlin virtualCamNoiseComponent;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (virtualCam != null)
        {
            virtualCamNoiseComponent = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    private void Update()
    {
        horizontal += Input.GetAxis("Horizontal");
        vertical += Input.GetAxis("Vertical");
        Rotate();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Rotate()
    {
        var velocity = rb.velocity;
        if (velocity == Vector3.zero) { return; }
        
        var lookDirection = Quaternion.LookRotation(velocity, Vector3.up);
        mesh.rotation = Quaternion.RotateTowards(mesh.rotation, lookDirection, rotationSpeed * Time.deltaTime);
    }

    private void Move()
    {
        if (horizontal != 0f || vertical != 0f)
        {
            var mult = Time.fixedDeltaTime * acceleration;
            var newPosition = new Vector3(horizontal * mult, 0f, vertical * mult);
            rb.AddRelativeForce(newPosition, ForceMode.Acceleration);
        }
#if UNITY_EDITOR
        //Move faster for testing
        var newMaxSpeed = maxSpeed * maxSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            newMaxSpeed *= 6f;
        }
        if (rb.velocity.sqrMagnitude > (newMaxSpeed))
        {
            var newMove = rb.velocity.normalized;
            newMove *= maxSpeed;
            rb.velocity = newMove;
        }
#else
        if (rb.velocity.sqrMagnitude > (maxSpeed * maxSpeed))
        {
            var newMove = rb.velocity.normalized;
            newMove *= maxSpeed;
            rb.velocity = newMove;
        }
#endif
        horizontal = 0f;
        vertical = 0f;
    }
    
    private void ShakeCloseToDeathSource(Vector3 deathSourcePosition)
    {
        var deathSourceDistance = Mathf.Max(0.5f, Vector3.Distance(transform.position, deathSourcePosition));
        virtualCamNoiseComponent.m_AmplitudeGain = 0.5f / deathSourceDistance;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("DeathSource"))
        {
            ShakeCloseToDeathSource(other.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DeathSource"))
        {
            virtualCamNoiseComponent.m_AmplitudeGain = 0f;
        }
    }
}
