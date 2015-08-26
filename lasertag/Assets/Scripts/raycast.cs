using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class raycast : MonoBehaviour {

	// public variables
	public Camera cam;
	public Transform gun;
	public int AutoDmg = 5;
	public int dmg = 20;
	public int DmgDelay = 60;
	public int force = 5;
	public int AutoForce = 5;
	public Sprite AutoOff;
	public Sprite AutoOn;
	public Sprite AddForceOn;
	public Sprite AddForceOff;  
	public Transform enemy;
	//private variables
	private int timePassed = 0;
	private LineRenderer line;
	private bool DoAuto = false;
	private bool AddForce = false;
	private Image autoImages;
	private Image forceImage;
	private GameObject spawnPoint;
	// Use this for initialization
	void Start () {
		
		spawnPoint = GameObject.Find("Cylinder");
		forceImage = GameObject.Find("AddForceImage").GetComponentInChildren<Image>();
		autoImages = GameObject.Find("AutoImage").GetComponentInChildren<Image>();
		line = GameObject.Find("laserspawn").gameObject.GetComponent<LineRenderer>();
		line.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		ToggleAuto ();
		ToggleForce ();
		fireSelection ();
		SpawnEnemy();

	}
	void SpawnEnemy(){
		if (Input.GetKeyDown(KeyCode.R)){
			Instantiate(enemy, spawnPoint.transform.forward, Quaternion.identity);
		}
	}
	void DisableLine(){
		line.enabled = false;
	}
	void ToggleAuto(){
		if (Input.GetKeyDown(KeyCode.M)){
			Debug.Log("toggled! " + DoAuto);
			DoAuto = !DoAuto;
			if (!DoAuto){
				autoImages.sprite = AutoOff;
			} else {
				autoImages.sprite = AutoOn;
			}
		}
	}
	void ToggleForce(){
		if (Input.GetKeyDown(KeyCode.N)){
			Debug.Log("toggled! " + AddForce);
			AddForce = !AddForce;
			if (!AddForce){
				forceImage.sprite = AddForceOff;
			} else {
				forceImage.sprite = AddForceOn;
			}

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
			if (!AddForce)
			{
				if (Physics.Raycast (ray, out hit, 100)) {
					line.enabled = true;
					line.SetPosition (1, hit.point);
					Invoke ("DisableLine", 0.1f);
					hit.transform.SendMessage ("damage", dmg, SendMessageOptions.DontRequireReceiver);
				//Destroy(hit.transform.gameObject);
				} else {
					line.enabled = true;
					line.SetPosition (1, ray.GetPoint (60));
					Invoke ("DisableLine", 0.1f);
				}
		   } else {
				if (Physics.Raycast (ray, out hit, 100)) {
					line.enabled = true;
					line.SetPosition (1, hit.point);
					if (hit.rigidbody){
						hit.rigidbody.AddForceAtPosition(transform.forward * force, hit.point);
						Debug.Log("Rigidbody " + hit.transform.gameObject.name + " Hit!");
					}
					Invoke ("DisableLine", 0.1f);
					//Destroy(hit.transform.gameObject);
				} else {
					line.enabled = true;
					line.SetPosition (1, ray.GetPoint (60));
					Invoke ("DisableLine", 0.1f);
				}
			}
	    }
	}
	void fireLaserRapid()
	{
		timePassed++;

		line.enabled = true;
		if (!AddForce) {
			if (Input.GetButton ("Fire1")) {
				Ray ray = new Ray (cam.transform.position, cam.transform.forward);
				RaycastHit hit;
				line.SetPosition (0, transform.Find ("laserspawn").position);
				
				if (Physics.Raycast (ray, out hit, 100)) {
					line.SetPosition (1, hit.point);
					if (timePassed >= DmgDelay) {
						hit.transform.SendMessage ("damage", AutoDmg, SendMessageOptions.DontRequireReceiver);
						timePassed = 0;
					}
				} else {
					line.SetPosition (1, ray.GetPoint (60));
				}
			} else {
				line.enabled = false; 
			}
		} else {
			if (Input.GetButton ("Fire1")) {
				Ray ray = new Ray (cam.transform.position, cam.transform.forward);
				RaycastHit hit;
				line.SetPosition (0, transform.Find ("laserspawn").position);
				
				if (Physics.Raycast (ray, out hit, 100)) {
					line.SetPosition (1, hit.point);
					if (hit.rigidbody){
						hit.rigidbody.AddForceAtPosition(transform.forward * AutoForce, hit.point);
						Debug.Log("Rigidbody " + hit.transform.gameObject.name + " Hit!");
					}
					//if (timePassed >= DmgDelay) {
					//}
				} else {
					line.SetPosition (1, ray.GetPoint (60));
				}
			} else {
				line.enabled = false; 
			}
		}

    }
}
