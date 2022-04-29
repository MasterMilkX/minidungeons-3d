using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using System.Collections;
using System.IO;
using TreeLanguageEvolute;

public class levelEditorManager : MonoBehaviour
{

    //Scripts
    HumanGameManager _HumanGameManager;
    editorUtils _editorUtils;

    // UI elements
    public LevelView levelView;
    public Dropdown myDropdown;
    public Text levelNameInput;
    public Canvas editUI;
    public Text RunnerMonstersKilledUI;



    // Instantiated game objects
    public GameObject Enterance, Exit, Wall, Potion, Chest, Trap, Portal, Melee, Ranged, Blob, Ogre, Minitaur, Empty;

    //Book keeping variables
    string levelName;
    SimLevel currentLevel;
    bool playing = false;
    bool isWall = false;

    // Variables that designate which is the currently active tile
    GameObject activeSelection;
    char activeASCII;

    //Checks so that every level has the correct number of Portals, enterances, exits
    [HideInInspector]
    public int activeEnteranceCount = 0, activePortalCount = 0, activeExitCount = 0;

    [HideInInspector]
    public GameObject tempPortal = null;


    // Use this for initialization
    void Start()
    {
        // This is a simple map with just outside borders.
        levelName = "Map0-Clean";


        myDropdown.onValueChanged.AddListener(delegate
        {
            changeActive(myDropdown);
        });

        activeSelection = Enterance;
        activeASCII = 'H';

        _HumanGameManager = levelView.GetComponent<HumanGameManager>();
        _editorUtils = GetComponent<editorUtils>();

        _HumanGameManager.LoadLevelEditor(levelName, true);


        currentLevel = _HumanGameManager.currentLevel;

        activePortalCount = currentLevel.Portals.Count;
        activeExitCount = currentLevel.startingExitCount;
        activeEnteranceCount = currentLevel.startingEnteranceCount;

    }



