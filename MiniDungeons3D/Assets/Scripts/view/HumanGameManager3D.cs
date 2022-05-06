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
    public Audio_Manager audio_manager;

    public GameObject[] _UIInterfaces;

    // Made SimHeroAction public.
    public SimHeroAction nextAction;

    // 3D rendering
    public Transform Player;

    public List<Transform> place_objs;

    // included for debugging right now
    public TestLevelGenerator testGenerator;

    // Start is called before the first frame update
    void Start()
    {
        humanController = GetComponent<SimControllerHeroHuman3D>();
        testGenerator = GetComponent<TestLevelGenerator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (levelView._okForInput)
        {
            if (currentLevel.SimLevelState == SimLevelState.Playing)
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
            else if (currentLevel.SimLevelState == SimLevelState.Won)
            {
                audio_manager.PlaySFX(5);
                StartNewGame();

            } else if(currentLevel.SimLevelState == SimLevelState.Lost)
            {
                audio_manager.PlaySFX(6);
                StartNewGame();

            }
        }
    }    

    public void StartNewGame()
    {
        FlushUI();

        string[][] newMap = testGenerator.CreateRandomAscMap();
        LoadLevel(newMap);
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

        humanController.Initialize(currentLevel, levelView.Player);


        _viewState = ViewState.Playing;
    }

    //place prefabs on the game screen according to the map
    

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
