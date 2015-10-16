using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseToggle : MonoBehaviour {

	public Text FPSText;

	public GameObject MainMenuUI;
	public GameObject PauseMenuUI;

	public static bool IsPaused = false;
	public bool ShowFPS = false;
	public static bool Disconnecting = false;

	private int fps = 0;
	private int frameCount = 0;

	public float FPSUpdateTime = 0.5f;
	private float passedTime = 0.0f;

	//Vector3 PlayerPausePosition = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		TogglePauseBool();
		LockAndUnlockMouse();
		CalulateFPS();
	/*	if (IsPaused && PhotonNetwork.connected && !Disconnecting && NetworkManager.MyPlayer != null) {
			NetworkManager.MyPlayer.transform.position = PlayerPausePosition;
		} */
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
				unPause();
			} else if (IsPaused) {
				Pause ();
			}
	    }
	}

	void Pause() {
		if (Cursor.lockState == CursorLockMode.Locked && Cursor.visible == false ){
			//PlayerPausePosition = NetworkManager.MyPlayer.transform.position;
			PauseMenuUI.SetActive(true);
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
	
	void unPause() {
		if (Cursor.lockState == CursorLockMode.None && Cursor.visible == true){
			PauseMenuUI.SetActive(false);
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	void CalulateFPS() {
		if (ShowFPS && PhotonNetwork.connected){
			frameCount++;
			passedTime += Time.deltaTime;
			if (passedTime > FPSUpdateTime){
				fps = Mathf.FloorToInt(frameCount / passedTime);
				frameCount = 0;
				passedTime = FPSUpdateTime - passedTime;
				
				var text = FPSText.GetComponent<Text>();
				text.color = Color.red;
				text.text = "FPS: " + fps;
				FPSText.enabled = true;
			}
		} else {
			FPSText.enabled = false;
		}
	}

	void DisconnectMsg() {
		GameObject.Find("_PhotonStuff").GetComponent<NetworkManager>().AddChatMessage(PhotonNetwork.player.name + " has disconnected");
		//NetworkManager.AddChatMessage(PhotonNetwork.player.name + " has disconnected"); 
	}

	void DisablePlayerIngameUIElements(){
		GameObject.Find("CrossHair").GetComponent<Image>().enabled = false;
		GameObject.Find("ConnectionStatus").GetComponent<Text>().enabled = false;
		GameObject.Find("FPS").GetComponent<Text>().enabled = false;
		GameObject.Find("TeleportAbilityStatus").GetComponent<Text>().enabled = false;
		GameObject.Find("AutoStatis").GetComponent<Text>().enabled = false;
		GameObject.Find("CurrentHealth").GetComponent<Text>().enabled = false;
	}

	void DelayedDisconnect(){
		PhotonNetwork.Disconnect();
	}

	void DelayedSetDisconnectingBooleon(){
		Disconnecting = false;
	}

	public void DisconnectButton() {
		Disconnecting = true;
		IsPaused = true;
		PauseMenuUI.SetActive(false);
		DisconnectMsg();
		DisablePlayerIngameUIElements();
		GameObject.Destroy(NetworkManager.MyPlayer);
		GameObject.Find("StandbyCamera").GetComponent<Camera>().enabled = true;
		Invoke("DelayedDisconnect", 0.1f);
		Invoke("DelayedSetDisconnectingBooleon", 0.1f);
		MainMenuUI.SetActive(true);
	}

	public void UnPauseButton() {
		IsPaused = false;
	}

	public void ToggleFpsCounter() {
		ShowFPS = !ShowFPS;
	}
}
