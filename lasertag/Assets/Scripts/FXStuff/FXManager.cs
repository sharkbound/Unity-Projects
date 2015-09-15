using UnityEngine;
using System.Collections;

public class FXManager : MonoBehaviour {

	public AudioClip SniperBulletFXAudio;
	//AudioSource AS;

	void Start() {
		//AS = GameObject.FindObjectOfType<PlayerMovement>
	}

	[PunRPC]
	public void SniperBulletFX(Vector3 startPos, Vector3 endPos) {
		Debug.Log("SniperBulletFX");

		AudioSource.PlayClipAtPoint(SniperBulletFXAudio, startPos);
	}
}
