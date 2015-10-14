using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponData : MonoBehaviour {

	Text AutoStatis;

	public float FireRate = 0.5f;
	public float MaxDamage = 25f;
	public float MinDamage = 15f;

	public float AutoMaxDamage = 9f;
	public float AutoMinDamage = 5f;
	public float AutoFireRate = 0.25f;

	public bool IsAuto = false;
	// Use this for initialization
	void Start () {
		
		AutoStatis = GameObject.FindGameObjectWithTag("AutoStatisText").GetComponent<Text>();
		AutoStatis.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {

	   if (Input.GetKeyDown(KeyCode.Mouse1)) {
			IsAuto = !IsAuto;
		}
		if (PhotonNetwork.connected){
			if (IsAuto) {
				if (AutoStatis.text != "auto enabled. right click to change") {
					AutoStatis.text = "auto enabled. right click to change";
				}
			}
			else {
				if (AutoStatis.text != "auto disabled. right click to change") {
					AutoStatis.text = "auto disabled. right click to change";
				}
			}
		}

	}
}
