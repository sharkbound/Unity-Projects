using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public float HitPoints = 100f;
	public float currentHP;
	private GameObject CrateRespawn;
	// Use this for initialization
	void Start () {
		currentHP = HitPoints;
	
	}
    
	[PunRPC]
    public void TakeDmg(float dmg) {

		currentHP -= dmg;

		if (currentHP <= 0f) {
			Die ();
		}
	}

	void Die(){
		if (GetComponent<PhotonView>().instantiationId == 0) {
			Destroy(gameObject);
		}
		else {
			if ( GetComponent<PhotonView>().isMine ) {
				if ( gameObject.tag == "Player" ){
					GameObject.Find("StandbyCamera").SetActive(true);
					GameObject.FindObjectOfType<NetworkManager>().RespawnTimer = 2.5f;
				}
				PhotonNetwork.Destroy(gameObject);
			}
		}
	}

	void deathMSG(){

		GameObject gameManager = GameObject.Find("_PhotonStuff");
		string playerName = PhotonNetwork.player.name;
		gameManager.GetComponent<NetworkManager>().AddChatMessage(playerName + " has died!");
	}

}
