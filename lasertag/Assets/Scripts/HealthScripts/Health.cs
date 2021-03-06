﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour {

	Text HealthRemainingText;

	public float HitPoints = 100f;
	public float currentHP;

	int FloatCurrentHealthToInt = 100;

	//public static bool PlayerDead = false;

	Health PlayerHealth;


	private GameObject CrateRespawn;

	public string[] deathMessages = new string[7];

	// Use this for initialization
	void Start () {
		setDeathMessages();
		currentHP = HitPoints;
		GetHealthComponent();

	}

	void Update(){
		if (Input.GetKey(KeyCode.L)) {
			TakeDmg(1000, "suicide");
		}

		SetHealthDisplay();
	}

	void GetHealthComponent() {
		if (gameObject.tag == "Player"){
			HealthRemainingText = GameObject.Find("CurrentHealth").GetComponent<Text>();
			PlayerHealth = NetworkManager.MyPlayer.GetComponent<Health>();
		} 
	}

	void SetHealthDisplay() {
		if ( gameObject.tag == "Player" && HealthRemainingText.text != PlayerHealth.currentHP.ToString() ) {
			FloatCurrentHealthToInt = (int)PlayerHealth.currentHP;
			HealthRemainingText.text = "HP: " + FloatCurrentHealthToInt.ToString();
		} 
	}

	void setDeathMessages(){
		deathMessages[0] = " was humiliated by ";
		deathMessages[1] = " died by the hands of ";
		deathMessages[2] = " walked into the gun of ";
		deathMessages[3] = " was trying to make peace with  ";
		deathMessages[4] = " tried to beat ";
		deathMessages[5] = " ate the bullets of ";
		deathMessages[6] = " was completly and utterly demolished by ";
	}
    
	[PunRPC]
    public void TakeDmg(float dmg, string enemyName) {

		currentHP -= dmg;

		if (currentHP <= 0f) {
			Die (enemyName);
		}
	}

	void Die(string enemyName){

		if (GetComponent<PhotonView>().instantiationId == 0) {
			Destroy(gameObject);
		}
		else {
			if ( GetComponent<PhotonView>().isMine ) {
				if ( gameObject.tag == "Player" ){
					deathMSG(enemyName);
					GameObject.Find("TeleportAbilityStatus").GetComponent<Text>().enabled = false;
					GameObject.Find("CurrentHealth").GetComponent<Text>().enabled = false;
					GameObject.Find("StandbyCamera").GetComponent<Camera>().enabled = true;
					GameObject.FindObjectOfType<NetworkManager>().RespawnTimer = 2.5f;
				}
				PhotonNetwork.Destroy(gameObject);
			}
		}
	}

	void deathMSG(string enemyName){

		GameObject gameManager = GameObject.Find("_PhotonStuff");
		string playerName = PhotonNetwork.player.name;
		string randomDeathMSG = deathMessages[Random.Range(0, deathMessages.Length)] ;
		gameManager.GetComponent<NetworkManager>().AddChatMessage(playerName + randomDeathMSG + enemyName);
	}

}
