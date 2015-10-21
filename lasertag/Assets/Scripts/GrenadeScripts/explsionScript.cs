using UnityEngine;
using System.Collections;

public class explsionScript : MonoBehaviour {

 	AudioSource AS;

	public float ExplosionDelay = 2.5f;
	public float radius = 10f;
	public float ExplosionDamage = 80f;

	Health health;

	
	// Use this for initialization
	void Start () {
		AS = GetComponent<AudioSource>();
		PauseToggle.IsGrenadeThrown = true;
		Invoke("Explode", ExplosionDelay);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Explode() {

		AS.Play();

		Collider[] hits = Physics.OverlapSphere(transform.position, radius);

		foreach (Collider collider in hits) {

			health = collider.GetComponent<Health>();

			if (health != null){
				health.TakeDmg(ExplosionDamage, "Grenade");
			}
		}

		PauseToggle.IsGrenadeThrown = false;
		Invoke("DestroyGrenade", 0.1f);
	}


	void DestroyGrenade(){
		if (GetComponent<PhotonView>().isMine) {
			PhotonNetwork.Destroy(gameObject);
		}
	}

/*	void OnCollisionEnter(Collision col){

		if (col.transform.tag == "Player" && col.gameObject != NetworkManager.MyPlayer) {

			AS.Play();
			
			Collider[] hits = Physics.OverlapSphere(transform.position, radius);
			
			foreach (Collider collider in hits) {
				
				health = collider.GetComponent<Health>();
				
				if (health != null){
					health.TakeDmg(ExplosionDamage, "Grenade");
				}
			}
			
			PauseToggle.IsGrenadeThrown = false;
			Invoke("DestroyGrenade", 0.1f);
		}
	} */

}
