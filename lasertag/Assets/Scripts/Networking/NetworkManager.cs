using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {
	private Text ConnectStatus;
	public Camera OverViewCamera;
	private PlayerSpawns[] spawns;
	// Use this for initialization
	void Start () {

		spawns = GameObject.FindObjectsOfType<PlayerSpawns>();
		ConnectStatus = GameObject.Find("ConnectionStatus").GetComponent<Text>();
		Connect ();
	}

	void ChangeStatusText(){
		ConnectStatus.text = PhotonNetwork.connectionStateDetailed.ToString();
		//Debug.Log(PhotonNetwork.connectionStateDetailed.ToString());
	}

	void Connect(){
		Debug.Log("Connect");
		PhotonNetwork.ConnectUsingSettings("Alpha Version");
		ChangeStatusText();
	}

	void OnJoinedLobby(){
		Debug.Log("OnJoinedLobby");
		RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 10 };
		PhotonNetwork.JoinOrCreateRoom("Dev", roomOptions, TypedLobby.Default);
	}

	void OnJoinedRoom(){
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

		PlayerSpawns MySpawnLocation = spawns[ Random.Range(0, spawns.Length) ];
		GameObject MyPlayer = PhotonNetwork.Instantiate("RobotCharater", MySpawnLocation.transform.position, 
		                          MySpawnLocation.transform.rotation, 0);
		((MonoBehaviour)MyPlayer.GetComponent("FirstPersonController")).enabled = true;
		MyPlayer.transform.FindChild("FirstPersonCharacter").GetComponent<Camera>().enabled = true;
		MyPlayer.transform.FindChild("FirstPersonCharacter").GetComponent<AudioListener>().enabled = true;
		//MyPlayer.GetComponent<AudioSource>().enabled = true;
		ToggleOverViewCam();

	}

	void ToggleOverViewCam(){
		OverViewCamera.enabled = !OverViewCamera.enabled;
	}
}
