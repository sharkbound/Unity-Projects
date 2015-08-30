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
	public int LaunchSpeed = 10;
	public Sprite AutoOff;
	public Sprite AutoOn;
	public Sprite AddForceOn;
	public Sprite AddForceOff;  
	public Rigidbody enemy;
	//private variables
	private int timePassed = 0;
	private LineRenderer line;
	private bool DoAuto = false;
	private bool AddForce = false;
	private Image autoImages;
	private Image forceImage;
	//private GameObject spawnPoint;
	// Use this for initialization
	void Start () {
		//spawnPoint = GameObject.Find("Cylinder");

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
		DestroyStuff();
	}
	void DestroyStuff(){
		int destCount = 0;
		if (Input.GetKeyDown(KeyCode.Delete)){
			GameObject[] destroyList = GameObject.FindGameObjectsWithTag("CubeEnemy");
				foreach(GameObject obj in destroyList){
				Destroy(obj.gameObject);
				destCount++;
			}
			Debug.Log("Destroyed items: " + destCount);
			destCount = 0;
		}
	}
	void SpawnEnemy(){
		if (Input.GetKey(KeyCode.Mouse1)){
			Vector3 offset = new Vector3(0, 2, 0);
			Rigidbody clone;
			clone = Instantiate(enemy, cam.transform.position + offset, cam.transform.rotation) as Rigidbody;
			clone.AddForce(transform.forward * LaunchSpeed);
		}
		else if (Input.GetKeyUp(KeyCode.R)){
			Vector3 offset = new Vector3(0, 2, 0);
			Rigidbody clone;
			clone = Instantiate(enemy, transform.position + offset, Quaternion.identity) as Rigidbody;
			clone.AddForce(transform.forward * LaunchSpeed);
		}
	}
	void DestroyTimer(float time){
		Invoke("DisableLine", time);
	}
	void DisableLine(){
		line.enabled = false;
	}
	void ToggleAuto(){
		if (Input.GetKeyDown(KeyCode.M)){
			//Debug.Log("toggled! " + DoAuto);
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
			//Debug.Log("toggled! " + AddForce);
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
			if (line.enabled == true) {
				line.enabled = false;
			}
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
				line.enabled = true;
				if (Physics.Raycast (ray, out hit, 100)) {
					line.SetPosition (1, hit.point);
					hit.transform.SendMessage ("damage", dmg, SendMessageOptions.DontRequireReceiver);
				} else {
					line.SetPosition (1, ray.GetPoint (60));
				}
		   } else {
				line.enabled = true;
				if (Physics.Raycast (ray, out hit, 100)) {
					line.SetPosition (1, hit.point);
					if (hit.rigidbody){
						hit.rigidbody.AddForceAtPosition(transform.forward * force, hit.point);
						Debug.Log("Rigidbody " + hit.transform.gameObject.name + " Hit!");
					}
				} else {
					line.SetPosition (1, ray.GetPoint (60));
				}
			}
	    }
		if (line.enabled == true){
			DestroyTimer(0.05f);
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
			}
		} else {
			if (Input.GetButton ("Fire1")) {
				Ray ray = new Ray (cam.transform.position, cam.transform.forward);
				RaycastHit hit;
				line.SetPosition (0, transform.Find ("laserspawn").position);
				
				if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
					line.SetPosition (1, hit.point);
					if (hit.rigidbody){
						//hit.rigidbody.AddForceAtPosition(transform.forward * AutoForce, hit.point);
						//hit.rigidbody.AddExplosionForce(35f, hit.point, 100f, 10f);
						hit.rigidbody.AddForce(Vector3.up * 1000f);
						float dist = Vector3.Distance(hit.point, gun.position);
						Debug.Log("Rigidbody " + hit.transform.gameObject.name + "Hit! the object is " + dist + " far away from you ");
					}
					//if (timePassed >= DmgDelay) {
					//}
				} else {
					line.SetPosition (1, ray.GetPoint (60));
				}
			}
		}

    }
}
