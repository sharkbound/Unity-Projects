using UnityEngine;
using System.Collections;

public class NetworkPlayer : Photon.MonoBehaviour {

	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;
	Animator anim;
	bool gotFirstUpdate = false;
	float realAimAngle = 0f;

	// Use this for initialization
	void Start () {
		InitAnim();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(photonView.isMine){
			// do nothing, the movement script is handling everything for us
		}
		else {
			transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
			anim.SetFloat("AimAngle", Mathf.Lerp(anim.GetFloat("AimAngle"), realAimAngle, 0.1f));
		}
	}

	void InitAnim() {
		if ( anim == null ){
			anim = GetComponent<Animator>();
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if(stream.isWriting){
			// this is our player, we send of posision data here
			stream.SendNext(transform.position); //send our posision to the network
			stream.SendNext(transform.rotation); // send our rotation to the network
			stream.SendNext(anim.GetFloat("Speed"));
			stream.SendNext(anim.GetBool("Jumping"));
			stream.SendNext(anim.GetFloat("AimAngle"));
		}
		else {
			//this is everyone elses players, we recieve their posisions here  

			// right now realPosition holds the player position on the Last frame
			// instead of simply updating "RealPosition" and continuing to lerp
			// we MAY want to set out transform.position IMMEDIITLY to this old "realPosition"
			// then update realPosition

			realPosition = (Vector3)stream.ReceiveNext(); //recieve others posisions
			realRotation = (Quaternion)stream.ReceiveNext(); // recieve others rotations 
			anim.SetFloat("Speed", (float)stream.ReceiveNext());
			anim.SetBool("Jumping", (bool)stream.ReceiveNext());
			realAimAngle = (float)stream.ReceiveNext();

			if (gotFirstUpdate == false) {
				transform.position = realPosition;
				transform.rotation = realRotation;
				anim.SetFloat("AimAngle", realAimAngle);
				gotFirstUpdate = true;
			}
		}

	}
}
