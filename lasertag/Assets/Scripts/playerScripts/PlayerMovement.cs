using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {


	[SerializeField] float speed = 10f;
	public float runSpeed = 15f;
	Vector3 direction = Vector3.zero;  // forward back and left right
	float verticalVelocity = 0f;
	[SerializeField] float jumpSpeed = 20f;
	CharacterController charater;
	Animator anim;
	CapsuleCollider capCol;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		charater = GetComponent<CharacterController>();
		//capCol = GetComponent<CapsuleCollider>();
	}
	
	// Update is called once per frame
	void Update () {

		// if the game is paused do nothing / dont move
		if (PauseToggle.IsPaused) {
			return;
		}

		// WASD forward/back is stored in direction
		direction = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, 
		                                             Input.GetAxis("Vertical"));
		// this makes sure you dont move faster if moving diaganlu
		if (direction.magnitude > 1f){
			direction = direction.normalized;
		}

		// set the speed float to the vector3 lenght of the direction vector
		anim.SetFloat("Speed", direction.magnitude);

		if (charater.isGrounded && Input.GetButton("Jump")) {
			verticalVelocity = jumpSpeed;
		}

		adjustAimAngle();
	}

	void adjustAimAngle() { 
		Camera myCamera = this.GetComponentInChildren<Camera>();

		if (myCamera == null) {
			Debug.LogError("Apperently i am missing a camera...");
			return;
		}

		float AimAngle = 0f;
		if (myCamera.transform.rotation.eulerAngles.x <= 90) {
			AimAngle = -myCamera.transform.rotation.eulerAngles.x;
		}
		else {
			AimAngle = 360 - myCamera.transform.rotation.eulerAngles.x;
		}

		anim.SetFloat("AimAngle", AimAngle);

	}

	void FixedUpdate(){
		Vector3 dist = Vector3.zero;
		if (Input.GetKey(KeyCode.LeftShift)) {
			dist = direction * runSpeed * Time.deltaTime;
		}
		else {
			dist = direction * speed * Time.deltaTime;
		}
		// check if character is grounded and verticalvelocity is negative
		if (charater.isGrounded && verticalVelocity < 0) {

			//set jumping bool to false
			anim.SetBool("Jumping", false);

			// apply gravity 
			verticalVelocity = Physics.gravity.y * Time.deltaTime;
		} 
		else {

			if (Mathf.Abs(verticalVelocity) > jumpSpeed*0.75f) {
				anim.SetBool("Jumping",true);
			}

			// apply gravity 
			verticalVelocity += Physics.gravity.y * Time.deltaTime;
		}

		dist.y = verticalVelocity * Time.deltaTime;

		// move the character
		charater.Move(dist);
	}
}
