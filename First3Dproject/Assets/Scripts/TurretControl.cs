using UnityEngine;
using System.Collections;

public class TurretControl : MonoBehaviour {

	public Transform LookAtTarget;
	public float RotationDamping;
	public Rigidbody Projectile;
	public float BulletSpeed;
	private int seconds;
	//private float fireDelay = 30f;

	// Use this for initialization
	void Start () {
	
	}


	// Update is called once per frame
	void Update () {
		if(LookAtTarget)
		{

			RaycastHit hit;
			Ray ray = new Ray (transform.Find("TurretBarrelEnd").transform.position, transform.Find("TurretBarrelEnd").transform.forward);


			seconds++;
		    if ((Physics.Raycast(ray, out hit, 30)))
			{
				if (hit.collider.tag == "Player")
				{
					if(seconds > 111)  // when counter is more than 61 it will reset to 0
					{
						seconds = 0;
					}
					if(seconds > 110) // when counter is 60 it will shoot
					{
						shoot ();
					}
				}
			}
            
			Debug.DrawRay(transform.Find("TurretBarrelEnd").transform.position,transform.Find("TurretBarrelEnd").transform.forward);
			//Debug.DrawRay(transform.position,transform.forward);
            
			var rotate = Quaternion.LookRotation(LookAtTarget.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * RotationDamping);
			/*
			if(seconds >111)  // when counter is more than 61 it will reset to 0
			{
				seconds = 0;
			}
			if(seconds > 110) // when counter is 60 it will shoot
			{
				shoot ();
			}
             */
            
		}

	}
	void shoot()
	{
		Rigidbody bullet = Instantiate(Projectile,
		                               transform.Find("TurretBarrelEnd").transform.position,
		                               Quaternion.identity) as Rigidbody;
		bullet.gameObject.tag = "enemyProjectile";
		bullet.AddForce (transform.forward * BulletSpeed);

	}
}
