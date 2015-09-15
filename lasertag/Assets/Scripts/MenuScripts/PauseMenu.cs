using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class PauseMenu : MonoBehaviour {

	public float FPSUpdateTime = 0.5f;

	public GameObject PauseUI;
	//public GameObject PlayerCam;
	public GameObject FPSText;
	// private variables
	private bool paused = false;
	private bool ShowFps = false;
	private int fps = 0;
	private int frameCount = 0;
	private float passedTime = 0.0f;
	private Shooting fireScript;
	// Use this for initialization
	void Start () {
		fireScript = GameObject.FindObjectOfType<PlayerMovement>().GetComponent<Shooting>();  
		PauseUI.SetActive(false);
		var text = FPSText.GetComponent<Text>();
		text.color = Color.red;
		if (!ShowFps){
			FPSText.SetActive(false);
		}
	}
	// Update is called once per frame
	void Update () {
		PauseToggle();
		TogglePauseMenu();
		if (ShowFps){
			frameCount++;
			passedTime += Time.deltaTime;
			if (passedTime > FPSUpdateTime){
				fps = Mathf.FloorToInt(frameCount / passedTime);
				frameCount = 0;
				passedTime = FPSUpdateTime - passedTime;

				var text = FPSText.GetComponent<Text>();
				text.color = Color.red;
				text.text = "FPS: " + fps;
				FPSText.SetActive(true);
			}
		} else {
			FPSText.SetActive(false);
		}
		 
	}
	public void PauseToggle(){
		if (Input.GetKeyDown(KeyCode.Escape)){
			paused = !paused;
		}
	}
	public void DisconnectButton() {
		GameObject.Find("PauseMenuUI").SetActive(false);
		GameObject.Find("MainMenuUI").SetActive(true);
		PhotonNetwork.Disconnect();
	}
	public void ResumeButton(){
		paused = false;
	}
	public void QuitButton(){
		Application.Quit();
	}
	public void ToggleFPS(){
		ShowFps = !ShowFps;
	}
	public void TogglePauseMenu(){
		if (paused){
			fireScript.enabled = false;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			PauseUI.SetActive(true);
			Time.timeScale = 0;
		} else {
			fireScript.enabled = false;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			PauseUI.SetActive(false);
			Time.timeScale = 1;
		}
	}
}
