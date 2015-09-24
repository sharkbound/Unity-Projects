using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {

	public float SelfDestructDelay = 1f;

	// Use this for initialization
	void Update () {
		SelfDestructDelay -= Time.deltaTime;

		if (SelfDestructDelay <= 0) {

			PhotonView PV = GetComponent<PhotonView>();

			if (PV != null && PV.instantiationId != 0) {
				PhotonNetwork.Destroy(gameObject);
			}
			else { 
				Destroy(gameObject);
			}
		}
	}

}
