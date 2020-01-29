using UnityEngine;
using System.Collections;

namespace SPLabs.Controllers{

	public class JumpState : BaseState{
		private float mJumpSpeed = 5f,
					mDownRayDist = 0.5f,			//Range to scan downward, has to be at least 0.2
					mGroundedMin = 0.11f;		//Minimum distance to be concidered on the ground.

		private bool 
					mIsDownRayHit = false,
					mIsGrounded = false;

		private int mJumpState = 0; //1:Launch, 2:In air, 3:Landing, 4:grounded, 5:Complete

		private RaycastHit mDownRayHit;


		public JumpState(){
			this.name = "Jump";
			this.addEventTypes(StateEventType.jumpComplete,StateEventType.jumpLaunch);
		}

		#region State Overrides
		public override void HandleEventType(StateEventType evt){
			switch(evt){
				case StateEventType.jumpLaunch: mJumpState = 1; break;
				case StateEventType.JumpGrounded: mJumpState = 4; break;
				case StateEventType.jumpComplete: mJumpState = 5; break;
			}
		}

		public override void OnStateStart(UserControlState ucs){
			//Debug.Log(ucs.ForwardSpeed);
			//Vector3 vel = ucs.RBody.velocity;
			//vel.y += mJumpSpeed;
			//ucs.RBody.velocity = vel;
			//ucs.Anim.SetInteger("JumpState",1);
			mJumpState = 0;
			mIsGrounded = false;
			ucs.Anim.SetBool("Jump",true);
			//ucs.Anim.SetTrigger("Jump");
		}

		public override void OnStateEnd(UserControlState ucs){}

		public override void Update(UserControlState ucs){
			checkGrounded(ucs);

			if(mJumpState == 2 && mIsGrounded && !ucs.Anim.IsInTransition(0)){
				Debug.Log("GROUNDED---------------------------------------------------");
				mJumpState = 3;
				ucs.Anim.SetBool("Jump",false);
			}else if(mJumpState == 5){
				mJumpState = 0;
				ucs.Anim.SetBool("Jump",false);
				StateMan.ChangeState("Grounded",ucs);
			}
		}

		public override void FixedUpdate(UserControlState ucs){
			float y = ucs.RBody.velocity.y;
			if(mJumpState == 1){
				//ucs.RBody.velocity = ucs.Trans.TransformDirection(0,ucs.RBody.velocity.y + (mJumpSpeed ),ucs.ForwardSpeed);
				y += mJumpSpeed;
				mJumpState = 2;
			}else if(mJumpState == 4){
				ucs.Speed.z = 0f;
				//ucs.RBody.velocity = ucs.Trans.TransformDirection(0,ucs.RBody.velocity.y,ucs.ForwardSpeed);
			}

			ucs.RBody.velocity = ucs.Trans.TransformDirection(0,y,ucs.Speed.z);
		}
		#endregion

		private void checkGrounded(UserControlState ucs){
			Vector3 origin = ucs.Trans.position + (Vector3.up * 0.1f);
	 
			#if UNITY_EDITOR
			#endif
			Debug.DrawLine(origin, origin + (Vector3.down * mDownRayDist),Color.red);


			mIsDownRayHit = Physics.Raycast(origin,Vector3.down, out mDownRayHit, mDownRayDist);
			if(mIsDownRayHit){
				mIsGrounded = (mDownRayHit.distance <= mGroundedMin);
				//if(mDownRayHit.distance > mGroundedMin){
					//Debug.Log(mDownRayHit.distance);
					//Debug.Log(mDownRayHit.collider.name);
				//}
			}
		}

	}

}