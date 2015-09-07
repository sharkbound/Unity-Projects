using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {


	[SerializeField]float speed = 10f;
	Vector3 direction = Vector3.zero;
	CharacterController charater;
	// Use this for initialization
	void Start () {
		charater = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		direction = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
	}

	void FixedUpdate(){
		charater.SimpleMove(direction * speed);
	}
}
