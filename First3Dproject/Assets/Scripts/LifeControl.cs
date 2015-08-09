using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LifeControl : MonoBehaviour {
	public Sprite Life1;  // 1 heart
	public Sprite Life2;  // 2 hearts
	public Sprite Life3;   // 3 hearts
	public static int LIVES = 3;
	public static int HITS = 1;
	public Transform bodypart1;
	public Transform bodypart2;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		print ("Lives: " + LIVES + "---- Hits: " + HITS);
		// image array
		Image[] images;
		//set images array equal images in the child component
		images  = gameObject.GetComponentsInChildren<Image>();
	  switch (LIVES) 
		{
			// if player has 3 hearts
		case 3:
			//do this for the image in images
			foreach (Image image in images)
			{
				// set the sprite to 3 hearts
				image.sprite = Life3;
			}
				break;
			// if player has 2 hearts
		case 2:
			//do this for the image in images
			foreach (Image image in images)
			{
				// set the sprite to 2 hearts
				image.sprite = Life2;
			}		
			break;
			// if player has 2 hearts
		case 1:
			//do this for the image in images
			foreach (Image image in images)
			{
				// set the sprite to 1 hearts
				image.sprite = Life1;
			}
				break;
			// if player has no life left
		case 0:
			// make player dead
			moveScript.dead = true;
			//set lifes to 3
			LIVES = 3;
			break;
		}
		//var bodyparts = gameObject.GetComponentsInChildren<Transform>();
	switch(HITS){
		case 1:
			// Disable bodypart1

			break;
		case 2:
			// Disable bodypart2
			break;
		case 3:
			break;
		}

	}
}
