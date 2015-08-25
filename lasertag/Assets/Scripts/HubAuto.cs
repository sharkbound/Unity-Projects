using UnityEngine;
using System.Collections;

public class HubAuto : MonoBehaviour {
	
	private bool DoAuto = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void ToggleAuto(){
		if (Input.GetKeyDown(KeyCode.M)){
			Debug.Log("toggled! " + DoAuto);
			DoAuto = !DoAuto;
		}
	}
}
