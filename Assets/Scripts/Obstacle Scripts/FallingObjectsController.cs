using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectsController : MonoBehaviour
{ 
    // Public Variables
    public Transform[] triggerPositon;          // The position of the falling objects trigger
    public GameObject[] obstacleObjects;   // The Obstacle Objects position
     
    // When the player enters any trigger
    // execute the following
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            DelayedAction();    // Wait 1 second before dropping the object

            // Instantiate the falling object 
            Instantiate(obstacleObjects[0], triggerPositon[0].transform.position + new Vector3(0, 5, 3), Quaternion.identity);
             
            Debug.Log("Uh Oh");
        }
 
    }

    // While the player is on the trigger
    // Execute the following
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Taking Damage");
    }
     
    // When the player exits the trigger the player
    // stops taking damage
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Phew it's Over");
    }

    // Delay the amount of time before
    // The object falls after being triggered
    IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(1);
    }
}
