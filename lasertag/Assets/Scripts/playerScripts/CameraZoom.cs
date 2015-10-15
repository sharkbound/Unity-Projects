using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour {

	public float zoom = 20;
	public float normal = 90;

	public float currentZoom = 0f;

	bool IsZoomed = false;

	// Use this for initialization
	void Start () {
		//normal = Camera.main.fieldOfView;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey(KeyCode.Mouse1)) {
			IsZoomed = true;
		} else {
			IsZoomed = false;
		}

		if (IsZoomed) {
			currentZoom = zoom;
			Camera.main.fieldOfView = currentZoom;
		}
		else {
			currentZoom = normal;
			Camera.main.fieldOfView = currentZoom;
		}
	
	}
}
