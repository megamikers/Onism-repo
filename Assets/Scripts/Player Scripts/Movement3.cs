using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class Movement3 : MonoBehaviour{
	// Variables 
	private Rigidbody mRBody;
	private Animator mAnim;

	private float	mVertAxis = 0,					//Vertical Input Axis, Control Forward/Backward movement
					mHoriAxis = 0,					//Horizontal Input Axis, Control Rotation.
					mDownRayDist = 2f,			//Range to scan downward, has to be at least 0.2
					mGroundedMin = 0.11f,			//Minimum distance to be concidered on the ground.
					mLandingMin = 0.8f,
					mWalkSpeed = 8f,				//How fast does the character walk.
					mTurnSpeed = 100f,				//How fast does the character turn
					mJumpSpeed = 5f,
					mPrevForwardSpeed = 0f;			//Keep track of the forward speed when jumping.

	private int mJumpState = 0;
	private float mJumpDelay = 0;
	private bool	mIsJumpStart = false,
					mIsDownRayHit = false,
					mIsGrounded = false;

	private RaycastHit mDownRayHit;

	private const float INPUT_MIN = 0.1f;
	

	#region Comp Events
	void Start(){
		mRBody = GetComponent<Rigidbody>();
		mAnim = GetComponent<Animator>();
	}
	
	void Update(){
		updateUserInput();
		turn(mHoriAxis);
	}

	void FixedUpdate(){
		updateRaycasts();
		move(mVertAxis,mIsJumpStart);
	}
	#endregion

	#region DataUpdates
	private void updateUserInput(){
		mVertAxis = Input.GetAxis("Vertical");
		mHoriAxis = Input.GetAxis("Horizontal");
		mIsJumpStart = (Input.GetAxisRaw("Jump") == 1);
	}

	private void updateRaycasts(){
		Vector3 origin = transform.position + (Vector3.up * 0.1f);
	 
		#if UNITY_EDITOR
		Debug.DrawLine(origin, origin + (Vector3.down * mDownRayDist),Color.red);
		#endif

		mIsDownRayHit = Physics.Raycast(origin,Vector3.down, out mDownRayHit, mDownRayDist);
		if(mIsDownRayHit){
			mIsGrounded = (mDownRayHit.distance <= mGroundedMin);
			if(mDownRayHit.distance > mGroundedMin){
				//Debug.Log(mDownRayHit.distance);
				//Debug.Log(mDownRayHit.collider.name);
			}
		}else{ mIsGrounded = false;  } //Debug.Log("no hit");

		/*
		Vector3 origin = transform.position + (Vector3.up * mDownRayDist);
		 
		#if UNITY_EDITOR
		Debug.DrawLine(origin, transform.position + (Vector3.down * mDownRayDist),Color.red);
		#endif

		if(Physics.Raycast(origin,Vector3.down, out mDownRayHit, mDownRayDist*2)){
			mIsGrounded = (mDownRayHit.distance <= mGroundedMin);
		}else mIsGrounded = false;
		*/
	}
	#endregion

	bool isJumpLaunch = false;
	public void onAnimJumpLaunch(){ isJumpLaunch = true;
	Debug.Log("onAnimJumpLaunch");
	}

	bool isJumpGrounded = false;
	public void onAnimJumpGrounded()
    {
        isJumpGrounded = true;
        Debug.Log("onAnimJumpGrounded");
	}

	bool isJumpComplete = false;
	public void onAnimJumpComplete()
    {
		isJumpComplete = true;
		Debug.Log("onAnimJumpComplete");
	}

	#region Control Movement
	public void move(float rate,bool doJump){
		Vector3 vMove = new Vector3(0,mRBody.velocity.y,0); //Gravity will change the body's Y velocity, so when applying forward movement need to keep current y

		//Calc forward velocity
		float forwardSpeed = 0;

		if(mIsGrounded && mJumpState == 0 ){ //Only Move forward when touching the ground.
			vMove.z = forwardSpeed = (Mathf.Abs(rate) > INPUT_MIN)? mWalkSpeed * rate : 0f;
		}


		//JumpState 0, nothing, 1, initial, 2 in air, 3 landing, 4 grounded.
		//Handle Jumping
		if(doJump && mIsGrounded && mJumpState == 0){
			Debug.Log("Jump Init");
			mPrevForwardSpeed = forwardSpeed; //Save current speed to keep momentum while airborne.
			//mAnim.SetTrigger("Jump");
			mJumpState = 1;
			mJumpDelay = 0.5f;//0.5f;
		}
		//else if(! mIsGrounded){
		//	forwardSpeed = mPrevForwardSpeed;
		//}

		if(isJumpLaunch){
			isJumpLaunch = false;
			vMove.y += mJumpSpeed * 1.5f;
		}

		if(mJumpState != 0){
			mJumpDelay -= Time.deltaTime;
			Debug.Log("vy " + vMove.y.ToString());
			Debug.Log(mJumpDelay);
			if(mJumpDelay <= 0 && mJumpState == 1){
				//vMove.y += mJumpSpeed * 1.1f;
				mJumpState = 2;
				Debug.Log("JumpState 2");
			} 

			//if(mJumpState == 1 && mAnim.GetCurrentAnimatorStateInfo(0).IsName("FallingIdle")){
			//	vMove.y += mJumpSpeed;
			//	mJumpState = 2;
			//Debug.Log("JumpState 2");
			//}


			else if(mJumpState == 2 && mIsDownRayHit && mDownRayHit.distance <= mLandingMin && mAnim.GetCurrentAnimatorStateInfo(0).IsName("FallingIdle")){
				mJumpState = 3;
				//Debug.Log("----------------------------------------------");
				//Debug.Log("Landing " + mDownRayHit.distance.ToString() + " ");
				//Debug.Log(transform.position.y);
				//Debug.Log("----------------------------------------------");
			}
			
			/*
			if(mIsGrounded) mJumpState = 0;
			else if(mJumpState == 1){ //Beginning Jump Sequence
				bool isFalling = mAnim.GetCurrentAnimatorStateInfo(0).IsName("FallingIdle");
				bool inTrans = mAnim.IsInTransition(0);
				if(isFalling && !inTrans) mJumpState = 2;
			}
			*/
			if(mJumpState == 3 && mIsGrounded){ mJumpState = 4; mJumpDelay = 1f; Debug.Log(mAnim.GetCurrentAnimatorStateInfo(0).IsName("JumpEnd")); Debug.Log(mAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle")); Debug.Log(mAnim.IsInTransition(0)); } 
			if(mJumpState <= 3) vMove.z = mPrevForwardSpeed; //forwardSpeed = mPrevForwardSpeed; //&& !mIsGrounded
			//if(mJumpState == 3 && mIsGrounded && !mAnim.GetCurrentAnimatorStateInfo(0).IsName("JumpEnd") && mAnim.IsInTransition(0)){ mJumpState = 0; Debug.Log("Exit jump state"); }
			if(mJumpState == 4 && mJumpDelay < 0){ mJumpState = 0; Debug.Log("Exit jump state"); }
			//Debug.Log(forwardSpeed);
		}

		//Apply Velocity
		//vMove.z = forwardSpeed;
		mRBody.velocity = transform.TransformDirection(vMove); //convert it to world space, so it takes rotation into account when dealing with the direction

		//Set Animation States
		mAnim.SetFloat("Direction",(forwardSpeed == 0)?0:1);
		mAnim.SetBool("IsGrounded",mIsGrounded);
		mAnim.SetInteger("JumpState",mJumpState);
	}

	public void turn(float rate){
		if(Mathf.Abs(rate) > INPUT_MIN)
			transform.rotation *= Quaternion.AngleAxis(mTurnSpeed * rate * Time.deltaTime,Vector3.up);
	}
	#endregion
}