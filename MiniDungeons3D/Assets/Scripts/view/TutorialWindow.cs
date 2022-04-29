using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.Video;

//Code by M "Milk" Charity [6/4/21]



[System.Serializable]
public class TutorialStep {
    public string vid_loc;
	public string mechanic_text;
}

[System.Serializable]
public class TutorialSet{
	public string play_style;
	public TutorialStep[] steps;
}

[System.Serializable]
public class AllTut{
	public TutorialSet[] tutorials;
}

public class TutorialWindow : MonoBehaviour
{

	//ui objects
	public GameObject tutWindow;
	public VideoPlayer video;
	public Text instructionsTxt;
	public GameObject nextBtn;
	public GameObject prevBtn;
	public Text videoIndex;


	//json settings
	public string jsonTutorialPath;
	public string playStyle;
	//public float videoFPS;
	
    public TutorialStep[] curTutorial;
    public int curStep = 0;
    //private int videoAnimIndex = 0;



    // Start is called before the first frame update
    void Start()
    {
    	curStep = 0;
        LoadTutorial(playStyle);
        GotoTut(0);
    }

    //load the JSON file to read in the different windows for the specific play style as specified by the JSON
    void LoadTutorial(string style){
    	//reset tutorial set
    	curStep = 0;

    	//read in the JSON
        string p = jsonTutorialPath.Replace(".json","");
    	string jsonCont = Resources.Load<TextAsset>(p).text;
        Debug.Log(jsonCont);
    	AllTut ts = JsonUtility.FromJson<AllTut>(jsonCont);

    	//get specific playstyle
    	foreach(TutorialSet s in ts.tutorials){
    		if(s.play_style == style){
    			curTutorial = s.steps;
    			return;
    		}
    	}
    	

    }

    /*
    //adds a step for the tutorial
    void AddTutStep(string instr,string videoPath){
    	TextAsset ip = Resources.Load<TextAsset>(instr).text;
    	TextAsset vp = Resources.Load<TextAsset>(videoPath).text;
    	curTutorial.Add(new TutorialStep(ip,vp));
    }
    */

    //goes to a specific tutorial step and updates the text and video
    void GotoTut(int index){

    	//update values
    	curStep = index;
    	videoIndex.text = (curStep+1).ToString() + " / " + curTutorial.Length.ToString();
    	instructionsTxt.text = curTutorial[curStep].mechanic_text;
    	video.url = Application.dataPath + "/" + curTutorial[curStep].vid_loc;
    	video.Play();

    	//hide or show next/prev buttons
    	nextBtn.SetActive((curStep == curTutorial.Length-1 ? false : true));
    	prevBtn.SetActive((curStep == 0 ? false : true));

    }

    public void NextTut(){GotoTut(curStep+1);}
    public void PrevTut(){GotoTut(curStep-1);}
}
