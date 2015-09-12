using UnityEngine;
using System.Collections;

public class cursorLock : MonoBehaviour {

	bool Locked = true;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.E)) {
			if (Locked){
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				Locked = !Locked;
			}
			else {
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				Locked = !Locked;
			}
		}
	}
}

