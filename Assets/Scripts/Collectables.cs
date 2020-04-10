using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Collectables : MonoBehaviour
{
     
    // If this collectable is near the "Player" tag 
    // it will be collected
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag( "Player"))
        {
            Debug.Log("Collected");
            Destroy(this.gameObject);
            SceneManager.LoadScene("Credit");
        }
    }
     
}
