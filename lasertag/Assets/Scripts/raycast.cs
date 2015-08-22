using UnityEngine;
using System.Collections;

public class raycast : MonoBehaviour {

	public Camera cam;
	public Transform gun;
	private LineRenderer line;
	// Use this for initialization
	void Start () {
		line = GameObject.Find("laserspawn").gameObject.GetComponent<LineRenderer>();
		line.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
			fireLaserOnce ();
	}
	void DisableLine(){
		line.enabled = false;
	}
	void fireLaserOnce()
	{
		Ray ray = new Ray (cam.transform.position, cam.transform.forward);
		RaycastHit hit;
		if (Input.GetButtonDown ("Fire1")) {
			line.SetPosition (0, transform.Find ("laserspawn").position);
			
			if (Physics.Raycast (ray, out hit, 100)) {
				line.enabled = true;
				line.SetPosition (1, hit.point);
				//Invoke("DisableLine", 2);
			} else {
				line.enabled = true;
				line.SetPosition (1, ray.GetPoint (60));
				//Invoke("DisableLine", 2);
			}
		} else {
			line.enabled = false;
		}
	}
}
