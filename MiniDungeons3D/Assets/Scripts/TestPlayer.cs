using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
	//movement vars
	private Vector3 curPos;
	public Vector3 targetPos;
	private bool moving = false;
	public float moveSpeed = 0.75f;
	private float t = 0;

	//rotation vars
	public Quaternion targetRot;
	private bool turning = false;
	public float turnSpeed = 0.75f;


    // Update is called once per frame
    void Update()
    {	
    	//moving position
    	if(moving){
    		t += Time.deltaTime/moveSpeed;
    		transform.position = Vector3.MoveTowards(curPos, targetPos, t);
			if(Vector3.Distance(transform.position, targetPos) < 0.1){
				transform.position = targetPos;
				moving = false;
			}
		}
		//turning angle
		else if(turning){
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
			if(Quaternion.Angle(transform.rotation, targetRot) < 2){
				transform.rotation = targetRot;
				turning = false;
			}
		}
		//allow new movement
		else{
			if(Input.GetKeyDown(KeyCode.UpArrow)){
				MoveForward();
			}else if(Input.GetKeyDown(KeyCode.DownArrow)){
				MoveBackward();
			}else if(Input.GetKeyDown(KeyCode.LeftArrow)){
				TurnLeft();
			}else if(Input.GetKeyDown(KeyCode.RightArrow)){
				TurnRight();
			}
		}

    }

    // HELLA basic movement controls
    void MoveForward(){
    	t = 0;
    	curPos = transform.position;
    	targetPos = transform.position - transform.forward*1.0f;
    	moving = true;
    }	

    void MoveBackward(){
    	t = 0;
		curPos = transform.position;
    	targetPos = transform.position + transform.forward*1.0f;
    	moving = true;
    }

    void TurnRight(){
    	targetRot = transform.rotation * Quaternion.Euler(0,90,0);
    	turning = true;
    }

    void TurnLeft(){
    	targetRot = transform.rotation*Quaternion.Euler(0,-90,0);
    	turning = true;
    }
}
