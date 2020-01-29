using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour{
	[SerializeField] Transform target;
	Vector3 defaultDistance = new Vector3(0f,1.5f,-2f);
	float distanceDamp = 0.5f;
	//float rotationDamp = 2f;

	Transform myT;

	Vector3 velocity = Vector3.one;

	void Awake(){
		myT = transform;
	}

	// Use this for initialization
	void Start (){
		
	
	}
	
	// Update is called once per frame
	void LateUpdate(){
		Vector3 toPos = target.position + (target.rotation * defaultDistance);
		Vector3 curPos = Vector3.SmoothDamp(myT.position, toPos, ref velocity, distanceDamp);
		myT.position = curPos;

		//myT.LookAt(target,target.up);
		myT.LookAt(target.position + new Vector3(0f,1f,0f), target.up);
		
/*
		Vector3 toPos = target.position + (target.rotation * defaultDistance);
		Vector3 curPos = Vector3.Lerp(myT.position, toPos, distanceDamp * Time.deltaTime);
		myT.position = curPos;

		Quaternion toRot = Quaternion.LookRotation(target.position - myT.position,target.up);
		Quaternion curRot = Quaternion.Slerp(myT.rotation, toRot, rotationDamp * Time.deltaTime);
		myT.rotation = curRot;
*/
		}

}
