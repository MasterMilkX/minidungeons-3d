using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITorches : MonoBehaviour {

	public Sprite image0;
	public Sprite image1;

	public Image torchLeft;
	public Image torchRight;

	public bool flipped;

	public float myTime;
	public float frequency = 0.15f;

	// Use this for initialization
	void Start () {
		myTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - myTime > frequency){
			myTime = Time.time;
			FlipImages();
		}
	}

	void FlipImages(){
		if(flipped){
			torchLeft.sprite = image0;
			torchRight.sprite = image1;
			flipped = false;
		} else {
			torchLeft.sprite = image1;
			torchRight.sprite = image0;
			flipped = true;
		}
	}
}
