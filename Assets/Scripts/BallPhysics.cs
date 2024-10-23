using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    public float speed = 5f;
    public Vector3 dir = new Vector3(0, 0, 0);
    public float gravity = 9.81f;
    public GameObject gameObj;

    // Start is called before the first frame update
    void Start()
    {
        gameObj.GetComponent<Rigidbody>().AddForce(transform.right * speed, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        // gameObj.GetComponent<Rigidbody>().AddForce(transform.up * speed, ForceMode.Force);
    }
}
