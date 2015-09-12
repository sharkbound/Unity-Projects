﻿using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {


	[SerializeField] float speed = 10f;
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
	}

	void FixedUpdate(){

		Vector3 dist = direction * speed * Time.deltaTime;

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
