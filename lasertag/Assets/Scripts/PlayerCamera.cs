using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

	public float cameraSpeed = 10.0f;  // move speed
	public float RotationDamp = 0.1f;
	float CurrentXRot;
	float CurrentYRot;
	float x2;
    float y2;
	float x;
	float y;

	// Update is called once per frame
	//is the player on ground?
	void Update(){
		CharacterController controller = GameObject.Find("pc").GetComponent<CharacterController>();
		if (controller.isGrounded) {

			//move the camera with the mouse
			x += Input.GetAxis("Mouse X") * cameraSpeed;
			y +=  -Input.GetAxis("Mouse Y") * cameraSpeed;

			//x = Mathf.Clamp(x, -90, 90);
			//y = Mathf.Clamp(y, -90, 90);

			CurrentXRot = Mathf.SmoothDamp(CurrentXRot, x,ref x2, RotationDamp);  // dampen the camera rotation on X axis
			CurrentYRot = Mathf.SmoothDamp(CurrentYRot, y,ref y2, RotationDamp);  // dampen the camera rotation on Y axis

			transform.rotation = Quaternion.Euler(CurrentYRot,CurrentXRot,0);
		}
	}
}
