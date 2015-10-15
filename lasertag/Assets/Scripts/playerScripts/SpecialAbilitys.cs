using UnityEngine;
using System.Collections;

public class SpecialAbilitys : MonoBehaviour {

	public float teleportCooldown = 2.5f;
	[SerializeField] float currentTeleportCooldown;
	//[SerializeField] float setTeleportPointCooldown;

	bool teleportedPointSet = false;

	Vector3 teleportLocation;

	// Use this for initialization
	void Start () {
		currentTeleportCooldown = teleportCooldown;
		//setTeleportPointCooldown = teleportCooldown;
	}
	
	// Update is called once per frame
	void Update () {

		//setTeleportPointCooldown -= Time.deltaTime;

	/*	if (setTeleportPointCooldown <= 0) {

			teleportLocation = transform.position;
			setTeleportPointCooldown = teleportCooldown;
		}  */

		currentTeleportCooldown -= Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.T) && currentTeleportCooldown <= 0f) {

			if (teleportedPointSet) {
				transform.position = teleportLocation;
				currentTeleportCooldown = teleportCooldown;
				teleportedPointSet = false;
			}
			else {
				teleportLocation = transform.position;
				teleportedPointSet = true;
			}
		}
	}
}
