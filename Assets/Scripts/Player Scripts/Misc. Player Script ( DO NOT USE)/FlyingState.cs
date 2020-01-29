using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SPLabs.Controllers{
	public class FlyingState : BaseState {
	#region Vars and Constructor
		private float	mSpeedtrans = 3f,
						mZSpeed		= 3f,
						mZSpeedFast	= 7f,
						mZSpeedFast2= 25f,
						mUpSpeed	= 2f,
						mDownSpeed	= -3.5f,
						mTurnSpeed	= 100f,
						mAccel		= 0.07f,			//Accelerating Rate
						mDAccel		= 0.2f;				//De-Accelerating Rate

		private int		mFlyMode	= 0;
		private bool	mFastMode	= false;

		public FlyingState(){ this.name = "Flying"; }
	#endregion

	#region State Overrides
		public override void OnStateStart(UserControlState ucs){
			mFlyMode = 1;

			ucs.Anim.SetInteger("FlyMode",mFlyMode);
			ucs.Anim.SetFloat("Speed",0);
			ucs.RBody.useGravity = false;

			ucs.Speed.Set(0,0,0);
		}

		public override void OnStateEnd(UserControlState ucs){
			ucs.Anim.SetInteger("FlyMode",0);
		}

		public override void Update(UserControlState ucs){
			//Check for specific inputs that only matters during the grounded state.
			if(Input.GetKey(KeyCode.Space) && ucs.Speed.z > 0.1) mFlyMode = 2;
			else mFlyMode = 1;

			if(Input.GetKeyDown(KeyCode.F)) mFastMode = !mFastMode;

			//if(Input.GetKeyDown(KeyCode.C)) mIsCrouch = !mIsCrouch;
			//if(Input.GetAxisRaw("Jump") == 1){
			//	StateMan.ChangeState("Jump",ucs);
			//	return;
			//}

			//Perform Turning
			if(Mathf.Abs(ucs.HoriAxis) > 0.1) ucs.Trans.rotation *= Quaternion.AngleAxis(mTurnSpeed * ucs.HoriAxis * Time.deltaTime,Vector3.up);
		}

		public override void FixedUpdate(UserControlState ucs){
			setVelocity(ucs);

			ucs.Anim.SetInteger("FlyMode",mFlyMode);
			//ucs.Anim.SetFloat("Speed",(mFastMode)?1:3);

			if(mFlyMode == 2){
				float s = ucs.Anim.GetFloat("Speed");
				float t = (mFastMode)?1:3;
				float dif = t-s;

				if(Mathf.Abs(dif) < 0.1) mSpeedtrans = t;
				else mSpeedtrans += dif * 0.1f; 

				ucs.Anim.SetFloat("Speed",mSpeedtrans);
			}

			//Determine Speed
			//Vector3 vMove = new Vector3(0,ucs.RBody.velocity.y,0); //Gravity will change the body's Y velocity, so when applying forward movement need to keep current y
			//calcSpeed(ucs);
			//vMove.z = ucs.ForwardSpeed; 

			//Set Animations
			//ucs.Anim.SetFloat("Speed",ucs.ForwardSpeed);

			//convert vect3 to world space, so it takes rotation into account when dealing with the direction
			//ucs.RBody.velocity = ucs.Trans.TransformDirection(vMove);
		}
		#endregion

		#region Helper functions

		private void setVelocity(UserControlState ucs){
			float vAAxis = Mathf.Abs(ucs.VertAxis);
			float zTarget = 0, yTarget = 0;

			//Movement
			if(vAAxis > 0.1f){
				if(mFlyMode == 1) zTarget = mZSpeed;
				else if(mFlyMode == 2 && !mFastMode) zTarget = mZSpeedFast;
				else if(mFlyMode == 2 && mFastMode) zTarget = mZSpeedFast2;
				zTarget *= ucs.VertAxis;
			}

			if(Input.GetKey(KeyCode.Q)) yTarget = mUpSpeed;
			else if(Input.GetKey(KeyCode.E)) yTarget = mDownSpeed;

			float	zDif = zTarget - ucs.Speed.z,
					yDif = yTarget - ucs.Speed.y;

			//Acceleration
			if(Mathf.Abs(zDif) < 0.1f) ucs.Speed.z = zTarget;
			else ucs.Speed.z += zDif * ((zTarget != 0)? mAccel : mDAccel);

			if(Mathf.Abs(yDif) < 0.1f) ucs.Speed.y = yTarget;
			else ucs.Speed.y += yDif * ((yTarget != 0)? mAccel : mDAccel);
			//Debug.Log(ucs.Speed);
			ucs.RBody.velocity = ucs.Trans.TransformDirection(ucs.Speed);

		/*
			float y = ucs.RBody.velocity.y;
			if(mJumpState == 1){
				//ucs.RBody.velocity = ucs.Trans.TransformDirection(0,ucs.RBody.velocity.y + (mJumpSpeed ),ucs.ForwardSpeed);
				y += mJumpSpeed;
				mJumpState = 2;
			}else if(mJumpState == 4){
				ucs.ForwardSpeed = 0f;
				//ucs.RBody.velocity = ucs.Trans.TransformDirection(0,ucs.RBody.velocity.y,ucs.ForwardSpeed);
			}

			ucs.RBody.velocity = ucs.Trans.TransformDirection(0,y,ucs.ForwardSpeed);
			*/
		}


		/*
		private void calcSpeed(UserControlState ucs){
			float dif,
				vAAxis = Mathf.Abs(ucs.VertAxis),
				targetSpeed = 0f; //By default the idle target speed is zero.

			//Determine the max target speed the player is trying to go.
			if(vAAxis > 0.1f){
				if(mIsCrouch){
					targetSpeed = vAAxis * ((ucs.VertAxis > 0)? mCrouchSpeed : mCrouchSpeedB); 
				}else{
					if(ucs.VertAxis < 0)						targetSpeed = mBackSpeed	* vAAxis;	//Moving Backwards
					else if(ucs.VertAxis > 0 && mIsFastMode)	targetSpeed = mRunSpeed		* vAAxis;	//Running Forward
					else										targetSpeed = mWalkSpeed	* vAAxis;	//Walking Forward
				}										
			}

			//Figure out the difference of how fast the user is going and how fast they want to go.
			dif = targetSpeed - ucs.ForwardSpeed;
			if(Mathf.Abs(dif) <= 0.1f) ucs.ForwardSpeed = targetSpeed;
			else ucs.ForwardSpeed += dif * ((targetSpeed != 0)? mAccel : nDAccel); //get to idle faster
		}
		*/
		#endregion
	}
}