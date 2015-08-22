using UnityEngine;
using System.Collections;

public class HitLifeReduce : MonoBehaviour {

	public int MaxLife = 100;
	public int CurrentLife = 100;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void damage(int dmg){

		CurrentLife -= dmg;
		Debug.Log (CurrentLife);
		if (CurrentLife <= 0){
			//Destroy(transform.gameObject);

			transform.gameObject.SetActive(false);
		}

	}
}
