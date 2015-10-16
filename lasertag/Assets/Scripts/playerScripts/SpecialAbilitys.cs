using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpecialAbilitys : MonoBehaviour {

	public float teleportCooldown = 2.5f;

	public string RemainingCooldownMessage = "teleport recharging - ";
	public string SetTeleportPointReady = "Teleport ready! press t to set point";
	public string PointSetAndAbilityReady = "press t to teleport!";

	[SerializeField] float currentTeleportCooldown;

	bool teleportedPointSet = false;

	Vector3 teleportLocation;

	Text abilityCooldownText;

	// Use this for initialization
	void Start () {
		currentTeleportCooldown = teleportCooldown;
		abilityCooldownText = GameObject.Find("TeleportAbilityStatus").GetComponent<Text>();
		abilityCooldownText.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		currentTeleportCooldown -= Time.deltaTime;

		if ( currentTeleportCooldown >= 0f ) {
			abilityCooldownText.text = RemainingCooldownMessage + currentTeleportCooldown.ToString("#.#");
		} 
		else if ( currentTeleportCooldown <= 0 && teleportedPointSet == false) {
			abilityCooldownText.text = SetTeleportPointReady;
		}

		if ( Input.GetKeyDown(KeyCode.T) && currentTeleportCooldown <= 0f ) {

			if (teleportedPointSet) {
				transform.position = teleportLocation;
				currentTeleportCooldown = teleportCooldown;
				teleportedPointSet = false;
			}
			else {
				abilityCooldownText.text = PointSetAndAbilityReady;
				teleportLocation = transform.position;
				teleportedPointSet = true;
			}
		}
	}
}
