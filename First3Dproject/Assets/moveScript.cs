using UnityEngine;
using System.Collections;

public class moveScript : MonoBehaviour {

	//variables
	public float Speed = 10.0f;
	public float JumpSpeed = 6.0f;
	public float Gravity = 20.0f;
	public float RotateSpeed = 6.0f;
	public float BulletSpeed;
	public Rigidbody BulletPrefab;
	public Transform ShootLocation;
	private Vector3 moveDirection = Vector3.zero;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		CharacterController controller = GetComponent<CharacterController> ();

		//is the player on ground?
		if (controller.isGrounded) {

			//rotate camera
			transform.Rotate(0, -Input.GetAxis("Horizontal") * RotateSpeed, 0);

			//shooting stuff once per key press
			if (Input.GetButtonDown("Jump"))
			{
				Rigidbody bullet = Instantiate(BulletPrefab,
				                                  GameObject.Find("spawnPoint").transform.position,
				                                  Quaternion.identity) as Rigidbody;
				bullet.AddForce(ShootLocation.forward * BulletSpeed);
			
			}
			if (Input.GetKey(KeyCode.M))
			{
				Rigidbody bullet = Instantiate(BulletPrefab,
				                               GameObject.Find("spawnPoint").transform.position,
				                               Quaternion.identity) as Rigidbody;
				bullet.AddForce(ShootLocation.forward * BulletSpeed);
			}


			//feed move direction with input
			moveDirection = new Vector3(0, 0, -Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			//multiply it by speed
			moveDirection *= Speed;
		}
		//Applying gravity to the controller
		moveDirection.y -= Gravity * Time.deltaTime;

		//Making the character move
		 controller.Move(moveDirection * Time.deltaTime);
	}
}
