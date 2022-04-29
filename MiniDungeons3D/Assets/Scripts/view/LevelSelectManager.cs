using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class LevelSelectManager : MonoBehaviour
{

    public GameObject Content;
    public GameObject LevelButtonPrefab;
    public HumanGameManager HGM;

    void OnEnable()
    {
        // GameSaver.LoadGame();
        // clear list
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        // add classic maps
        TextAsset[] classicMaps = Resources.LoadAll<TextAsset>("AsciiMaps/TutorialMaps/");

        foreach (TextAsset classicMap in classicMaps)
        {
            //Debug.Log(classicMap);
            int count = int.Parse(classicMap.name.Substring(3));
            // Debug.Log(GameSaver.Progress);
            // if (count <= GameSaver.Progress)
            // {
            GameObject button = Instantiate(LevelButtonPrefab, Content.transform) as GameObject;
            button.GetComponent<Button>().onClick.AddListener(delegate
           {
               HGM.LoadLevel(count);
           });

            button.GetComponentInChildren<Text>().text = "Map " + count;
            // }
        }

        // add any custom made maps
        /*	TextAsset[] playerMaps = Resources.LoadAll<TextAsset>("AsciiMaps/PlayerMaps/");
            foreach (TextAsset playerMap in playerMaps)
            {
                string name = playerMap.name;

                GameObject button = Instantiate(LevelButtonPrefab, Content.transform) as GameObject;
                button.GetComponent<Button>().onClick.AddListener(delegate
               {
                   HGM.LoadLevel(name);
               });

                button.GetComponentInChildren<Text>().text = name;
            }*/


        // string path = Application.persistentDataPath;
        // DirectoryInfo dir = new DirectoryInfo(path);
        // FileInfo[] info = dir.GetFiles("*.txt");
        // foreach (FileInfo f in info) 
        // { 
        // 	string temp = f.ToString ();
        // 	string temp1 = temp.Substring(0,temp.Length - 4);
        // 	string name;

        // 	name = temp1.Substring(temp1.LastIndexOf('/') + 1);

        // 	#if UNITY_EDITOR
        // 	name = temp1.Substring(temp1.LastIndexOf('\\') + 1);
        // 	#endif

        // 	//if the map is complete(has hero and exit)
        // 	if (IsComplete(name)) {
        // 		// instantiate button,add text and add action listener to the button
        // 		GameObject button = Instantiate (LevelButtonPrefab, Content.transform) as GameObject;
        // 		button.GetComponentInChildren<Text> ().text = name;
        // 		button.GetComponent<Button> ().onClick.AddListener (delegate {
        // 			HGM.LoadLevel (name);
        // 		});
        // 	}
        // }
    }

    //checks for complete map
    bool IsComplete(string fileName)
    {
#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh ();
#endif

        //read the file in a string
        string content = File.ReadAllText(Application.persistentDataPath + "/" + fileName + ".txt");
        //check if the file contains "Hero" and "Exit"
        if (content.Contains("e") && content.Contains("H"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
