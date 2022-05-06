using System;
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


        // 3D moving and turning vars
        //movement vars
        private Vector3 curPos;
        public Vector3 targetPos;
        private bool moving = false;
        public float moveSpeed = 0.75f;
        private float t = 0;

        //rotation vars
        public Quaternion targetRot;
        private bool turning = false;
        public float turnSpeed = 0.75f;

        // Use this for initialization
        void Awake()
        {
            Initialized = false;
            HasAction = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Initialized)
            {
                if (moving)
                {
                    t += Time.deltaTime / moveSpeed;
                    Player3D.position = Vector3.MoveTowards(curPos, targetPos, t);
                    if (Vector3.Distance(Player3D.position, targetPos) < 0.1)
                    {
                        Player3D.position = targetPos;
                        moving = false;
                    }
                }
                else if (turning)
                {
                    Player3D.rotation = Quaternion.Lerp(Player3D.rotation, targetRot, turnSpeed * Time.deltaTime);
                    if (Quaternion.Angle(Player3D.rotation, targetRot) < 2)
                    {
                        Player3D.rotation = targetRot;
                        turning = false;
                    }
                }
                else
                {
                    TakeInput();
                }
            }
        }

        public void Initialize(SimLevel level)
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
                //MoveForward();
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
                TurnRight();
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
                TurnLeft();
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
            float y = Player3D.rotation.eulerAngles.y;
            Debug.Log("Rotation: " + y);
            if (y == 0f)
            {
                return "north";
            } else if (y == 90f)
            {
                return "west";
            } else if(y == 180f)
            {
                return "south";
            } else
            {
                return "east";
            }
        }

        // HELLA basic movement controls
        public void MoveForward()
        {
            t = 0;
            curPos = Player3D.position;
            targetPos = Player3D.position - Player3D.forward * 1.0f;
            moving = true;
        }

        public void MoveBackward()
        {
            t = 0;
            curPos = Player3D.position;
            targetPos = Player3D.position + Player3D.forward * 1.0f;
            moving = true;
        }

        void TurnRight()
        {
            targetRot = Player3D.rotation * Quaternion.Euler(0, 90, 0);
            turning = true;
        }

        void TurnLeft()
        {
            targetRot = Player3D.rotation * Quaternion.Euler(0, -90, 0);
            turning = true;
        }
    }



}