using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : NetworkBehaviour
{
    [SerializeField] private Transform tower;
    public Transform Tower => tower;
    [SerializeField] private Transform body;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float towerRotationSpeed;
    [SerializeField] private float bodyRotationSpeed;

    private Rigidbody rb;
    public Rigidbody GetRB => rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void MoveTank(Vector3 direction)
    {
        Vector3 moveDirection = direction.normalized * moveSpeed;
        rb.velocity = moveDirection;
    }

    public void RotateBody(Vector3 direction)
    {
        LookAtDirection(body, direction, bodyRotationSpeed);
    }

    public void RotateTower(Vector3 direction)
    {
        LookAtDirection(tower, direction, towerRotationSpeed);
    }

    private void LookAtDirection(Transform objTransform, Vector3 direction, float rotationSpeed)
    {
        if (direction != Vector3.zero)
        {
            Quaternion neededRotation = Quaternion.LookRotation(direction);
            objTransform.rotation = Quaternion.Slerp(objTransform.rotation,
                                                    neededRotation,
                                                    rotationSpeed * Time.deltaTime);
        }
    }
}
