using UnityEngine;
using System.Collections;

public class cursorLock : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	   
		if (Input.GetKeyUp(KeyCode.Q)) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		if (Input.GetKeyUp(KeyCode.E)) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		if (Input.GetKeyUp(KeyCode.R)) {
			Debug.LogError("lol");
		}
	}
}
