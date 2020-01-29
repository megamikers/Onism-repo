using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace SPLabs.Controllers{
	public enum StateEventType : int{
		jumpLaunch=1, JumpGrounded=2, jumpComplete=3
	}

	public class UserControlState{
		public float		VertAxis = 0f,
							HoriAxis = 0f;

		public Vector3		Speed = Vector3.zero;
		
		public Transform	Trans = null;
		public Animator		Anim = null;
		public Rigidbody	RBody = null;

		public UserControlState(Transform t){
			Trans	= t;
			Anim	= t.GetComponent<Animator>();
			RBody	= t.GetComponent<Rigidbody>();
		}

		public void InputUpdate(){
			VertAxis = Input.GetAxis("Vertical");
			HoriAxis = Input.GetAxis("Horizontal");
		}
	}

	public class BaseState{
		public string name{ get; protected set; }

		protected StateManager StateMan = null;
		private List<StateEventType> mEventTypes = new List<StateEventType>();

		public void setStateManager(StateManager sm){ StateMan = sm; }
		public void addEventTypes(params StateEventType[] evt){ for(int i=0; i < evt.Length; i++) mEventTypes.Add(evt[i]); }

		public virtual void HandleEventType(StateEventType evt){ Debug.Log("Handle Event " + evt.ToString()); }
		public virtual void Update(UserControlState ucs){ Debug.Log("Update " + name); }
		public virtual void FixedUpdate(UserControlState ucs){ Debug.Log("FixedUpdate " + name); }
		public virtual void OnStateStart(UserControlState ucs){ Debug.Log("onStateStart " + name); }
		public virtual void OnStateEnd(UserControlState ucs){ Debug.Log("onStateEnd " + name); }
	}


	public class StateManager : MonoBehaviour{
		private Dictionary<string,BaseState> mStateList = new Dictionary<string,BaseState>();
		public BaseState ActiveState { get; private set; }

		public void onAnimationEvent(int evt){
			Debug.Log("StateManager.OnAnimationEvent" + evt);
			if(ActiveState != null) ActiveState.HandleEventType((StateEventType) evt);
		}

		//Managing State Data
		public void AddState(params BaseState[] states){
			for(int i=0; i < states.Length; i++){
				states[i].setStateManager(this);
				mStateList.Add(states[i].name,states[i]);
			} 
		}

		public bool ChangeState(string sName,UserControlState ucs){
			if(!mStateList.ContainsKey(sName)) return false;

			if(ActiveState != null) ActiveState.OnStateEnd(ucs);

			ActiveState = mStateList[sName];
			ActiveState.OnStateStart(ucs);

			return true;
		}
		
	}

}
