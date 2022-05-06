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
using Assets.Scripts.simulator.Controllers.Human;

public class HumanGameManager3D : MonoBehaviour
{
    public string currentLevelString;
    public string[][] currentLevelArray;
    public SimLevel currentLevel;
    public SimControllerHeroHuman3D humanController;
    public LevelView levelView;
    public GameObject _inGame;
    public static ViewState _viewState = ViewState.MainMenu;
    public bool PlayingClassic;


    public GameObject[] _UIInterfaces;

    // Made SimHeroAction public.
    public SimHeroAction nextAction;

    // 3D rendering
    public GameObject wall;
    public GameObject floor;
    public GameObject ceiling;
    public GameObject exit;
    public GameObject minitaur;
    public GameObject goblin;
    public GameObject wizard;
    public GameObject ogre;
    public GameObject blob;
    public GameObject potion;
    public GameObject treasure;
    public GameObject trap;
    public GameObject portal;
    public Transform Player;

    public Transform Level;
    public List<Transform> place_objs;

    // Start is called before the first frame update
    void Start()
    {
        humanController = GetComponent<SimControllerHeroHuman3D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (levelView._okForInput)
        {
            if (humanController.HasAction)
            {
                nextAction = humanController.NextAction(currentLevel);
                //Debug.Log("act " + nextAction);
                if (currentLevel.MoveIsLegal(currentLevel.SimHero, nextAction))
                {
                    Debug.Log(nextAction.ActionType.ToString());
                    currentLevel.RunTurn(nextAction);
                    levelView.RefreshView(currentLevel);
                    try
                    {
                        humanController.positions.Add(currentLevel.SimHero.Point);
                        humanController.levelStates.Add(currentLevel.ToAsciiMap());
                    }
                    catch
                    {
                        Debug.Log("human controller issue");
                    }
                    // try { _ReplayManager.sessionReplay.AddEngineAction(nextAction); } catch { }

                }
            }
        }
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
        humanController.Initialize(currentLevel, Player);


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
                Debug.Log("Here: " + h + ", " + w);
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

        
        //place populate the level
        for (int h = 0; h < currentLevelArray.Length; h++)
        {
            for (int w = 0; w < currentLevelArray[h].Length; w++)
            {
                GameObject newObj = null;
                string val = currentLevelArray[h][w];
                // wall
                if (val == "X")
                {
                    newObj = Instantiate(wall, new Vector3(w, 0.0f, h), Quaternion.identity);
                    
                }
                // hero
                else if (val == "H")
                {
                    Player.position = new Vector3(w, -0.55f, h);
                    Player.rotation = Quaternion.identity;
                }
                // exit
                else if(val == "e")
                {
                    newObj = Instantiate(exit, new Vector3(w, 0.0f, h), Quaternion.identity);
                }
                else if (val == "m")
                {
                    newObj = Instantiate(minitaur, new Vector3(w, 0.0f, h), Quaternion.identity);
                }
                else if (val == "M")
                {
                    newObj = Instantiate(goblin, new Vector3(w, 0.0f, h), Quaternion.identity);
                }
                else if (val == "R")
                {
                    newObj = Instantiate(wizard, new Vector3(w, 0.0f, h), Quaternion.identity);
                }
                else if (val == "o")
                {
                    newObj = Instantiate(ogre, new Vector3(w, 0.0f, h), Quaternion.identity);
                }
                else if (val == "b")
                {
                    newObj = Instantiate(blob, new Vector3(w, 0.0f, h), Quaternion.identity);
                }
                else if (val == "T")
                {
                    newObj = Instantiate(treasure, new Vector3(w, 0.0f, h), Quaternion.identity);
                }
                else if (val == "t")
                {
                    newObj = Instantiate(trap, new Vector3(w, 0.0f, h), Quaternion.identity);
                }
                else if (val == "P")
                {
                    newObj = Instantiate(potion, new Vector3(w, 0.0f, h), Quaternion.identity);
                }
                else if (val == "p")
                {
                    newObj = Instantiate(portal, new Vector3(w, 0.0f, h), Quaternion.identity);
                }
                
                // is this technically less code? idk
                try
                {
                    newObj.transform.SetParent(Level);
                }
                catch
                {
                    // do nothing this is fine
                }
            }
        }
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
            if (i < map.Length - 1)
            {
                mapstring += "\n";
            }
        }
        Debug.Log(mapstring);
        return mapstring;
    }
}
