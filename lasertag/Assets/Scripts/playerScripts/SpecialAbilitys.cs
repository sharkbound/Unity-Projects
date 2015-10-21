using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpecialAbilitys : MonoBehaviour {


	public Rigidbody GrenadePrefab;

	public float teleportCooldown = 2.5f;
	public float GrenadeThrowPower = 10f;

	public string RemainingCooldownMessage = "teleport recharging - ";
	public string SetTeleportPointReady = "Teleport ready! press t to set point";
	public string PointSetAndAbilityReady = "press t to teleport!";

	[SerializeField] float currentTeleportCooldown;

	bool teleportedPointSet = false;

	Vector3 teleportLocation;
	//Vector3 GrenadeSpawnLocation;

	Text abilityCooldownText;

	//GameObject GrenadeSpawnGameobject;

	// Use this for initialization
	void Start () {
		currentTeleportCooldown = teleportCooldown;
		abilityCooldownText = GameObject.Find("TeleportAbilityStatus").GetComponent<Text>();
		abilityCooldownText.enabled = true;
		//GrenadeSpawnGameobject = GetComponentInChildren<GrenadeSpawn>().gameObject;
	}
	
	// Update is called once per frame
	void Update () {

		TeleportAbility();
		ThrowGrenade();
	}

	void ThrowGrenade() {

		if (!PauseToggle.IsPaused && !PauseToggle.IsGrenadeThrown && Input.GetKeyDown(KeyCode.G)) {

			//GrenadeSpawnLocation = GrenadeSpawnGameobject.transform.position;
				
			GameObject clone = PhotonNetwork.Instantiate("Grenade", Camera.main.transform.position, transform.rotation, 0);
			clone.GetComponent<Rigidbody>().velocity = Camera.main.transform.forward * GrenadeThrowPower;
		}
	}

	void TeleportAbility() {

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
