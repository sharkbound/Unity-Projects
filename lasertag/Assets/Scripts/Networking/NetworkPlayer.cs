using UnityEngine;
using System.Collections;

public class NetworkPlayer : Photon.MonoBehaviour {

	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(photonView.isMine){
			// do nothing, the movement script is handling everything for us
		}
		else {
			transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if(stream.isWriting){
			// this is our player, we send of posision data here
			stream.SendNext(transform.position); //send our posision to the network
			stream.SendNext(transform.rotation); // send our rotation to the network
		}
		else{
			//this is everyone elses players, we recieve their posisions here  
			realPosition = (Vector3)stream.ReceiveNext(); //recieve others posisions
			realRotation = (Quaternion)stream.ReceiveNext(); // recieve others rotations 
		}

	}
}
