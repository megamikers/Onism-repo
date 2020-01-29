using UnityEngine;
using System.Collections;


[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class ControlState{
	public bool isLeftShift = false, isLeftShiftPrev = false, isLeftShiftChanged = false;

	public void Update(){
		isLeftShiftPrev = isLeftShift;
		isLeftShift = Input.GetKey(KeyCode.LeftShift);
		isLeftShiftChanged = (isLeftShift != isLeftShiftPrev);
	}
}

public class Movement : MonoBehaviour {
	Animator mAnim;


	bool isWalking = false;

	private void Start(){
		mAnim = GetComponent<Animator>();
	}


	Vector3 v3CurrentDir = Vector3.zero;
	Vector3 v3PrevDir = Vector3.forward;
	int TurnDirection = 0;

	private void FixedUpdate(){

		bool is180 = mAnim.GetCurrentAnimatorStateInfo(0).IsName("Turning180x");
		bool inTrans = mAnim.IsInTransition(0);
		if(is180 || inTrans) return;

		float zAxis = Input.GetAxis("Vertical"),
			  xAxis = Input.GetAxis("Horizontal"),
			  dAxis = Mathf.Sqrt(xAxis * xAxis + zAxis * zAxis);

		v3CurrentDir.z = Mathf.Round(zAxis * 10)/10;
		v3CurrentDir.x = Mathf.Round(xAxis * 10)/10;

		Vector3 dir = new Vector3(xAxis,0,zAxis);
		float turning = 0;
		if(v3CurrentDir != Vector3.zero && !is180 && !inTrans){
			Quaternion lookRotation = Quaternion.LookRotation(v3CurrentDir);
			float angle = Quaternion.Angle(lookRotation,transform.rotation);

			if(v3CurrentDir != v3PrevDir){
				Debug.Log(" New Direction " + angle);
				//if(Input.GetKey("Fire1")) angle = 180;

				if(angle >= 160 && !is180){
					mAnim.SetTrigger("Turn180");
					Debug.Log("180");
					TurnDirection = 0;
				}else if(angle > 100) TurnDirection = -1;
				else if(angle < 80) TurnDirection = 1;


			}else{
				if(angle > 0.5){
					transform.rotation = Quaternion.Lerp(transform.rotation,lookRotation,Time.deltaTime * 5f);
				}else{
					TurnDirection = 0;
					transform.rotation = lookRotation;
				}
			}

			v3PrevDir.x = v3CurrentDir.x;
			v3PrevDir.z = v3CurrentDir.z;
		}


		//mAnim.SetFloat("Direction",(zAxis != 0 || xAxis != 0)?1:0);
		mAnim.SetFloat("Direction",dAxis);
		//Debug.Log(v3CurrentDir.z + " " + v3CurrentDir.x + " " + dAxis);

		//mAnim.SetFloat("Direction",zAxis);
		//mAnim.SetFloat("Turning",TurnDirection);
	}
    /*
	       #if UNITY_EDITOR
        Debug.Log(CommandState.ToString());
        #endif
        */
    #region Void UPDATES
    private void Update(){
		//if(Input.GetKeyDown(KeyCode.LeftShift)){
		//	isWalking = !isWalking;
		//	mAnim.SetBool("Walk",isWalking);
		//}

		if(Input.GetKeyDown(KeyCode.Space) || Input.GetAxis("Jump") > 0) mAnim.SetTrigger("Jump");

		//if(Input.GetKeyDown(KeyCode.LeftShift)) mAnim.SetBool("Walk",true);
		//else if(Input.GetKeyUp(KeyCode.LeftShift)) mAnim.SetBool("Walk",false);
		//mAnim.SetFloat("Turn",Input.GetAxis("Horizontal"));
		//mAnim.SetFloat("MoveX",Input.GetAxis("Horizontal"));
		//mAnim.SetFloat("MoveZ",Input.GetAxis("Vertical"));


		//float movex = Input.GetAxis("Horizontal") * 300f * Time.deltaTime;
		//float movez = Input.GetAxis("Vertical") * 300f * Time.deltaTime;

		//GetComponent<Rigidbody>().velocity = new Vector3(movex,0f,movez);
	}
    #endregion
}
