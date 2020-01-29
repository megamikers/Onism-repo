using UnityEngine;
using System.Collections;

public class CameraCtrl : MonoBehaviour {

	public Transform target;
	float lookSmooth = 0.09f;
	public float xTilt = 25f;
	Vector3 offsetFromTarget = new Vector3(0,2,-2);

	Vector3 destination = Vector3.zero;
	float rotateVel = 0;
	Movement2 charCtrl; //Not really needed.

	public void SetCameraTarget(Transform t){
		target = t;
		if(target != null){
			charCtrl = target.GetComponent<Movement2>();
			if(charCtrl == null) Debug.LogError("Cameras Target needs Movement2");
		}else Debug.LogError("Camera needs a target");
	}


	// Use this for initialization
	void Start(){
		SetCameraTarget(target);
	}

	void LateUpdate(){
		//Position
		destination = target.position + target.transform.rotation * offsetFromTarget;
		transform.position = destination;

		//Look At Rotation
		float eulerYAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y,target.eulerAngles.y, ref rotateVel,lookSmooth);
		transform.rotation = Quaternion.Euler(xTilt,eulerYAngle,0);
		//transform.rotation = Quaternion.Euler(transform.eulerAngles.x,eulerYAngle,0);
	}
}
