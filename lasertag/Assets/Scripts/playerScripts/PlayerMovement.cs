using UnityEngine;
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
		if (direction.magnitude > 1f){
			direction = direction.normalized;
		}

		if (charater.isGrounded) {
			if (anim.GetBool("Jumping") != false){
				anim.SetBool("Jumping", false);
			}
			if (Input.GetButton("Jump")) {
				verticalVelocity = jumpSpeed;
			}
			else {
			verticalVelocity = 0f;
			}
		}
		else {
			anim.SetBool("Jumping", true);
		}

		anim.SetFloat("Speed", direction.magnitude);

	}

	void FixedUpdate(){
		Vector3 dist = direction * speed * Time.deltaTime;
		verticalVelocity += Physics.gravity.y * Time.deltaTime;

		dist.y = verticalVelocity * Time.deltaTime;

		charater.Move(dist);
	}
}
