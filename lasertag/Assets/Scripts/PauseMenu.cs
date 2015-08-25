using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	public GameObject PauseUI;
	public GameObject PlayerCam;

	private bool paused = false;
	private bool ShowFps = false;
	// Use this for initialization
	void Start () {
		PauseUI.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)){
			paused = !paused;
		}
		if (paused){
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			PauseUI.SetActive(true);
			Time.timeScale = 0;
		}
		if (!paused){
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			PauseUI.SetActive(false);
			Time.timeScale = 1;
		}
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
	public void ToggleVsync(){
		if (QualitySettings.vSyncCount == 1){
			QualitySettings.vSyncCount = 0;
		}
		if (QualitySettings.vSyncCount == 0){
			QualitySettings.vSyncCount = 1;
		}
	}
}
