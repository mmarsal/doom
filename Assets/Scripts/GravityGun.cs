using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityGun : MonoBehaviour
{
    public bool isPulling;
    public bool isHolding;
    public Transform shootPoint;
    public float raycastRange = 50f;
    public LayerMask pullableLayer;
    public float pullForce = 10f;
    public float stopDistance = 2f;
    public float holdOffset = 1.5f;

    private GameObject pulledObject;
    private Rigidbody pulledRb;

    // Update is called once per frame
    void Update()
    {
        if (isPulling || isHolding)
        {
            if (isPulling && !isHolding && pulledObject == null)
            {
                ShootRay();
            }
            if (pulledObject != null)
            {
                if (isHolding)
                {
                    HoldObject();
                } else {
                    PullObject();
                }
            }

        }

        if (isPulling && pulledObject == null)
        {
            ShootRay();
        }

        if (pulledObject != null)
        {
            PullObject();
        }
    }

    private void HoldObject()
    {
        Vector3 holdPosition = shootPoint.position + shootPoint.forward * holdOffset;
        pulledObject.transform.position = holdPosition;

        pulledRb.velocity = Vector3.zero;
        pulledRb.angularVelocity = Vector3.zero;
    }

    private void PullObject()
    {
        Vector3 directionToPlayer = shootPoint.position - pulledObject.transform.position;
        float distance = directionToPlayer.magnitude;

        if (distance < stopDistance)
        {
            isHolding = true;
            return;
        }
        
        pulledRb.AddForce(directionToPlayer.normalized * pullForce, ForceMode.Acceleration);
    }

    private void StopPulling()
    {
        if (pulledRb != null)
        {
            pulledRb.useGravity = true;
        }

        pulledObject = null;
        pulledRb = null;
    }

    private void ShootRay()
    {
        RaycastHit hit;
        bool isHit = Physics.Raycast(shootPoint.position, shootPoint.forward, out hit, raycastRange, pullableLayer);
        Debug.DrawRay(shootPoint.position, shootPoint.forward * raycastRange, isHit ? Color.green : Color.red);

        if (isHit && hit.collider != null)
        {
            pulledObject = hit.collider.gameObject;
            pulledRb = pulledObject.GetComponent<Rigidbody>();

            if (pulledRb != null)
            {
                pulledRb.useGravity = false;
            }
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isPulling = true;
        }
        else if (context.canceled)
        {
            isPulling = false;
            isHolding = false;
            StopPulling();
        }
    }
}
