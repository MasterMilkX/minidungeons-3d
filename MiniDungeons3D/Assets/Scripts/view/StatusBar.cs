//UI Status bar that fills based on value
//Code by Milk

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
	//ui properties
	public Sprite fillSpr;
	public Sprite emptySpr;
	private float barWidth;
	private float fullWidth;
	private float height;
	public Color color = Color.red;

	//values allowed - use single units (100% -> 10; 75% -> round 7 or 8)
	public int maxVal = 10;
	public int curVal = 7;

	private List<Image> rects;

	void Awake(){
		fullWidth = transform.GetComponent<RectTransform>().rect.width;
		height = transform.GetComponent<RectTransform>().rect.height;
		//barWidth = fullWidth/maxVal;
		//Debug.Log("w: " + fullWidth + " | h: " + height + " | bw: "+ barWidth);
		//CreateFullBar(maxVal);
		//UpdateValue(curVal);
	}

	//test increment and decrement of the bars
	// void Update(){
	// 	if(Input.GetKeyDown(KeyCode.Q)){
	// 		UpdateValue(curVal-1);
	// 	}
	// 	if(Input.GetKeyDown(KeyCode.W)){
	// 		UpdateValue(curVal+1);
	// 	}

	// 	if(Input.GetKeyDown(KeyCode.E)){

	// 		CreateFullBar(Random.Range(3,20));
	// 		UpdateValue(Random.Range(1,20));
	// 	}
	// }


	//create individual bars inside the progress bar
	public void CreateFullBar(int mv, bool increasing)
	{

		maxVal = mv;
		barWidth = fullWidth/maxVal;

		//remove previous
		foreach (Transform bar in transform) {
			GameObject.Destroy(bar.gameObject);
		}

		//create new
		rects = new List<Image>();
		for(int i=0;i<maxVal;i++){
			//set image size and position
			GameObject b = new GameObject(transform.name + "_b" + i);
         	b.transform.parent = transform;
         	b.transform.localPosition = Vector3.zero;
			RectTransform sr = b.AddComponent<RectTransform>();
			sr.sizeDelta = new Vector2(barWidth, height);
         	sr.anchoredPosition = new Vector3((i*barWidth)-(fullWidth/2.0f)+(barWidth/2),0.0f);
			b.transform.localScale = new Vector3(1, 1, 1);
			//set image
			Image s = b.AddComponent<Image>();

			if (increasing)
			{
				s.sprite = emptySpr;
			}
			else
			{
				s.sprite = fillSpr;
				s.color = color;
			}
         	//add to list
         	rects.Add(s);
         	
         	//Debug.Log(b.name);
		}
	}

	//set the max value and current value of the status bar
	public void SetStatValues(int mv, int cv){
		maxVal = mv;
		curVal = cv;
	}

    //update the current value of the bar (full = full sprite, empty = empty sprite)
    public void UpdateValue(int v){
    	curVal = (v > maxVal ? maxVal : (v <= 0 ? 0 : v));
    	for(int i=0;i<maxVal;i++){
    		if(i+1 <= curVal){
    			rects[i].gameObject.SetActive(true);
    			rects[i].sprite = fillSpr;
				rects[i].color = color;
			}
			else{
    			if(emptySpr)
    				rects[i].sprite = emptySpr;
    			 else
    			 	rects[i].gameObject.SetActive(false);
    		}
    	}
    }
}
