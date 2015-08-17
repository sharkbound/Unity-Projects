using UnityEngine;
using System.Collections;

public class MoveScript : MonoBehaviour {
	// move speed
	public float Speed = 10.0f;
	public float cameraSpeed = 10.0f;
	//public float JumpSpeed = 6.0f;
	// gravity that effects the player
	public float Gravity = 20.0f;
	//camera rotate speed
	public float RotateSpeed = 6.0f;
	private Vector3 moveDirection = Vector3.zero;

    void Start(){
	}
	// Update is called once per frame
	//is the player on ground?
	void Update(){

		// charater controller variable
		CharacterController controller = GetComponent<CharacterController> ();
		if (controller.isGrounded) {
			//trying to get mouse camera controls working
			//float h = Input.GetAxis("Mouse X") * cameraSpeed;
			//float v =  -Input.GetAxis("Mouse Y") * cameraSpeed;
			//transform.Rotate(v, h, 0);
			//Vector3 cameraRotate = new Vector3(v, h, 0);
			//transform.Find("Camera").transform.Rotate(cameraRotate, Space.World);
             

			//rotate camera
			transform.Rotate (0, Input.GetAxis ("Horizontal") * RotateSpeed, 0);

			//feed move direction with input
			moveDirection = new Vector3 (0, 0, Input.GetAxis ("Vertical"));
			// move in the inputed direction
			moveDirection = transform.TransformDirection (moveDirection);
			//multiply it by speed
			moveDirection *= Speed;
		}
		//Applying gravity to the controller
		moveDirection.y -= Gravity * Time.deltaTime;
	
		//Making the character move
		controller.Move (moveDirection * Time.deltaTime);
	}
}

