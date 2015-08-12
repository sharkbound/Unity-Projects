using UnityEngine;
using System.Collections;

public class TextControler : MonoBehaviour {
	public bool IsQuitButton;
	// Use this for initialization
	void Start () {
	
	}

	void OnMouseEnter(){
		var text = GetComponentInChildren<MeshRenderer> ();
		//change text color
		text.material.color = Color.red;
	}

	void OnMouseExit(){
		var text = GetComponentInChildren<MeshRenderer> ();
		//change text color
		text.material.color = Color.white;
	}


	void OnMouseUp(){
		// is it a quit button?
		if (IsQuitButton) {
			// exit game
			Application.Quit();

		} else {
			//GameObject.Find("Directional Light").GetComponent<Light>().enabled = false;
			//GameObject.Find("MainmenuTerrain").GetComponent<Terrain>().enabled = false;
			// load level
			Application.LoadLevel(1);
		}
		
	}
}
