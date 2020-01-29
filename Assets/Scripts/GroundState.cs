using UnityEngine;
using System.Collections;

namespace SPLabs.Controllers{
	public class GroundedState : BaseState{
		#region Vars and Constructor

		private float	mCrouchSpeed = 2.635f,
						mCrouchSpeedB = -2.635f,
						mWalkSpeed	= 1.6f,				//How fast does the character walk.
						mBackSpeed	= -1.061f,
						mRunSpeed	= 3.625f,
						mTurnSpeed	= 100f,				//How fast does the character turn
						mAccel		= 0.07f,			//Accelerating Rate
						nDAccel		= 0.2f;				//De-Accelerating Rate

		private bool	mIsCrouch = false,
						mIsFastMode = false;

		public GroundedState(){ this.name = "Grounded"; }
		#endregion

		#region State Overrides
		public override void OnStateStart(UserControlState ucs){
			ucs.RBody.useGravity = true;
		}

		public override void OnStateEnd(UserControlState ucs){
			if(mIsCrouch){
				mIsCrouch = false;
				ucs.Anim.SetBool("Crouch",mIsCrouch);
			}
		}

		public override void Update(UserControlState ucs){
			//.........................................................
			//Check for specific inputs that only matters during the grounded state.
			if(Input.GetKeyDown(KeyCode.F)) mIsFastMode = !mIsFastMode;

			if(Input.GetKeyDown(KeyCode.C)){
				mIsCrouch = !mIsCrouch;
				ucs.Anim.SetBool("Crouch",mIsCrouch);
			}

			if(Input.GetAxisRaw("Jump") == 1){
				StateMan.ChangeState("Jump",ucs);
				return;
			}

			//.........................................................
			//Perform Turning
			if(Mathf.Abs(ucs.HoriAxis) > 0.1) ucs.Trans.rotation *= Quaternion.AngleAxis(mTurnSpeed * ucs.HoriAxis * Time.deltaTime,Vector3.up);
		}

		public override void FixedUpdate(UserControlState ucs){
			//Determine Speed
			Vector3 vMove = new Vector3(0,ucs.RBody.velocity.y,0); //Gravity will change the body's Y velocity, so when applying forward movement need to keep current y
			calcSpeed(ucs);
			vMove.z = ucs.Speed.z; 

			//Set Animations
			ucs.Anim.SetFloat("Speed",ucs.Speed.z);

			//convert vect3 to world space, so it takes rotation into account when dealing with the direction
			ucs.RBody.velocity = ucs.Trans.TransformDirection(vMove);
		}
		#endregion

		#region Helper functions
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
			dif = targetSpeed - ucs.Speed.z;
			if(Mathf.Abs(dif) <= 0.1f) ucs.Speed.z = targetSpeed;
			else ucs.Speed.z += dif * ((targetSpeed != 0)? mAccel : nDAccel); //get to idle faster
		}
		#endregion
	}
}