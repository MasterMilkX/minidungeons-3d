using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class HumanGameManager3D : MonoBehaviour
{
    public string currentLevelString;
    public string[][] currentLevelArray;
    public SimLevel currentLevel;
    public SimControllerHeroHuman humanController;
    public LevelView levelView;
    public GameObject _inGame;
    public static ViewState _viewState = ViewState.MainMenu;
    public bool PlayingClassic;


    public GameObject[] _UIInterfaces;


    // 3D rendering
    public GameObject wall;
    public GameObject floor;
    public GameObject ceiling;

    public Transform Level;
    public List<Transform> place_objs;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevel(string[][] levelArray)
    {
        levelView = GetComponent<LevelView>();
        levelView.ClearView();

        currentLevelArray = levelArray;
        currentLevelString = GetMapstring(levelArray);
        
        Debug.Log("level : " + currentLevelString);
        currentLevel = new SimLevel(currentLevelString);
        levelView.Initialize(currentLevel);

        //humanController.Initialize(currentLevel);
        FlushUI();
        _inGame.SetActive(true);

        PlaceAscMap();
        _viewState = ViewState.Playing;
    }

    //place prefabs on the game screen according to the map
    public void PlaceAscMap()
    {
        List<Vector3> open_pos = new List<Vector3>();

        //place the floor
        for (int h = 0; h < currentLevelArray.Length; h++)
        {
            for (int w = 0; w < currentLevelArray[h].Length; w++)
            {
                GameObject newFloor = Instantiate(floor, new Vector3(w, -0.55f, h), Quaternion.identity);
                newFloor.transform.SetParent(Level);
            }
        }

        //place upside floor (ceiling)
        for (int h = 0; h < currentLevelArray.Length; h++)
        {
            for (int w = 0; w < currentLevelArray[h].Length; w++)
            {
                GameObject newFloor = Instantiate(ceiling, new Vector3(w, 0.55f, h), Quaternion.identity * Quaternion.Euler(180, 0, 0));
                newFloor.transform.SetParent(Level);
            }
        }

        
        //place the floor
        for (int h = 0; h < currentLevelArray.Length; h++)
        {
            for (int w = 0; w < currentLevelArray[h].Length; w++)
            {
                if (currentLevelArray[h][w] == "X")
                {
                    GameObject newWall = Instantiate(wall, new Vector3(w, 0.0f, h), Quaternion.identity);
                    newWall.transform.SetParent(Level);
                }
                else
                    open_pos.Add(new Vector3(w, -0.55f, h));
            }
        }

        //place the player on a random open spot
        //Player.position = open_pos[Random.Range(0, open_pos.Count)];



    }

    void FlushUI()
    {
        PlayingClassic = false;
        foreach (GameObject face in _UIInterfaces)
        {
            face.SetActive(false);
        }
    }

    public string GetMapstring(string[][] map)
    {
        string mapstring = "";
        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[i].Length; j++)
            {
                mapstring += map[i][j];
            }
            mapstring += "\n";
        }
        return mapstring;
    }
}
