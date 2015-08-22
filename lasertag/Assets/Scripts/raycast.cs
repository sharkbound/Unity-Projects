using UnityEngine;
using System.Collections;

public class raycast : MonoBehaviour {

	public Camera cam;
	public Transform gun;
	public int AutoDmg = 5;
	public int dmg = 20;
	public int DmgDelay = 60;
	private int timePassed = 0;
	private LineRenderer line;
	private bool DoAuto = false;
	// Use this for initialization
	void Start () {
		line = GameObject.Find("laserspawn").gameObject.GetComponent<LineRenderer>();
		line.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		ToggleAuto ();
		fireSelection ();
		if (Input.GetKeyDown(KeyCode.R)){
		//	GameObject.Find("enemy").SetActive(true);

		}
	}
	void DisableLine(){
		line.enabled = false;
	}
	void ToggleAuto(){
		if (Input.GetKeyDown(KeyCode.M)){
			Debug.Log("toggled! " + DoAuto);
			DoAuto = !DoAuto;
		}
	}
	void fireSelection(){
		if (Input.GetButton ("Fire1")) {
			if (DoAuto) {
				fireLaserRapid ();
			} else if (!DoAuto) {
				fireLaserOnce ();
			}
		} else {
			line.enabled = false;
		}
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
				Invoke("DisableLine", 0.1f);
				hit.transform.SendMessage("damage", dmg, SendMessageOptions.DontRequireReceiver);
				//Destroy(hit.transform.gameObject);
			} else {
				line.enabled = true;
				line.SetPosition (1, ray.GetPoint (60));
				Invoke("DisableLine", 0.1f);
			}
		}
	}
	void fireLaserRapid()
	{
		timePassed++;

		line.enabled = true;
		if (Input.GetButton ("Fire1")) {
			Ray ray = new Ray (cam.transform.position, cam.transform.forward);
			RaycastHit hit;
			line.SetPosition (0, transform.Find ("laserspawn").position);
			
			if (Physics.Raycast (ray, out hit, 100)) {
				line.SetPosition (1, hit.point);
				if (timePassed >= DmgDelay){
					hit.transform.SendMessage("damage", AutoDmg, SendMessageOptions.DontRequireReceiver);
					timePassed = 0;
				}
			} else {
				line.SetPosition (1, ray.GetPoint (60));
			}
		} else {
			line.enabled = false; 
		}
	}
}
