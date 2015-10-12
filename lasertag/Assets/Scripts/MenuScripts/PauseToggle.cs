using UnityEngine;
using System.Collections;

public class PauseToggle : MonoBehaviour {

	public static bool IsPaused = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		TogglePauseBool();
		LockAndUnlockMouse();
	}

	void TogglePauseBool() {
		if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)){
			IsPaused = !IsPaused;
			//Debug.Log("toggled IsPaused to : " + IsPaused);
		}
	}

	void LockAndUnlockMouse(){
	    if (PhotonNetwork.connected) {
			if (!IsPaused) {
				if (Cursor.lockState == CursorLockMode.None && Cursor.visible == true){
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
					Debug.Log("not paused");
				}
			} else if (IsPaused) {
				if (Cursor.lockState == CursorLockMode.Locked && Cursor.visible == false ){
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					Debug.Log("i am paused");
				}
			}
	    }
	}
}
