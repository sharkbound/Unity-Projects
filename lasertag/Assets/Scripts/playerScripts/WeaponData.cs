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

	public string AutoOnMessage = "auto enabled. Press M to change";
	public string AutoOffMessage = "auto disabled. Press M to change";

	public bool IsAuto = false;
	// Use this for initialization
	void Start () {
		
		AutoStatis = GameObject.FindGameObjectWithTag("AutoStatisText").GetComponent<Text>();
		AutoStatis.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {

	   if (Input.GetKeyDown(KeyCode.M)) {
			IsAuto = !IsAuto;
		}
		if (PhotonNetwork.connected){
			if (IsAuto) {
				if (AutoStatis.text != AutoOnMessage) {
					AutoStatis.text = AutoOnMessage;
				}
			}
			else {
				if (AutoStatis.text != AutoOffMessage) {
					AutoStatis.text = AutoOffMessage;
				}
			}
		}

	}
}
