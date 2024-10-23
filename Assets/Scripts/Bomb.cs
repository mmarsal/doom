using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float lifeTime = 3f;
    public float explosionRadius = 5f;
    public float explosionForce = 100f;
    public LayerMask explosionLayerMask;

    private void OnEnable()
    {
        StartCoroutine(DetonateAfterTime(lifeTime));
    }

    private IEnumerator DetonateAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Explode();
        Destroy(gameObject);
    }

    private void Explode()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayerMask);

        foreach (Collider obj in objectsInRange)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = obj.transform.position - transform.position;
                rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
