using UnityEngine;
using System.Collections;

public class SmoothGrenadeMovement : MonoBehaviour {

	PhotonView photonView;

	Vector3 realPosition = Vector3.zero;

	Quaternion realRotation = Quaternion.identity;

	// Use this for initialization
	void Start () {
		photonView = GetComponent<PhotonView>();
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
			// this is our grenade, we send of posision data here
			stream.SendNext(transform.position); //send our posision to the network
			stream.SendNext(transform.rotation); // send our rotation to the network
		}
		else {
			//this is everyone elses grenades, we recieve their posisions here  
			
			// right now realPosition holds the player position on the Last frame
			// instead of simply updating "RealPosition" and continuing to lerp
			// we MAY want to set out transform.position IMMEDIITLY to this old "realPosition"
			// then update realPosition
			
			realPosition = (Vector3)stream.ReceiveNext(); //recieve others posisions
			realRotation = (Quaternion)stream.ReceiveNext(); // recieve others rotations 
		}
		
	}
}
