using UnityEngine;
using System.Collections;

public class TurretControl : MonoBehaviour {

	public Transform LookAtTarget;
	public float RotationDamping;
	public Rigidbody Projectile;
	public float BulletSpeed;
	private int seconds;

	// Use this for initialization
	void Start () {
	
	}


	// Update is called once per frame
	void Update () {
		if(LookAtTarget)
		{

			var rotate = Quaternion.LookRotation(LookAtTarget.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * RotationDamping);

			seconds++;
			if(seconds >61)  // when counter is more than 61 it will reset to 0
			{
				seconds = 0;
			}
			if(seconds > 60) // when counter is 60 it will shoot
			{
				shoot ();
			}

		}

	}
	void shoot()
	{
		Rigidbody bullet = Instantiate(Projectile,
		                               transform.Find("TurretBarrelEnd").transform.position,
		                               Quaternion.identity) as Rigidbody;
		bullet.AddForce (transform.forward * BulletSpeed);

	}
}
