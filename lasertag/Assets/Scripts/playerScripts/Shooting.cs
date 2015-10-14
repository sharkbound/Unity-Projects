using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour {

	public float MaxRayDist = 100f;
	//public float Damage = 25f;
	float randomDmg = 0f;
	float cooldown = 0f;
	RaycastHit hitinfo;
	Ray ray;
	FXManager fxManager;
	PhotonView fxManagerPV;
	WeaponData wd;

	void Start() {
		fxManager = GameObject.FindObjectOfType<FXManager>();

		if (fxManager != null) {
			fxManagerPV = fxManager.GetComponent<PhotonView>();
		}

		if (fxManager == null) {
			Debug.LogError("could not find a FXManager");
		}

		wd = gameObject.GetComponentInChildren<WeaponData>();
		if (wd == null) {
			Debug.LogError("could not find WeaponData component!");
		}
	}

	// Update is called once per frame
	void Update () {

		cooldown -= Time.deltaTime;
	
		// if player presses left mouse button
		if (Input.GetButton("Fire1")){
			fire();
		}
	}


	void fire(){

		if (PauseToggle.IsPaused) {
			return;
		}

		if (wd == null) {
			wd = gameObject.GetComponentInChildren<WeaponData>();
			if (wd == null) {
				Debug.LogError("could not find WeaponData component!");
				return;
			}
		}

		if (cooldown > 0){
			return;
		}

		ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward); 
		Transform hitTransform;
		Vector3 hitPoint;

		hitTransform = FindClosestHitObject(ray, out hitPoint);

		if(hitTransform != null) {

			//Debug.Log ("we hit - " + hitTransform.transform.name);
			Health h = hitTransform.GetComponent<Health>();

			while (h == null && hitTransform.parent) {
				hitTransform = hitTransform.parent;
				h = hitTransform.GetComponent<Health>();
			}

			if (h != null) {
				// this is the network equivilent of calling
				//  h.TakeDmg(Damage);
				PhotonView pv = h.GetComponent<PhotonView>();
				if (pv == null) {
					Debug.LogError("PhotonView not found");
				}
				else {
					if (wd.IsAuto) {
						randomDmg = Random.Range(wd.AutoMinDamage, wd.AutoMaxDamage);
					} else {
						randomDmg = Random.Range(wd.MinDamage, wd.MaxDamage);
					}
					//Debug.LogWarning("The random dmg value is: " + randomDmg);
					h.GetComponent<PhotonView>().RPC("TakeDmg", PhotonTargets.AllBuffered, randomDmg, 
					                                 PhotonNetwork.player.name);
					//Debug.LogWarning(h.currentHP);

				}
			}
			if (fxManager != null) {
				DoGunFX(hitPoint);
			}
		}
		else {
			// we did not hit anything except empty space, but do visual fx anyway
			if (fxManager != null) {
				hitPoint = Camera.main.transform.position + (Camera.main.transform.forward * 100f);
				DoGunFX(hitPoint);
				//fxManagerPV.RPC("SniperBulletFX", PhotonTargets.All, Camera.main.transform.position, hitPoint);
			}
		}
		if (wd.IsAuto) {
			cooldown = wd.AutoFireRate;
		} else {
			cooldown = wd.FireRate;
		}
	}

	void DoGunFX(Vector3 hitpoint){
		fxManagerPV.RPC("SniperBulletFX", PhotonTargets.All, wd.transform.position, hitpoint);
	}

	Transform FindClosestHitObject(Ray raycast, out Vector3 hitPoint) {
	
		Transform closestHit = null;
		float distance = 0f;
		hitPoint = Vector3.zero;

		RaycastHit[] hits =	Physics.RaycastAll(ray, MaxRayDist);
		foreach(RaycastHit rayhit in hits) {
			if (rayhit.transform != this.transform && (closestHit == null || rayhit.distance < distance)) {
				// we have hit something that is :
				// a) not us
				// b) the first object we hit that isnt us
				// c) or, if not b) is at least closer than the previous thing
				closestHit = rayhit.transform;
				distance = rayhit.distance;
				hitPoint = rayhit.point;
			}
		}
		//closest hit is now ether null or it contains the closest hit
		return closestHit;
	}
}
