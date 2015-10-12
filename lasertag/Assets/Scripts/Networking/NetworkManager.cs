using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {
	public GameObject MyPlayer;
	public Text ChatWindow;
	public List<string> chatMessages;
	int maxChatMessages = 5;
	private Text ConnectStatus;
	public Camera OverViewCamera;
	private PlayerSpawns[] spawns;
	public bool OfflineMode = false;
	public InputField UsernameInput;
	public GameObject UsernameUI;
	public GameObject MainMenuUI;
	private PhotonView PV;
	public float RespawnTimer = 0f;
	// Use this for initialization
	void Start () {

		GetComponent<FXManager>().enabled = true;
		PV = GetComponent<PhotonView>();
		spawns = GameObject.FindObjectsOfType<PlayerSpawns>();
		ConnectStatus = GameObject.Find("ConnectionStatus").GetComponent<Text>();
		chatMessages = new List<string>();
	}
     
	void OnGUI(){
		if (PhotonNetwork.connected == true && PhotonNetwork.connecting == false) {
			foreach (string msg in chatMessages) {
				GUILayout.Label(msg);
			}
		}
	} 

	void ChangeStatusText(){
		ConnectStatus.text = PhotonNetwork.connectionStateDetailed.ToString();
	}

	void ConnectToServer(){
		if (OfflineMode) {
			PhotonNetwork.player.name = PlayerPrefs.GetString("username");
			UsernameUI.SetActive(false);
			PhotonNetwork.offlineMode = true;
			OnJoinedLobby();
		}
		else {
			PhotonNetwork.player.name = PlayerPrefs.GetString("username");
			UsernameUI.SetActive(false);
			Debug.Log("Connect, Playername: " + PhotonNetwork.player.name);
			PhotonNetwork.ConnectUsingSettings("Dev Build networktest v1.5");
			ChangeStatusText();
		}
	}

	void OnJoinedLobby(){
		Debug.Log("OnJoinedLobby");
		RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 10 };
		PhotonNetwork.JoinOrCreateRoom("Dev", roomOptions, TypedLobby.Default);
	}

	void OnJoinedRoom(){
		AddChatMessage(PhotonNetwork.player.name + " has joined the server");
		Debug.Log("OnJoinedRoom");
		SpawnMyPlayer();
		ChangeStatusText();
	}

	void OnPhotonCreateRoomFailed(){
		Debug.Log("OnPhotonCreateRoomFailed");
	}

	void OnConnectionFail(){
		Debug.Log("OnConnectionFail");
	}

	void OnConnectedToMaster(){
		ChangeStatusText();
		Debug.Log("OnConnectedToMaster");
	}

	void SpawnMyPlayer(){
		if (spawns == null){
			Debug.LogError("Something went wrong :(");
			return;
		}
		AddChatMessage(PhotonNetwork.player.name + " has spawned!");

		PlayerSpawns MySpawnLocation = spawns[ Random.Range(0, spawns.Length) ];
	    MyPlayer = PhotonNetwork.Instantiate("AnimRobot2", MySpawnLocation.transform.position,
		                          MySpawnLocation.transform.rotation, 0);

		MyPlayer.transform.FindChild("FirstPersonCharacter").GetComponent<Camera>().enabled = true;
		MyPlayer.transform.FindChild("FirstPersonCharacter").GetComponent<AudioListener>().enabled = true;
		MyPlayer.transform.FindChild("FirstPersonCharacter").GetComponent<PlayerCam>().enabled = true;
		MyPlayer.GetComponent<PlayerMovement>().enabled = true;
		MyPlayer.GetComponent<PlayerCam>().enabled = true;
		MyPlayer.GetComponent<FirstToThirdperson>().enabled = true;
		MyPlayer.GetComponent<Shooting>().enabled = true;
		MyPlayer.GetComponentInChildren<WeaponData>().enabled = true;
		ToggleOverViewCam();
	}

	void ToggleOverViewCam(){
		//OverViewCamera.enabled = !OverViewCamera.enabled;
		OverViewCamera.enabled = false;
	}

	public void UpdateSavedUsername() {
		PlayerPrefs.SetString("username", UsernameInput.text);
	}

	public void SinglePlayerButton() {
		OfflineMode = true;
		//GameObject.Find("_OtherScripts").GetComponent<PauseMenu>().enabled = true;
		MainMenuUI.SetActive(false);
		GameObject.Find("CrossHair").GetComponent<Image>().enabled = true;
		ConnectToServer();
	}

	public void MultiplayerButton() {
		OfflineMode = false;
		//GameObject.Find("_OtherScripts").GetComponent<PauseMenu>().enabled = true;
		MainMenuUI.SetActive(false);
		GameObject.Find("CrossHair").GetComponent<Image>().enabled = true;
		ConnectToServer();
	}

	public void QuitButton() {
		Application.Quit();
	}

	public void EnterUsername() {
		MainMenuUI.SetActive(false);
		UsernameUI.SetActive(true);
		if (!PlayerPrefs.HasKey("username")) {
			PlayerPrefs.SetString("username", "Awesome Dude");
		}
		UsernameInput.text = PlayerPrefs.GetString("username");
	}

	public void AddChatMessage(string m) {
		PV.RPC("AddChatMessage_RPC", PhotonTargets.AllBuffered, m);
	}

	[PunRPC]
	public void AddChatMessage_RPC(string m) {
		while (chatMessages.Count > maxChatMessages) {
			chatMessages.RemoveAt(0);
		}
		chatMessages.Add(m);
	}

	void OnLeftRoom() {
		//AddChatMessage(PhotonNetwork.player.name + " has left the server");
	}


	void Update() {
		if (RespawnTimer > 0) {
			RespawnTimer -= Time.deltaTime;

			if (RespawnTimer <= 0) { // respawn player
				SpawnMyPlayer();
			}
		}
	}
}
