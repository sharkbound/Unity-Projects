using UnityEngine;
using System.Collections;

public class Turretcollision : MonoBehaviour {

	public Transform explosion;
	// Use this for initialization
	void Start () {
	
	}
	
	void OnTriggerEnter(Collider hit)
	{
		if (hit.gameObject.tag == "playerProjectile") {
			Destroy(hit.gameObject);
			Destroy(gameObject);
		    //Transform exp = Instantiate(explosion, gameObject.transform.position, Quaternion.identity) as Transform;
			//Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
		    //Destroy(exp.gameObject);

		}
	}
	/*
	IEnumerator wait(){

		Object exp = Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
		yield return new WaitForSeconds (2f);
		Destroy (exp);

	}
    */
}
