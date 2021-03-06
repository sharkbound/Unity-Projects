﻿using UnityEngine;
using System.Collections;

public class FXManager : MonoBehaviour {

	//public GameObject PhotonScriptsGameobject;
	//PhotonView PV;
	public Transform TempGameObject;
	public AudioClip SniperBulletFXAudio;

	public GameObject SniperFXCompletePrefab;
	GameObject SniperFX;
	LineRenderer LR;

	Transform Audio;
	AudioSource AS;

	//WeaponData wd;

	void Start() {
		//Audio = Instantiate(TempAudioSource);
		//AS = Audio.GetComponent<AudioSource>();
	}

	[PunRPC]
	public void SniperBulletFX(Vector3 startPos, Vector3 endPos) {
		//wd = GameObject.FindObjectOfType<WeaponData>();

		//Debug.Log("SniperBulletFX");
		//CreateTempAudioSource(startPos);
		if (SniperFXCompletePrefab != null) {
			SniperFX = (GameObject)Instantiate(SniperFXCompletePrefab, startPos, Quaternion.LookRotation(endPos - startPos)); // Quaternion.LookRotation(endPos - startPos)
			LR = SniperFX.transform.Find("LineFX").GetComponent<LineRenderer>();
			if (LR != null) {
				LR.SetPosition(0, startPos);
				LR.SetPosition(1, endPos);
			}
			else {
				Debug.LogError("Line Renderer is missing!");
			}
		}
		else {
			Debug.LogError("SniperFXCompletePrefab is missing!");
		}

	}

	void CreateTempAudioSource(Vector3 startPos) {
		Audio = Instantiate(TempGameObject);
		
		Audio.position = startPos;
		
		AS = Audio.gameObject.AddComponent<AudioSource>();
		AS.spatialBlend = 1;
		AS.PlayOneShot(SniperBulletFXAudio);
		
		Destroy(Audio.gameObject, SniperBulletFXAudio.length + 0.1f);
	}
}
