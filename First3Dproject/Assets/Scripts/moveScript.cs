using UnityEngine;
using System.Collections;

public class moveScript : MonoBehaviour {

	//variables
	
	int onetime = 0;
	//tried counter
	public int trys = 4;
	//getting hit
	public float dieingRotation;
	public float decreaseSpinSpeed;
	public float decayTime;
	public static bool gotHit = false;
	float[] backups = new float[3] {900f, 0.1f, 1.8f};
	// move speed
	public float Speed = 10.0f;
	//public float JumpSpeed = 6.0f;
	// gravity that effects the player
	public float Gravity = 20.0f;
	//camera rotate speed
	public float RotateSpeed = 6.0f;
	//bullet speed
	public float BulletSpeed;
	// bullet prefab and shoot location
	public Rigidbody BulletPrefab;
	public Transform ShootLocation; //location to shoot from

	private Vector3 moveDirection = Vector3.zero;   // vector for storing movement
	public static bool dead = false; // is player dead?
	// Use this for initialization
	void Start () {

	}
	void LateUpdate()
	{
		// if player is dead
		if (dead) {

			if (onetime == 1){
				onetime = 0;
				dead = false;
			}
			//set players position to 4 above center of map
			transform.position = new Vector3(0, 4, 0);
			// make player no longer dead
			trys -= 1;
			if (trys <= 0)
			{
				trys = 4;
				onetime = 0;
			  Application.LoadLevel(0);
			}
			onetime = 1;
		}
		if (gotHit) {
			if(dieingRotation < 1){
				// not hit anymore, reset and get back in the game!
				dieingRotation = backups[0];
				decreaseSpinSpeed = backups[1];
				decayTime = backups[2];
				LifeControl.HITS += 1;
				gotHit = false;
			}
			else {
				// charater hit, spin charater around!
				transform.Rotate(0, dieingRotation * Time.deltaTime,0, Space.World);
				dieingRotation -= decreaseSpinSpeed;
				decreaseSpinSpeed += decayTime;
			}
		}
	}
	// when the controller collides with something
	//void OnControllerColliderHit(ControllerColliderHit hit)
	void OnTriggerEnter(Collider hit)
	{
		// if the object collided with has the tag fallout do this
		if(hit.gameObject.tag == "fallout")
		{
			// subtract 1 life
			LifeControl.LIVES = 0;
			//teleport player to center of map
			dead = true;
		}

		if (hit.gameObject.tag == "enemyProjectile" && gotHit == false) {
			gotHit = true;
			Destroy(hit.gameObject);
		}
	}
	// Update is called once per frame

	void Update () {
	
		// charater controller variable
		CharacterController controller = GetComponent<CharacterController> ();

		// if player presses j
		if (Input.GetKeyDown (KeyCode.J)) {
			// "jump" up 2
			moveDirection = new Vector3(0, 2f, 0);
			//move up 2
			controller.Move(moveDirection);
		}

		//is the player on ground?
		if (controller.isGrounded) {

			//rotate camera
			transform.Rotate(0, -Input.GetAxis("Horizontal") * RotateSpeed, 0);

			//shooting stuff once per key press
			if (Input.GetButtonDown("Jump"))
			{
				// create a copy of the bullet 
				Rigidbody bullet = Instantiate(BulletPrefab,
				                                  GameObject.Find("spawnPoint").transform.position,
				                                  Quaternion.identity) as Rigidbody;
				//give it a tag
				bullet.tag = "playerProjectile";
				// add forward force to the bullet
				bullet.AddForce(ShootLocation.forward * BulletSpeed);
			
			}
			// if player presses m
			if (Input.GetKey(KeyCode.M))
			{
				// create a copy of the bullet
				Rigidbody bullet = Instantiate(BulletPrefab,
				                               GameObject.Find("spawnPoint").transform.position,
				                               Quaternion.identity) as Rigidbody;
				//give it a tag
				bullet.tag = "playerProjectile";
				// add forward force to the bullet
				bullet.AddForce(ShootLocation.forward * BulletSpeed);
			}


			//feed move direction with input
			moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
			// move in the inputed direction
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
