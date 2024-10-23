using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObstacleGun : MonoBehaviour
{
    public bool isShooting;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float timeBetweenShoots = 0.2f;
    public float shootForce = 100f;

    public float nextShootTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if(isShooting && Time.time > nextShootTime)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(shootPoint.forward * shootForce, ForceMode.Impulse);
        nextShootTime = Time.time + timeBetweenShoots;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isShooting = true;   
        }
        else if (context.canceled)
        {
            isShooting = false;
        }
    }
}
