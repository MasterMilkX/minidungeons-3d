using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class LevelSelect_d : MonoBehaviour {

	public GameObject Content;
	public GameObject LevelButtonPrefab;
	public DisplayManager_d displayManager; 
	string path = "";

	void OnEnable()
	{
		// clear scrollbar list
		foreach (Transform child in Content.transform)
		{
			Destroy(child.gameObject);
		}

		//list custom made maps
		string path = Application.persistentDataPath;
		DirectoryInfo dir = new DirectoryInfo(path);
		FileInfo[] info = dir.GetFiles("*.txt");
		foreach (FileInfo f in info) 
		{ 
			string temp = f.ToString ();
			string temp1 = temp.Substring(0,temp.Length - 4);
			string name;

			name = temp1.Substring(temp1.LastIndexOf('/') + 1);

			#if UNITY_EDITOR
			name = temp1.Substring(temp1.LastIndexOf('\\') + 1);
			#endif
			// instantiate button,add text and add action listener to the button
			GameObject button = Instantiate(LevelButtonPrefab, Content.transform) as GameObject;
			button.GetComponentInChildren<Text>().text = name;
			button.GetComponent<Button>().onClick.AddListener(delegate
			{
					displayManager.ShowEditOption(name);
			});
		}
	}
}

