using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SimControllerHeroHuman : MonoBehaviour
{

    public bool Initialized;
    public bool HasAction;
    public bool replayMode = false;

    public List<SimHeroAction> actions { get; set; }

    public ReplayManager _ReplayManager;

    //Made the nextAction public;
    public SimHeroAction nextAction;

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

    void Start()
    {
        _ReplayManager = GetComponent<ReplayManager>();

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
        JavelinOff();
        actions.Add(nextAction);
        return nextAction;
    }

    // Update is called once per frame
    void Update()
    {
        if (Initialized && true)
        {
            TakeInput();
        }
    }




    void ClickOrTapInput(Vector2 input)
    {
        if (HumanGameManager._viewState == ViewState.Playing)
        {
            if (javelinActivated)
            {
                //javelin throw 
                if (_level.SimHero.Javelins > 0)
                {
                    LevelView levelView = FindObjectOfType<LevelView>();
                    Vector3 input3 = new Vector3(input.x, input.y, 0f);
                    Vector3 worldPoint = Camera.main.ScreenToWorldPoint(input3);
                    for (int x = 0; x < levelView._map.GetLength(0); x++)
                    {
                        for (int y = 0; y < levelView._map.GetLength(1); y++)
                        {
                            Vector3 min = levelView._map[x, y].GetComponent<Renderer>().bounds.min;
                            Vector3 max = levelView._map[x, y].GetComponent<Renderer>().bounds.max;
                            if (worldPoint.x > min.x && worldPoint.x < max.x)
                            {
                                if (worldPoint.y > min.y && worldPoint.y < max.y)
                                {
                                    var point = new SimPoint(x, y);
                                    var possibleTargets = _level.GetPossibleJavelinTargets();
                                    foreach (SimPoint otherPoint in possibleTargets)
                                    {
                                        if (otherPoint == point)
                                        {
                                            // is javelin even toggled to use?
                                            if (JavelinToggle)
                                            {
                                                Debug.Log("OK to throw javelin");
                                                nextAction = new SimHeroAction(HeroActionTypes.JavelinThrow, point);
                                                HasAction = true;
                                                //ToggleJavelin();
                                                javelinActivated = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                //disable any previous highlight
                if (highlighted)
                {
                    DisableHighlight();
                }

                LevelView levelView = FindObjectOfType<LevelView>();
                Vector3 input3 = new Vector3(input.x, input.y, 0f);
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(input3);
                for (int x = 0; x < levelView._map.GetLength(0); x++)
                {
                    for (int y = 0; y < levelView._map.GetLength(1); y++)
                    {
                        Vector3 min = levelView._map[x, y].GetComponent<Renderer>().bounds.min;
                        Vector3 max = levelView._map[x, y].GetComponent<Renderer>().bounds.max;
                        if (worldPoint.x > min.x && worldPoint.x < max.x)
                        {
                            if (worldPoint.y > min.y && worldPoint.y < max.y)
                            {
                                var point = new SimPoint(x, y);
                                //get all charecters in game
                                var allCharecters = _level.GetAllCharecters();
                                int i = 0;
                                //check for each charecter
                                foreach (SimGameCharacter monster in _level.Characters)
                                {
                                    //if the charecter is clicked or not
                                    if (point == monster.Point)
                                    {

                                        TriggerMonsterInfo(i, levelView, input);
                                    }
                                    i++;
                                }
                            }
                        }
                    }

                }
            }

        }
    }

    void TriggerMonsterInfo(int i, LevelView levelView, Vector2 input)
    {
        //get the game object
        GameObject gm = levelView._gameCharacters[i];
        spriteRenderer = gm.GetComponent<SpriteRenderer>();
        //highlight the object
        oldColor = spriteRenderer.color;
        spriteRenderer.color = HighlightColor;

        highlighted = true;
        highlightedObject = gm;


        //get tapped object name
        string selected = gm.name;
        selected = selected.Replace("_new(Clone)", "");
        selected = selected.Replace("(Clone)", "");
        string data = null;

        HumanGameManager hgm = levelView.GetComponent<HumanGameManager>();
        //do according tapped object
        if (selected.Equals("potion"))
        {
            GetTotalCount(hgm.currentLevel, selected);
            data = selected + "\ncollected : " + hgm.currentLevel.SimHero.PotionsCollected + "/" + potion;
        }
        else if (selected.Equals("chest"))
        {
            GetTotalCount(hgm.currentLevel, selected);
            data = selected + "\ncollected : " + hgm.currentLevel.SimHero.TreasureCollected + "/" + treasure;
        }
        else if (selected.Equals("entrance") || selected.Equals("exit") || selected.Equals("portal") || selected.Equals("trap"))
        {
            data = selected;
        }
        else
        {
            data = selected + "\nhealth : " + hgm.currentLevel.SimHero.Health + "/ 10 ";
        }

        //activate the info panel and set the position
        panel.SetActive(true);
        if (input.y < 400)
        {
            panel.transform.position = new Vector3(input.x, input.y + 100, 0);
        }
        else
        {
            panel.transform.position = new Vector3(input.x, input.y - 100, 0);
        }
        textBox.GetComponent<Text>().text = data;
        tap = true;
        // yield return new WaitForSeconds(3.0f);

        // DisableHighlight();
    }


    //disable highlight
    void DisableHighlight()
    {
        // StopCoroutine("TriggerMonsterInfo");
        spriteRenderer.color = oldColor;
        highlighted = false;
        highlightedObject = null;
        panel.SetActive(false);
        tap = false;
    }

    //get total no of potion / chest in game
    private void GetTotalCount(SimLevel level, string element)
    {
        potion = 0;
        treasure = 0;
        foreach (SimGameCharacter character in level.Characters)
        {
            if (character.CharacterType == GameCharacterTypes.Treasure && element.Equals("chest"))
            {
                treasure++;
            }
            if (character.CharacterType == GameCharacterTypes.Potion && element.Equals("potion"))
            {
                potion++;
            }
        }
    }

    void UpdateOutline(bool outline)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", Color.white);
        spriteRenderer.SetPropertyBlock(mpb);
    }


    public void TakePhoneTap(TKTapRecognizer r)
    {
        Debug.Log(r);
        ClickOrTapInput(r.touchLocation());

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
            if (!javelinActivated)
            {
                nextAction = (new SimHeroAction(HeroActionTypes.Move, SimPoint.North));
                HasAction = true;
            }
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
            if (javelinActivated)
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
                HasAction = true;
                // try
                // {
                //     _ReplayManager.sessionReplay.AddUIAction(nextAction);
                // }
                // catch
                // {
                // }
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!javelinActivated)
            {
                nextAction = (new SimHeroAction(HeroActionTypes.Move, SimPoint.South));
                HasAction = true;
            }
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
            if (javelinActivated)
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
                //This sad workaround is temporary.
                // try
                // {
                //     _ReplayManager.sessionReplay.AddUIAction(nextAction);
                // }
                // catch
                // {
                // }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
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
        }



        /*if(Input.GetMouseButtonDown(0)){
                //Debug.Log(Input.mousePosition);
                //Vector3 mouse = Input.mousePosition;
                //ClickOrTapInput(new Vector2(mouse.x,mouse.y));
            }*/

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            JavelinOff();

            // else
            // {
            //     LevelView levelView = FindObjectOfType<LevelView>();
            //     levelView.ClearView();
            //     HumanGameManager hgm = levelView.GetComponent<HumanGameManager>();
            //     hgm.MainMenu();
            // }
        }
    }


    public void GetJavelinTargets()
    {
        if (_level.SimHero.Javelins > 0)
        {
            // refresh the list
            javTargets = new GameObject[_level.Characters.Length];
            // refresh the counter for javTargets
            int javCounter = 0;
            targets = new List<GameObject>();
            targetPoints = new List<SimPoint>();
            // grab list of targets
            var possibleTargets = _level.GetPossibleJavelinTargets();
            // for ever target, store their z order and raise them above the dimmer
            LevelView levelView = FindObjectOfType<LevelView>();
            foreach (SimPoint target in possibleTargets)
            {
                //Debug.Log ("*****************");
                GameObject cTile = levelView._map[target.X, target.Y];
                //Debug.Log ("Target: " + target);
                // search the monster list for monsters
                // TODO: make a helper function that does this all for us...or better yet - a mapping maybe?
                // i is a counter variable
                int i = 0;
                foreach (SimGameCharacter monster in _level.Characters)
                {
                    //Debug.Log ("Monster: " + monster);
                    // compare monster simpoints to our targets, if they match..
                    if (target == monster.Point && monster.Alive && (monster.GetType() == typeof(SimBlobMonster) ||
                        monster.GetType() == typeof(SimMeleeMonster) || monster.GetType() == typeof(SimMinitaurMonster) ||
                        monster.GetType() == typeof(SimOgreMonster) || monster.GetType() == typeof(SimRangedMonster)))
                    {
                        //Debug.Log ("Found monster object!");
                        // swipe the game character index
                        if (monster.GetType() == typeof(SimMinitaurMonster))
                        {
                            SimMinitaurMonster minitaur;
                            minitaur = (SimMinitaurMonster)monster;
                            if (minitaur.KnockoutCounter != 0)
                            {
                                break;
                            }
                        }
                        GameObject gm = levelView._gameCharacters[i];
                        gm.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        // TODO: save local z-order
                        //add object and its location to target list
                        targets.Add(gm);
                        targetPoints.Add(monster.Point);
                        // add gm to javTargets
                        javTargets[javCounter] = gm;
                        javCounter++;
                        break;
                    }
                    i++;
                }

            }
            if (targets.Count > 0)
            {
                targets.Add(null);
            }
        }
        else
        {
            targets = new List<GameObject>();
        }
    }


    public void JavelinOn()
    {
        //discard any highlight
        if (highlighted)
        {
            DisableHighlight();
        }

        //activate javelin
        javelinActivated = true;
        _dimmer.SetActive(true);
        _dimmer.transform.position = transform.position;
        _dimmer.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .7f);

        targetSelected = 0;
        ShowTarget(0);
    }

    public void JavelinOff()
    {
        //deactivate javelin
        javelinActivated = false;
        //discard any highlight
        if (highlighted)
        {
            DisableHighlight();
        }

        // set the dimmer inactive
        _dimmer.SetActive(false);
        // loop through z order edited targets and set back to 0
        foreach (GameObject target in javTargets)
        {
            if (target != null)
            {
                target.GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
        }
        javText.GetComponent<Text>().text = "";
    }

    void ShowTarget(int c)
    {
        if (highlightedObject != null)
        {
            GameObject lastTarget = highlightedObject;
            spriteRenderer = lastTarget.GetComponent<SpriteRenderer>();
            spriteRenderer.color = oldColor;
        }

        GameObject target = targets[c];
        if (target != null)
        {
            highlightedObject = target;
            spriteRenderer = target.GetComponent<SpriteRenderer>();
            oldColor = spriteRenderer.color;
            spriteRenderer.color = TargetColor;
            highlighted = true;
            javText.GetComponent<Text>().text = "Target selected";
        }
        else
        {
            highlightedObject = null;
            javText.GetComponent<Text>().text = "Nothing selected";
        }
    }


    /*public void OnJavelinPress()
    {
        //discard any highlight
        if (highlighted)
        {
            DisableHighlight();
        }
        if (_level.SimHero.Javelins > 0)
        {
            //if javelin is not activated
            if (!JavelinToggle)
            {
                //activate the javelin 
                ToggleJavelin();
                javelinActivated = true;
            }
            else
            {
                //deactivate the javelin 
                ToggleJavelin();
                javelinActivated = false;
            }
        }
    }*/

    public LevelReport GetResults(SimLevel level)
    {
        // ReportObject ro = new ReportObject();
        LevelReport levelReport = SimControllerHero.LevelReport(GetType().Name, null, 0.0f, 0.0f, level.SimHero.StepsTaken, level);
        return levelReport;
    }
}
