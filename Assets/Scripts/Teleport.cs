using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
	public Transform teleportTarget; //teleport position
	public GameObject thePlayer; //teleporting player

	void OnTriggerEnter(Collider other)
	{
		thePlayer.transform.position = teleportTarget.transform.position;
		//Teleports the player to the target position
	}
}
