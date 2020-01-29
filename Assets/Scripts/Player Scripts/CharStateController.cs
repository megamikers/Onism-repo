using UnityEngine;
using System.Collections;
using SPLabs.Controllers;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class CharStateController : StateManager{
	private UserControlState UCState;

	void Awake(){
		UCState = new UserControlState(transform);
		this.AddState(new GroundedState(),new JumpState(),new FlyingState());
		this.ChangeState("Grounded",UCState);
	}

	void Start(){}

	void Update(){
		//Input update
		UCState.InputUpdate(); 

		if(Input.GetKeyUp(KeyCode.X)){
			if(ActiveState.name == "Flying") this.ChangeState("Grounded",UCState);
			else this.ChangeState("Flying",UCState);
		}

		//Handle State
		ActiveState.Update(UCState);
	}

	void FixedUpdate(){
		ActiveState.FixedUpdate(UCState);
	}

}
