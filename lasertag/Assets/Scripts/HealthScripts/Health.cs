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
			if (PhotonNetwork.isMasterClient) {
				PhotonNetwork.Destroy(gameObject);

			}
		}
	}

}
