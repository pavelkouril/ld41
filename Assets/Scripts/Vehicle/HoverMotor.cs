using UnityEngine;
using System.Collections;
using System;

public class HoverMotor : MonoBehaviour
{
    public float speed = 90f;
    public float turnSpeed = 5f;
    public float hoverForce = 65f;
    public float hoverHeight = 3.5f;
    private float powerInput;
    private float turnInput;
    private Rigidbody carRigidbody;

    public bool MovedByAi;

    void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        powerInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverHeight, ~(1 << 9)))
        {
            float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
            Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
            carRigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
        }

        if (!CardGameManager.Instance.IsGameRunning)
        {
            return;
        }

        if (!MovedByAi)
        {
            carRigidbody.AddRelativeForce(0f, 0f, powerInput * speed);
            carRigidbody.AddRelativeTorque(0f, turnInput * turnSpeed, 0f);
        }
    }

    internal void Input_SteerLeft()
    {
        carRigidbody.AddRelativeTorque(0f, -1 * turnSpeed, 0f);
    }

    internal void Input_SteerRight()
    {
        carRigidbody.AddRelativeTorque(0f, 1 * turnSpeed, 0f);
    }

    internal void Input_SteerAccelerate()
    {
        carRigidbody.AddRelativeForce(0f, 0f, 1 * speed);
    }

    internal void Input_SteerBrake()
    {
        carRigidbody.AddRelativeForce(0f, 0f, -1 * speed);
    }
}
