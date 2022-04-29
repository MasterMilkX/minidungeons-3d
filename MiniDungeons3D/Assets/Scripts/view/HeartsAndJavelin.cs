using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartsAndJavelin : MonoBehaviour {

	public Sprite HeartActive;
	public Sprite HeartInActive;

	public Sprite JavelinActive;
	public Sprite JavelinInactive;

	public Image Heart1;
	public Image Heart2;
	public Image Heart3;
	public Image Heart4;
	public Image Heart5;
	public Image Heart6;
	public Image Heart7;
	public Image Heart8;
	public Image Heart9;
	public Image Heart10;
	public Image Javelin;
	
	public Image[] _hearts;

	public void Start(){
		_hearts = new Image[10];
		_hearts[0] = Heart1;
		_hearts[1] = Heart2;
		_hearts[2] = Heart3;
		_hearts[3] = Heart4;
		_hearts[4] = Heart5;
		_hearts[5] = Heart6;
		_hearts[6] = Heart7;
		_hearts[7] = Heart8;
		_hearts[8] = Heart9;
		_hearts[9] = Heart10;
	}

	public void UpdateHeartsAndJavelin(SimLevel level){
		if(level.SimHero.Javelins > 0){
			Javelin.GetComponent<Image>().sprite = JavelinActive;
		} else {
			Javelin.GetComponent<Image>().sprite = JavelinInactive;
		}

		for(int i = 0; i < _hearts.Length; i++){
			if(i < level.SimHero.Health){
				_hearts[i].sprite = HeartActive;
			} else {
				_hearts[i].sprite = HeartInActive;
			}
		}
	}
}
