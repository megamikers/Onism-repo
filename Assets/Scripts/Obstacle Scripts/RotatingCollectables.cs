using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingCollectables : MonoBehaviour
{
    private void Update()
    {
        // Rotate the Collectable at the given angles
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        // If this item is collected by the "Player", it's instantly destroyed
        if (collision.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);
        }
    }
}

