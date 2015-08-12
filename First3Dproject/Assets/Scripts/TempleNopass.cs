using UnityEngine;
using System.Collections;

public class TempleNopass : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnTriggerEnter(Collider hit)
	{
		if (hit.gameObject.tag == "playerProjectile" || hit.gameObject.tag == "enemyProjectile")
		{
			Destroy (hit.gameObject);
		}

	}
}
