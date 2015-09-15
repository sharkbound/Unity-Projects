using UnityEngine;
using System.Collections;

public class FirstToThirdperson : MonoBehaviour {
	bool Locked = true;
	Transform firstPersonCam;
	Transform thirdPersonCam;
	bool thirdPersonEnabled = false;
	bool firstPersonEnabled = true;
	Camera firstPerson;
	Camera thirdPerson;
	AudioListener firstPersonAudio;
	AudioListener thirdPersonAudio;
	PlayerCam firstPersonCameraScript;
	PlayerCam thirdPersonCameraScript;

	// Use this for initialization
	void Start () {
		firstPersonCam = transform.Find("FirstPersonCharacter");
		thirdPersonCam = transform.Find("ThirdPersonCharacter");

		thirdPerson = thirdPersonCam.GetComponent<Camera>();
		firstPerson = firstPersonCam.GetComponent<Camera>();

		firstPersonAudio = firstPersonCam.GetComponent<AudioListener>();
		thirdPersonAudio = thirdPersonCam.GetComponent<AudioListener>();

		firstPersonCameraScript = firstPersonCam.GetComponent<PlayerCam>();
		thirdPersonCameraScript = thirdPersonCam.GetComponent<PlayerCam>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.T)){
			Debug.Log ("press");
			transform.position = GameObject.Find("tele1").transform.position;
		}

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

		if (Input.GetKeyDown(KeyCode.LeftControl)) { 
			if (firstPersonEnabled){
				firstPerson.enabled = false;
				thirdPerson.enabled = true;
				firstPersonAudio.enabled = false;
				thirdPersonAudio.enabled = true;
				firstPersonCameraScript.enabled = false;
				thirdPersonCameraScript.enabled = true;

				firstPersonEnabled = !firstPersonEnabled;
				thirdPersonEnabled = !thirdPersonEnabled;
			}
			else if (thirdPersonEnabled){
				firstPerson.enabled = true;
				thirdPerson.enabled = false;
				firstPersonAudio.enabled = true;
				thirdPersonAudio.enabled = false;
				firstPersonCameraScript.enabled = true;
				thirdPersonCameraScript.enabled = false;

				firstPersonEnabled = !firstPersonEnabled;
				thirdPersonEnabled = !thirdPersonEnabled;
			}

		}
	}
}
