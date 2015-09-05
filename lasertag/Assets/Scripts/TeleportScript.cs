using UnityEngine;
using System.Collections;

public class TeleportScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	   if (Input.GetKeyDown(KeyCode.T)){
			Debug.Log ("press");
			transform.position = GameObject.Find("tele1").transform.position;
		}
	}
}
