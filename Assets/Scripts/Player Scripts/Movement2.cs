using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class Movement2 : MonoBehaviour{
	float inputDelay = 0.1f;
	float forwardVel = 3f;
	float rotateVel = 100f;
	float jumpVel = 5f;
	float distToGround = 0.05f;
	public LayerMask ground;

	public Quaternion targetRotation;
	Rigidbody rBody;
	float forwardInput = 0;
	float turnInput = 0;
	float jumpInput = 0;

	Vector3 velocity = Vector3.zero;

	public float downAccel = 0.5f;

	Animator mAnim;

	bool Grounded(){
		Debug.DrawLine(transform.position,transform.position + (Vector3.down * distToGround),Color.red);
		//return Physics.Raycast(transform.position,Vector3.down, distToGround, ground);
		#if UNITY_EDITOR
		Debug.DrawLine(transform.position+(Vector3.up * distToGround), transform.position + (Vector3.down * distToGround),Color.red);
		#endif

		return Physics.Raycast(transform.position+(Vector3.up * distToGround),Vector3.down, distToGround*2, ground);
	}

	// Use this for initialization
	void Start () {
		targetRotation = transform.rotation;
		rBody = GetComponent<Rigidbody>();
		mAnim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		forwardInput = Input.GetAxis("Vertical");
		turnInput = Input.GetAxis("Horizontal");
		jumpInput = Input.GetAxisRaw("Jump");

		if(Mathf.Abs(turnInput) > inputDelay){
			targetRotation  *= Quaternion.AngleAxis(rotateVel * turnInput * Time.deltaTime,Vector3.up);
			transform.rotation = targetRotation;	
		}
	}

	void FixedUpdate(){
		Vector3 vec = rBody.velocity;

		//Jumping.
		bool isGrounded = Grounded();
		Debug.Log(isGrounded);


		if(jumpInput > 0 && isGrounded){
			velocity.y = jumpVel;
			vec.y += jumpVel;     
		}else if(jumpInput == 0 && isGrounded){
			velocity.y = 0;
			//vec.y = 0;
		}else{ //falling
			//velocity.y -= downAccel;


		}
		/**/


			if(Mathf.Abs(forwardInput) > inputDelay){ 
				velocity.z = forwardVel * forwardInput;
				mAnim.SetFloat("Direction",1f);
			}else{
				velocity.z = 0;
				mAnim.SetFloat("Direction",0f);
			} 

			Debug.Log(vec);
			//rBody.velocity = transform.TransformDirection(velocity); //Without this it will always move in the world z position, not taking current rotation into consideration.

			Vector3 tmp = transform.TransformDirection(velocity);

			vec.x = tmp.x;
			vec.z = tmp.z;


		rBody.velocity = vec;



		//Moving forward
		//if(Mathf.Abs(forwardInput) > inputDelay) rBody.velocity = transform.forward * forwardInput * forwardVel;
		//}else rBody.velocity = Vector3.zero;
	}
}