    // Update is called once per frame
    void Update()
    {
        if (playing == true)
        {
            editUI.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                placeTile(getTile());
            }

        }
    }

    public void SetDropdownIndex(int index)
    {
        myDropdown.value = index;
    }

    // This is linked to the dropdown menu with the delegation on line 44.
    // This function basically allows you to select the current active tile type.
    public void changeActive(Dropdown target)
    {

        // It manipulates the activeSelection global (I am sorry) variables which and active ASCII variable which are later on used to PlaceTile function
        switch (target.value)
        {
            case 0:
                activeSelection = Enterance;
                activeASCII = 'H';
                break;
            case 1:
                activeSelection = Exit;
                activeASCII = 'e';
                break;
            case 2:
                activeSelection = Wall;
                isWall = true;
                activeASCII = 'X';
                break;
            case 3:
                activeSelection = Empty;
                isWall = false;
                activeASCII = ' ';
                break;
            case 4:
                activeSelection = Blob;
                activeASCII = 'b';
                break;
            case 5:
                activeSelection = Melee;
                activeASCII = 'M';
                break;
            case 6:
                activeSelection = Ranged;
                activeASCII = 'R';
                break;
            case 7:
                activeSelection = Ogre;
                activeASCII = 'o';
                break;
            case 8:
                activeSelection = Minitaur;
                activeASCII = 'm';
                break;
            case 9:
                activeSelection = Chest;
                activeASCII = 'T';
                break;
            case 10:
                activeSelection = Potion;
                activeASCII = 'P';
                break;
            case 11:
                activeSelection = Trap;
                activeASCII = 't';
                break;
            case 12:
                activeSelection = Portal;
                activeASCII = 'p';
                break;
            default:
                Debug.Log("Default");
                break;
        }
    }
    //The function to map from screen space to tile space. Fires off whenever a mouse is clicked. Gives the tile location in XY with top left being (0,0). Z is always 0.
    Vector3 getTile()
    {

        int activeTileX, activeTileY;
        float tileSize, sideOffset;

        Vector3 tileLocation;

        // Make sure to change this. Currently testing is done with FWGA Portrait 480x854 Portrait mode. The black bars should dissappear in the actual game.
        sideOffset = 25f;

        int verticalTileCount = 20;

        //Find the individual tile size representation on screen by dividing the screen height by number of tiles. Make sure to take offset into account if it exists.
        tileSize = Screen.height / verticalTileCount;

        activeTileX = Mathf.FloorToInt((Input.mousePosition.x - sideOffset) / tileSize);
        activeTileY = Mathf.FloorToInt(Input.mousePosition.y / tileSize);

        // Set top left as (0,0)
        tileLocation = new Vector3(activeTileX, verticalTileCount - activeTileY, 0);
        return tileLocation;
    }

    bool checkPlayable()
    {

        if (activeExitCount != 1)
        {
            Debug.Log("There is a problem with exit count.");
            return false;
        }

        if (activePortalCount == 1 || activePortalCount > 2)
        {
            Debug.Log("There is a problem with portal count.");
            return false;
        }

        return true;
    }

    void placeTile(Vector3 givenLocation)
    {

        // First you need to give the map a name. Right now this is debugging to console, it would be nice to have it in the game.
        if (levelName == "Map0-Clean")
        {
            Debug.Log("Please give your level a name to continiue.");
            return;
        }

        // Can not manipulate the outer walls.
        if (givenLocation.x <= 0 || givenLocation.x >= 9 || givenLocation.y <= 1 || givenLocation.y >= 20)
        {
            Debug.Log("You can not change the outer walls.");
            return;
        }

        //Checks for the correct number of portals.
        if (activeSelection == Portal)
        {
            if (activePortalCount >= 2)
            {
                Debug.Log("Too many portals, you can only have two. Please delete one.");
                return;
            }
            activePortalCount++;
        }

        if (activeSelection == Enterance)
        {
            if (activeEnteranceCount >= 1)
            {
                Debug.Log("Too many enterances, you can only have one. Please delete one.");
                return;
            }
            activeEnteranceCount++;
        }

        if (activeSelection == Exit)
        {
            if (activeExitCount >= 1)
            {
                Debug.Log("Too many enterances, you can only have one. Please delete one.");
                return;
            }
            activeExitCount++;
        }

        //Different from the pixelSize, this time it is the actual transform size in the engine.
        float tileSize = 0.2f;

        // The offsets are so that the bottom left is 0,0 not an arbitrary number. 
        // The -0.1f is so that the center of the sprite fits the center of the tile.
        float horizontalOffset = 1f - 0.1f;
        float verticalOffset = 2f - 0.1f;

        // use this to convert it from top left being 0,0 
        int verticalTileCount = 20;

        Vector3 location = new Vector3((givenLocation.x) * tileSize - horizontalOffset, (verticalTileCount - (givenLocation.y)) * tileSize - verticalOffset, 0);


        if (activeSelection == Portal && activePortalCount == 1)
        {
            tempPortal = Instantiate(activeSelection, location, Quaternion.identity) as GameObject;
        }
        else if (activeSelection == Portal && activePortalCount == 2)
        {
            Destroy(tempPortal);
        }
        else if (activeSelection == Empty && activePortalCount == 1)
        {
            // This code is stupid. Note to self refactor the boolean logic when you are not sleep deprived.
        }
        else if (((activeSelection != Portal && activeSelection != Empty) && activePortalCount == 1))
        {
            Debug.Log("Please finish putting down the second portal before you put down other objects");
            Debug.Log("Or delete both of the portals to continiue. The editor will delete when both of them are deleted");
            return;
        }



        /*
        if (activeSelection == Wall && isWall) {
            GameObject placedWall = Instantiate(activeSelection, location, Quaternion.identity) as GameObject;
            Tile _tile = placedWall.GetComponent<Tile>();
            _tile.TileType = TileTypes.wall;
        }else if (activeSelection == Wall && !isWall)  {
            GameObject placedEmpty = Instantiate(activeSelection, location, Quaternion.identity) as GameObject;
            Tile _tile = placedEmpty.GetComponent<Tile>();
            _tile.TileType = TileTypes.empty;
        } else {
            Instantiate(activeSelection, location, Quaternion.identity);
        }
        */

        // Save the ascii on to the map.
        placeOnASCII(givenLocation);
        return;
    }

    public void ReloadLevel()
    {
        levelName = levelNameInput.text;

        if (!System.IO.File.Exists(@"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt"))
        {
            Debug.Log("Map Does Not Exist. Creating new map.");
            System.IO.File.Copy(@"./Assets/Resources/AsciiMaps/Map0-Clean.txt",
                                @"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt");
            Debug.Log("Map Copying complete...");
        };

#if UNITY_EDITOR
		// This line will give us some problems in the future. Because it only runs in editor. I need to find a replacement on mobile.
		AssetDatabase.Refresh();
#endif

        _HumanGameManager.LoadLevelEditor(levelName, false);
        currentLevel = _HumanGameManager.currentLevel;
    }

    public void testCurrentLevel()
    {
        // Check playable is broken...
        if (true)
        {  //checkPlayable()) {
            playing = true;
            _HumanGameManager.TestLevel(levelName);
            return;
        }
        Debug.Log("Current level not playable!");
    }

    //This is the code that actually writes the changes to the text file that we use.
    void placeOnASCII(Vector3 givenLocation)
    {


        if (!System.IO.File.Exists(@"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt"))
        {
            System.IO.File.Copy(@"./Assets/Resources/AsciiMaps/Map0-Clean.txt",
                                @"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt");
        };

        string source = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";
        string dest = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";
        _editorUtils.changeLine(source, dest, (int)givenLocation.y - 1, (int)givenLocation.x, activeASCII);

#if UNITY_EDITOR
		AssetDatabase.Refresh();
#endif

        if (activePortalCount == 0 || activePortalCount == 2) _HumanGameManager.LoadLevelEditor(levelName, false);
    }

    public void DiggerGenerateMap(int type)
    {
        Creator dg = new DiggerGenerator();
        dg.GenerateMap();
        if (type == 1)
        {
            Furnisher f = new ConstrainFurnisher();
            f.GenerateMap(dg.GetMap());
            // overwrite the file with this map
            if (!System.IO.File.Exists(@"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt"))
            {
                System.IO.File.Copy(@"./Assets/Resources/AsciiMaps/Map0-Clean.txt",
                                    @"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt");
            };

            string source = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";
            string dest = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";

            _editorUtils.ChangeFile(source, dest, f.GetMap());
        }
        else if (type == 2)
        {
            Furnisher f = new CAFurnisher();
            f.GenerateMap(dg.GetMap());
            // overwrite the file with this map
            if (!System.IO.File.Exists(@"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt"))
            {
                System.IO.File.Copy(@"./Assets/Resources/AsciiMaps/Map0-Clean.txt",
                                    @"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt");
            };

            string source = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";
            string dest = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";

            _editorUtils.ChangeFile(source, dest, f.GetMap());
        }
        else if (type == 3)
        {
            Furnisher f = new GrammarFurnisher();
            f.GenerateMap(dg.GetMap());
            // overwrite the file with this map
            if (!System.IO.File.Exists(@"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt"))
            {
                System.IO.File.Copy(@"./Assets/Resources/AsciiMaps/Map0-Clean.txt",
                                    @"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt");
            };

            string source = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";
            string dest = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";

            _editorUtils.ChangeFile(source, dest, f.GetMap());
        }
        else if (type == 4)
        {
            Furnisher f = new AgentFurnisher();
            f.GenerateMap(dg.GetMap());
            // overwrite the file with this map
            if (!System.IO.File.Exists(@"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt"))
            {
                System.IO.File.Copy(@"./Assets/Resources/AsciiMaps/Map0-Clean.txt",
                                    @"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt");
            };

            string source = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";
            string dest = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";

            _editorUtils.ChangeFile(source, dest, f.GetMap());

        }
        CleanMe();
        ReloadLevel();
    }

    public void CellularGenerateMap(int type)
    {
        Creator ca = new CA();
        ca.GenerateMap();
        if (type == 1)
        {
            Furnisher f = new ConstrainFurnisher();
            f.GenerateMap(ca.GetMap());
            // overwrite the file with this map
            if (!System.IO.File.Exists(@"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt"))
            {
                System.IO.File.Copy(@"./Assets/Resources/AsciiMaps/Map0-Clean.txt",
                                    @"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt");
            };

            string source = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";
            string dest = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";

            _editorUtils.ChangeFile(source, dest, f.GetMap());
        }
        else if (type == 2)
        {

            Furnisher f = new CAFurnisher();
            f.GenerateMap(ca.GetMap());
            // overwrite the file with this map
            if (!System.IO.File.Exists(@"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt"))
            {
                System.IO.File.Copy(@"./Assets/Resources/AsciiMaps/Map0-Clean.txt",
                                    @"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt");
            };

            string source = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";
            string dest = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";

            _editorUtils.ChangeFile(source, dest, f.GetMap());
        }
        else if (type == 3)
        {
            Furnisher f = new GrammarFurnisher();
            f.GenerateMap(ca.GetMap());
            // overwrite the file with this map
            if (!System.IO.File.Exists(@"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt"))
            {
                System.IO.File.Copy(@"./Assets/Resources/AsciiMaps/Map0-Clean.txt",
                                    @"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt");
            };

            string source = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";
            string dest = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";

            _editorUtils.ChangeFile(source, dest, f.GetMap());
        }
        CleanMe();
        ReloadLevel();
    }
    public void GrammarGenerateMap(int type)
    {
        Creator gg = new GrammarGenerator();
        gg.GenerateMap();
        if (type == 1)
        {
            Furnisher f = new ConstrainFurnisher();
            f.GenerateMap(gg.GetMap());
            // overwrite the file with this map
            if (!System.IO.File.Exists(@"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt"))
            {
                System.IO.File.Copy(@"./Assets/Resources/AsciiMaps/Map0-Clean.txt",
                                    @"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt");
            };

            string source = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";
            string dest = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";

            _editorUtils.ChangeFile(source, dest, f.GetMap());
        }
        else if (type == 2)
        {
            Furnisher f = new CAFurnisher();
            f.GenerateMap(gg.GetMap());
            // overwrite the file with this map
            if (!System.IO.File.Exists(@"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt"))
            {
                System.IO.File.Copy(@"./Assets/Resources/AsciiMaps/Map0-Clean.txt",
                                    @"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt");
            };

            string source = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";
            string dest = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";

            _editorUtils.ChangeFile(source, dest, f.GetMap());
        }
        else if (type == 3)
        {
            Furnisher f = new GrammarFurnisher();
            f.GenerateMap(gg.GetMap());
            // overwrite the file with this map
            if (!System.IO.File.Exists(@"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt"))
            {
                System.IO.File.Copy(@"./Assets/Resources/AsciiMaps/Map0-Clean.txt",
                                    @"./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt");
            };

            string source = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";
            string dest = "./Assets/Resources/AsciiMaps/PlayerMaps/" + levelName + ".txt";

            _editorUtils.ChangeFile(source, dest, f.GetMap());
        }
        CleanMe();
        ReloadLevel();
    }
    void CleanMe()
    {
        activeEnteranceCount = 0;
        activePortalCount = 0;
        activeExitCount = 0;
    }

    /// <summary>
    /// Runs the runner agent on this map.
    /// </summary>
    public void RunnerAgentPlaythrough()
    {
        string controllerType = "MCTS";
        string utilityFunction = "R";
        string msPerMove = "10000";
        string maxMovesPerTurn = "200";
        string numRuns = "1";

        PathDatabase.levels = new System.Collections.ArrayList();
        PathDatabase.BigDatabase = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, SimPoint[]>>();
        PathDatabase.Farthest = new System.Collections.Generic.Dictionary<SimLevel, Pair>();
        PathDatabase.BuildDB(currentLevel);

        SimSentientSketchbookInterface minidungeons2 = new SimSentientSketchbookInterface();
        TreeProgram function = minidungeons2.BuildFunction("Assets/Resources/MCTSEvolvedAgents/GenProgram99R.txt");
        MapReport report = minidungeons2.RunController(currentLevel.LevelString, controllerType, utilityFunction, function);
        Debug.Log(report.ActionReport);
        Debug.Log(report.LevelReport);
        Attempt runnerAttempt = new Attempt(0, 1);

        LevelReport levelInfo = report.LevelReport;

        double utilityScore = levelInfo.utilityGained;
        RunnerMonstersKilledUI.text = utilityScore + "";
        Debug.Log(utilityScore);
    }

    public string EvaluateLevel(string controllerType, string utilityFunction, string msPerMove, string maxMovesPerTurn, string numRuns)
    {
        SimSentientSketchbookInterface miniDungeons2 = new SimSentientSketchbookInterface();
        GeneticProgrammingEngine genEngine = new GeneticProgrammingEngine();
        // number of steps taken
        genEngine.DeclareVariable("stepsTaken");
        // number of monsters slain
        genEngine.DeclareVariable("monstersSlain");
        // number of potions taken
        genEngine.DeclareVariable("potionsTaken");
        // number of treasure chests opened
        genEngine.DeclareVariable("treasuresOpened");
        // number of minitaur knockouts
        genEngine.DeclareVariable("minotaurKnockouts");
        // number of javelins thrown
        genEngine.DeclareVariable("javelinThrows");
        // number of teleports used
        genEngine.DeclareVariable("teleportsUsed");
        // number of traps sprung
        genEngine.DeclareVariable("trapsSprung");
        // distance from exit
        genEngine.DeclareVariable("distanceFromExit");
        // health left
        genEngine.DeclareVariable("healthLeft");
        // reward in the node
        genEngine.DeclareVariable("rReward");
        // interactable ratio
        genEngine.DeclareVariable("interactableRatio");
        genEngine.SetFunctions = new FunctionType[]
        { new Add (), new Substract (), new Multiply (), new Divide () };
        genEngine.SetValues = new TreeLanguageEvolute.ValueType[] { new RandomMacro() };
        Debug.Log(Directory.GetCurrentDirectory());
        TreeProgram function = genEngine.LoadProgram(@"Assets/Resources/MCTSEvolvedAgents/GenProgram99R.txt");

        PathDatabase.BuildDB(currentLevel);
        return miniDungeons2.EvaluateMap(currentLevel.LevelString, "label", controllerType, utilityFunction, msPerMove, maxMovesPerTurn, numRuns, function);
    }

    public static string SplitLevelString(string oneDLevel)
    {
        return oneDLevel.Replace('|', '\n');
    }
}
