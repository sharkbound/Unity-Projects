using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour {

	public int zoom = 20;
	public int normal = 90;
	public float ZoomSmoothing = 5f;

	bool IsZoomed = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey(KeyCode.Mouse1)) {
		}
	
	}
}
