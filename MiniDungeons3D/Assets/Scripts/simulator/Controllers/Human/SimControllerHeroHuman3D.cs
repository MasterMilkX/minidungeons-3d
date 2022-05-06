﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.simulator.Controllers.Human
{
    public class SimControllerHeroHuman3D : MonoBehaviour
    {

        public bool Initialized;
        public bool HasAction;
        public bool replayMode = false;

        public List<SimHeroAction> actions { get; set; }


        //Made the nextAction public;
        public SimHeroAction nextAction;

        public Transform Player3D;

        // boolean flipper for toggling javelin on and off
        private Boolean JavelinToggle;
        // list that keeps track of targets whos' z orders were changed
        public GameObject[] javTargets;

        // dimmer screen
        public GameObject _dimmer;


        public SimLevel _level;

        public LevelView _levelView;
        public bool okForInput;


        public GameObject panel, textBox, javText;

        private Color HighlightColor = Color.green;
        private Color TargetColor = Color.red;
        private SpriteRenderer spriteRenderer;
        bool highlighted = false, tap = false, javelinActivated = false;
        GameObject highlightedObject = null;
        Color oldColor;
        int potion, treasure;
        public List<SimPoint> positions { get; set; }
        public List<string> levelStates { get; set; }

        List<GameObject> targets;
        List<SimPoint> targetPoints;
        int targetSelected = 0;

        // Use this for initialization
        void Awake()
        {
            Initialized = false;
            HasAction = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Initialized && true)
            {
                TakeInput();
            }
        }

        public void Initialize(SimLevel level, Transform player)
        {
            _level = level;
            Initialized = true;
            JavelinToggle = false;
            PathDatabase.levels = new System.Collections.ArrayList();
            PathDatabase.BigDatabase = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, SimPoint[]>>();
            PathDatabase.Farthest = new System.Collections.Generic.Dictionary<SimLevel, Pair>();
            PathDatabase.BuildDB(level);
            actions = new List<SimHeroAction>();
            positions = new List<SimPoint>();
            levelStates = new List<string>();
            Player3D = player;
            //Debug.Log("Javelins:" + _level.SimHero.Javelins);
        }
        public SimHeroAction NextAction(SimLevel level)
        {
            if (!HasAction)
            {
                throw new Exception("Action not ready");
            }
            HasAction = false;
            //JavelinOff();
            actions.Add(nextAction);
            return nextAction;
        }
        void TakeInput()
        {
            if (Input.GetKeyUp(KeyCode.Y))
            {
                Debug.Log("Possible actions");
                var possibleActions = _level.GetPossibleHeroActions();
                foreach (SimHeroAction action in possibleActions)
                {
                    Debug.Log(action);
                }
            }


            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                //if (!javelinActivated)
                //{
                // Forward
                string direction = DirectionCheck();
                if (direction == "north")
                {
                    nextAction = (new SimHeroAction(HeroActionTypes.Move, SimPoint.North));
                } else if (direction == "south")
                {
                    nextAction = (new SimHeroAction(HeroActionTypes.Move, SimPoint.South));
                }
                else if (direction == "east")
                {
                    nextAction = (new SimHeroAction(HeroActionTypes.Move, SimPoint.East));
                }
                else if (direction == "west")
                {
                    nextAction = (new SimHeroAction(HeroActionTypes.Move, SimPoint.West));
                }
                HasAction = true;
                //}
                // try
                // {
                //     _ReplayManager.sessionReplay.AddUIAction(nextAction);
                // }
                // catch
                // {
                // }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                /*if (javelinActivated)
                {
                    if (targets.Count > 1)
                    {
                        if (targetSelected == targets.Count - 1)
                        {
                            targetSelected = 0;
                        }
                        else
                        {
                            targetSelected = targetSelected + 1;
                        }

                        ShowTarget(targetSelected);
                    }
                }
                else
                {
                nextAction = (new SimHeroAction(HeroActionTypes.Move, SimPoint.East));
                HasAction = true;*/
                // try
                // {
                //     _ReplayManager.sessionReplay.AddUIAction(nextAction);
                // }
                // catch
                // {
                // }
                //}
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                //if (!javelinActivated)
                //{
                string direction = DirectionCheck();
                if (direction == "north")
                {
                    nextAction = (new SimHeroAction(HeroActionTypes.Move, SimPoint.South));
                }
                else if (direction == "south")
                {
                    nextAction = (new SimHeroAction(HeroActionTypes.Move, SimPoint.North));
                }
                else if (direction == "east")
                {
                    nextAction = (new SimHeroAction(HeroActionTypes.Move, SimPoint.West));
                }
                else if (direction == "west")
                {
                    nextAction = (new SimHeroAction(HeroActionTypes.Move, SimPoint.East));
                }
                HasAction = true;
                //}
                // try
                // {
                //     _ReplayManager.sessionReplay.AddUIAction(nextAction);
                // }
                // catch
                // {
                // }
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                /*if (javelinActivated)
                {
                    if (targets.Count > 1)
                    {
                        if (targetSelected == 0)
                        {
                            targetSelected = targets.Count - 1;
                        }
                        else
                        {
                            targetSelected = targetSelected - 1;
                        }
                        ShowTarget(targetSelected);
                    }
                }
                else
                {
                    nextAction = (new SimHeroAction(HeroActionTypes.Move, SimPoint.West));
                HasAction = true;
                }*/
            }

            /*if (Input.GetKeyDown(KeyCode.Space))
            {
                GetJavelinTargets();

                if (_level.SimHero.Javelins == 0)
                {
                    javText.GetComponent<Text>().text = "No javelin";
                }
                else if (targets.Count == 0)
                {
                    javText.GetComponent<Text>().text = "No target";
                }
                else if (!javelinActivated && targets.Count > 0)
                {
                    JavelinOn();
                }
                else if (javelinActivated && highlightedObject == null)
                {
                    JavelinOff();
                }
                else if (javelinActivated && highlightedObject != null)
                {
                    nextAction = new SimHeroAction(HeroActionTypes.JavelinThrow, targetPoints[targetSelected]);
                    HasAction = true;
                    //ToggleJavelin();
                    JavelinOff();
                }
            }*/
        }
        
        string DirectionCheck()
        {
            float y = Player3D.rotation.y;
            if (y == 0f)
            {
                return "north";
            } else if (y == 90f)
            {
                return "east";
            } else if(y == 180f)
            {
                return "south";
            } else
            {
                return "west";
            }
        }
    }


}