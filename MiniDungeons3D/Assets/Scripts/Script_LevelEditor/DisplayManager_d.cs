using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TreeLanguageEvolute;

public class DisplayManager_d : MonoBehaviour
{
    public InputManager _inputManager;
    ResultCompare _resultCompare;

    public static string currentLevel = null;

    public GameObject[] _UIInterfaces;
    public GameObject _levelEditMenuScn, _newLevelScn, _selectLevelScn, _editorScn,
        _editOptionPanel, _newLevelMsgPanel, _renamePanel, _deletePanel, _selectLevelScroll, _testOptnPanel, _inputPanel, _resultPanel, _playTestResult, _autoTestResult, _editorMsgPanel, _hintPanel;

    public Text _msgNewLevel, _msgRename, _titleNewLevel, _selectedLevel, _titleEditLevel, _levelCreated, _levelChanged, _levelDelete, _playError, _health, _monster, _minitaure, _treasure, _potion, _step, time_a, health_a, healthLost_a, alive_a, javelin_a,
        monster_a, minitaure_a, treasure_a, potion_a, levelComplete_a, _hintText;

    public Button _backNewLevelBtn, _createLevelBtn, _backEditLevelBtn, _backResultBtn;

    public InputField _levelRename, _newLevelName;

    public Toggle _playTestTab, _autoTestTab, _runnerTab, _monsterKillerTab, _treasureCollectorTab, _complitionistTab;

    [HideInInspector]
    public bool edit = false;

    string selectedLevelName, newLevelName, path, val;

    public SimLevel testLevel;

    void Start()
    {
        // if the scene is loading after play testing a level 
        if (Variables.playTested)
        {
            edit = true;
            // go tomthe editor scene
            ShowScene(_editorScn);
            // load the last edited leve;
            currentLevel = Variables.levelName;
            //set play test to false
            Variables.playTested = false;
            SetPlayTestData();
            SetAutoTestData(Variables.runner);
        }
        // if it is a new load go to start scene
        else
        {
            FlushUI();
            _levelEditMenuScn.SetActive(true);
            //remove previous variables
            Destroy(GameObject.Find("Variables"));
        }
        _resultCompare = GetComponent<ResultCompare>();
    }

    // show the LevelEditMenu scene
    public void ShowLevelEditMenu()
    {
        ShowScene(_levelEditMenuScn);
    }

    // show the NewLevel scene
    public void ShowCreateLevel()
    {
        ShowScene(_newLevelScn);
    }

    // show the EditLevel scene
    public void ShowLevelList()
    {
        ShowScene(_selectLevelScn);
    }

    // listener for all Back buttons
    public void OnBack()
    {
        if (_levelEditMenuScn.active)
        {
            FlushUI();
            SceneManager.LoadScene("mvcTest");
        }
        else if (_newLevelScn.active && !_newLevelMsgPanel.active)
        {
            FlushUI();
            _newLevelName.text = "";
            _levelEditMenuScn.SetActive(true);
        }
        else if (_newLevelScn.active && _newLevelMsgPanel.active)
        {
            FlushUI();
            _titleNewLevel.gameObject.SetActive(true);
            _newLevelName.gameObject.SetActive(true);
            _createLevelBtn.gameObject.SetActive(true);
            _backNewLevelBtn.gameObject.SetActive(true);
            _newLevelScn.gameObject.SetActive(true);
            _newLevelMsgPanel.SetActive(false);
        }
        else if (_selectLevelScn.active && !_editOptionPanel.active)
        {
            FlushUI();
            _testOptnPanel.SetActive(false);
            _levelEditMenuScn.SetActive(true);
        }
        else if (_selectLevelScn.active && _editOptionPanel.active)
        {
            FlushUI();
            _titleEditLevel.gameObject.SetActive(true);
            _backEditLevelBtn.gameObject.SetActive(true);
            _selectLevelScroll.SetActive(true);
            _selectLevelScn.SetActive(true);
            _editOptionPanel.SetActive(false);
        }
        else if (_editorScn.active && !_inputPanel.active && !_resultPanel.active)
        {
            FlushUI();
            _testOptnPanel.SetActive(false);
            _levelEditMenuScn.SetActive(true);
            //reset all test data
            Variables.ResetVariables();
            SetPlayTestData();
            SetAutoTestData(null);
            // if input page is activated
            if (_inputManager != null)
            {
                _inputManager.ResetInput();
            }
        }
        else if (_editorScn.active && _inputPanel.active)
        {
            _inputPanel.SetActive(false);

        }
        else if (_editorScn.active && _resultPanel.active)
        {
            _testOptnPanel.SetActive(false);
            _resultPanel.SetActive(false);
            _playTestTab.isOn = true;
            _autoTestTab.isOn = false;
            _runnerTab.isOn = true;
            _monsterKillerTab.isOn = false;
            _treasureCollectorTab.isOn = false;
            _complitionistTab.isOn = false;
            _hintText.text = "";
            _hintPanel.SetActive(false);
            _backResultBtn.GetComponent<Button>().interactable = true;
        }
    }


    //fires when any level is selected for editing in _selectLevelScrollView
    public void ShowEditOption(string lavelName)
    {
        selectedLevelName = lavelName;
        _titleEditLevel.gameObject.SetActive(false);
        _backEditLevelBtn.gameObject.SetActive(false);
        _selectLevelScroll.SetActive(false);
        _editOptionPanel.SetActive(true);
        _selectedLevel.text = lavelName + " selected";
        currentLevel = lavelName;
    }


    // listener for CreateLevelBtn
    public void OnCreateLevel()
    {
        newLevelName = _newLevelName.text;

        // if level name field  is blank
        if (newLevelName.Length == 0)
        {
            _levelCreated.text = "*Blank field.";
            StartCoroutine(Disable(_levelCreated));
            return;
        }

        // if entered level name already exists
        string filePath = Application.persistentDataPath + "/" + newLevelName + ".txt";
        if (System.IO.File.Exists(filePath))
        {
            _msgNewLevel.text = "Map " + newLevelName + " exists. Want to override?";
            _titleNewLevel.gameObject.SetActive(false);
            _newLevelName.gameObject.SetActive(false);
            _createLevelBtn.gameObject.SetActive(false);
            _backNewLevelBtn.gameObject.SetActive(false);
            _newLevelMsgPanel.SetActive(true);
        }
        else
        {
            _newLevelName.text = "";
            //for unity editor
            //System.IO.File.Copy (@"./Assets/Resources/AsciiMaps/Map0-Clean.txt",@"./Assets/Resources/AsciiMaps/PlayerMaps/" + newLevelName + ".txt");
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh ();
#endif
            string emptyFile = Resources.Load<TextAsset>("AsciiMaps/Map0-Clean").text;
            File.WriteAllText(Application.persistentDataPath + "/" + newLevelName + ".txt", emptyFile);

            currentLevel = newLevelName;
            ShowScene(_editorScn);
        }
    }


    // listener for Override Btn
    public void OnOverride()
    {
        _titleNewLevel.gameObject.SetActive(true);
        _newLevelName.gameObject.SetActive(true);
        _createLevelBtn.gameObject.SetActive(true);
        _backNewLevelBtn.gameObject.SetActive(true);
        _newLevelMsgPanel.SetActive(false);

        //delete the existing file 
        string pathToFile = Application.persistentDataPath + "/" + newLevelName + ".txt";
        File.Delete(pathToFile);

        //for unity editor refresh the database
#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh ();
#endif
        _newLevelName.text = "";
        // create a new file with the same name
        string emptyFile = Resources.Load<TextAsset>("AsciiMaps/Map0-Clean").text;
        File.WriteAllText(Application.persistentDataPath + "/" + newLevelName + ".txt", emptyFile);

#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh ();
#endif

        currentLevel = newLevelName;
        ShowScene(_editorScn);
    }


    // listener for Edit Btn
    public void OnEdit()
    {
        edit = true;
        _titleEditLevel.gameObject.SetActive(true);
        _backEditLevelBtn.gameObject.SetActive(true);
        _selectLevelScroll.SetActive(true);
        _editOptionPanel.SetActive(false);
        ShowScene(_editorScn);
    }


    // listener for Delete Btn
    public void OnDelete()
    {
        _deletePanel.gameObject.SetActive(true);
        _editOptionPanel.gameObject.SetActive(false);
        _levelDelete.text = "Delete " + selectedLevelName + " ?";
    }

    // listener for delete no button
    public void OnDeleteNo()
    {
        _levelDelete.text = "";
        _deletePanel.gameObject.SetActive(false);
        _editOptionPanel.gameObject.SetActive(true);
    }

    // listener for delete yes button
    public void OnDeleteYes()
    {
        string pathToFile = Application.persistentDataPath + "/" + selectedLevelName + ".txt";
        File.Delete(pathToFile);

        //for unity editor refresh the database
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        _deletePanel.SetActive(false);
        _titleEditLevel.gameObject.SetActive(true);
        _backEditLevelBtn.gameObject.SetActive(true);
        _selectLevelScroll.SetActive(true);
        ShowLevelList();
        _levelChanged.text = "Level " + selectedLevelName + " deleted sucessfully.";
        StartCoroutine(Disable(_levelChanged));
    }

    // listener for Rename Btn
    public void OnClickRename()
    {
        _renamePanel.gameObject.SetActive(true);
        _editOptionPanel.gameObject.SetActive(false);
    }


    // listener for rename yes button
    public void OnRenameYes()
    {
        string levelRename = _levelRename.text;

        if (levelRename.Length == 0)
        {
            _msgRename.text = "*Blank field";
            StartCoroutine(Disable(_msgRename));
            return;
        }
        string filePath = Application.persistentDataPath + "/" + levelRename + ".txt";
        if (System.IO.File.Exists(filePath))
        {
            _msgRename.text = "Map " + levelRename + " exists. Use another name";

        }
        else
        {
            string oldFile = Application.persistentDataPath + "/" + selectedLevelName + ".txt";
            string newFile = Application.persistentDataPath + "/" + levelRename + ".txt";
            File.Move(oldFile, newFile);

            _levelRename.text = "";
            //for unity editor refresh the database
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh ();
#endif
            _renamePanel.SetActive(false);
            _titleEditLevel.gameObject.SetActive(true);
            _backEditLevelBtn.gameObject.SetActive(true);
            _selectLevelScroll.SetActive(true);
            ShowLevelList();
            _levelChanged.text = "Level " + selectedLevelName + " renamed to " + levelRename;
            StartCoroutine(Disable(_levelChanged));
        }
    }


    // listener for rename yes button
    public void OnRenameNo()
    {
        _levelRename.text = "";
        _renamePanel.gameObject.SetActive(false);
        _editOptionPanel.gameObject.SetActive(true);
    }


    // listener for text change in rename input
    public void OnTextChange()
    {
        _msgRename.text = "";
    }

    // show the given scene
    public void ShowScene(GameObject scene)
    {
        FlushUI();
        scene.SetActive(true);
    }

    void FlushUI()
    {
        foreach (GameObject face in _UIInterfaces)
        {
            face.SetActive(false);
        }
    }


    // disable object after 3 seconds
    IEnumerator Disable(Text labelText)
    {
        yield return new WaitForSeconds(2.0f);
        labelText.text = "";

        if (labelText == _playError)
        {
            //deactivate the error message panel
            _editorMsgPanel.SetActive(false);
        }
    }

    //returns the currently selected file name
    public string GetFile()
    {
        return currentLevel;
    }

    // returns true if an existing fileis editing else returns false
    public bool IsEdit()
    {
        return edit;
    }

    //listener for the play button in editor screen
    public void OnPlayTest()
    {
        //check for errors in map
        bool isError = CkeckForError();
        //if there is error
        if (isError)
        {
            return;
        }
        else
        {
            Variables.isPlayTest = true;
            Variables.levelName = currentLevel;
            SceneManager.LoadScene("mvcTest");
        }
    }

    // listener for play btn
    public void OnPlay()
    {
        _testOptnPanel.SetActive(true);
    }

    // listener for generate btn
    public void OnGenerate()
    {
        _inputPanel.SetActive(true);
    }

    // listener for auto test btn(code for runner only)
    public void OnAutoTest()
    {
        //check for errors in map
        bool isError = CkeckForError();
        //if there is error
        if (isError)
        {
            return;
        }
        else
        {
            // Runner Test
            testLevel = new SimLevel(GetMapText(currentLevel));
            string controllerType = "MCTS";
            string utilityFunction = "R";
            string msPerMove = "10000";
            string maxMovesPerTurn = "200";
            string numRuns = "1";

            PathDatabase.levels = new System.Collections.ArrayList();
            PathDatabase.BigDatabase = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, SimPoint[]>>();
            PathDatabase.Farthest = new System.Collections.Generic.Dictionary<SimLevel, Pair>();
            PathDatabase.BuildDB(testLevel);

            SimSentientSketchbookInterface minidungeons2 = new SimSentientSketchbookInterface();

            // ...this commented out line does not work in android....
            //TreeProgram function = minidungeons2.BuildFunction ("Assets/Resources/MCTSEvolvedAgents/GenProgram99R.txt");

            //code for android
            string file = "MCTSEvolvedAgents/GenProgram99R";
            TreeProgram function = minidungeons2.BuildFunction(file);
            MapReport report = minidungeons2.RunController(testLevel.LevelString, controllerType, utilityFunction, function);
            Debug.Log(report.ActionReport);
            Debug.Log(report.LevelReport);
            Variables.runner = report.LevelReport.runnerUtility + "";
            Attempt runnerAttempt = new Attempt(0, 1);

            string utilityScore = report.LevelReport.utilityGained + "";
            Debug.Log(utilityScore);

            // Treasure Collector Test
            testLevel = new SimLevel(GetMapText(currentLevel));
            controllerType = "MCTS";
            utilityFunction = "TC";
            msPerMove = "10000";
            maxMovesPerTurn = "200";
            numRuns = "1";

            minidungeons2 = new SimSentientSketchbookInterface();
            // ...this code does not work for android....
            //function = minidungeons2.BuildFunction ("Assets/Resources/MCTSEvolvedAgents/GenProgram99TC.txt");

            //code for android
            file = "MCTSEvolvedAgents/GenProgram99TC";
            function = minidungeons2.BuildFunction(file);
            report = minidungeons2.RunController(testLevel.LevelString, controllerType, utilityFunction, function);
            Debug.Log(report.ActionReport);
            Debug.Log(report.LevelReport);
            Variables.treasureCollertor = report.LevelReport.treasureCollectorUtility + "";
            runnerAttempt = new Attempt(0, 1);

            utilityScore = report.LevelReport.utilityGained + "";
            Debug.Log(utilityScore);


            // Monster Killer Test
            testLevel = new SimLevel(GetMapText(currentLevel));
            controllerType = "MCTS";
            utilityFunction = "MK";
            msPerMove = "10000";
            maxMovesPerTurn = "200";
            numRuns = "1";

            minidungeons2 = new SimSentientSketchbookInterface();

            // ...this code does not work for android....
            //function = minidungeons2.BuildFunction ("Assets/Resources/MCTSEvolvedAgents/GenProgram99MK.txt");

            //code for android
            file = "MCTSEvolvedAgents/GenProgram99MK";
            function = minidungeons2.BuildFunction(file);
            report = minidungeons2.RunController(testLevel.LevelString, controllerType, utilityFunction, function);
            Debug.Log(report.ActionReport);
            Debug.Log(report.LevelReport);
            Variables.monsterKiller = report.LevelReport.monsterKillerUtility + "";
            runnerAttempt = new Attempt(0, 1);

            utilityScore = report.LevelReport.utilityGained + "";
            Debug.Log(utilityScore);

            // Completionist
            testLevel = new SimLevel(GetMapText(currentLevel));
            controllerType = "MCTS";
            utilityFunction = "C";
            msPerMove = "10000";
            maxMovesPerTurn = "200";
            numRuns = "1";

            minidungeons2 = new SimSentientSketchbookInterface();
            // ...this code does not work for android....
            //function = minidungeons2.BuildFunction ("Assets/Resources/MCTSEvolvedAgents/GenProgram99C.txt");

            //code for android
            file = "MCTSEvolvedAgents/GenProgram99C";
            function = minidungeons2.BuildFunction(file);
            report = minidungeons2.RunController(testLevel.LevelString, controllerType, utilityFunction, function);
            Debug.Log(report.ActionReport);
            Debug.Log(report.LevelReport);
            Variables.complitionist = report.LevelReport.completionistUtility + "";
            runnerAttempt = new Attempt(0, 1);

            utilityScore = report.LevelReport.utilityGained + "";
            Debug.Log(utilityScore);

            //disable the panel
            _testOptnPanel.SetActive(false);
            //activate the panel
            _editorMsgPanel.gameObject.SetActive(true);
            //show message
            _playError.text = "Test completed";
            //disable message after 3 secs
            StartCoroutine(Disable(_playError));
            SetAutoTestData(Variables.runner);
        }
    }

    //checks for error in map
    bool CkeckForError()
    {
#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh ();
#endif
        string error = null;
        //read the file in a string
        string content = File.ReadAllText(Application.persistentDataPath + "/" + currentLevel + ".txt");
        //check if the file contains "Hero"
        if (!content.Contains("H"))
        {
            //if not set an error message
            error = "No Hero in the map";
        }
        //check if the file contains "Exit"
        if (!content.Contains("e"))
        {
            //if not set an error message
            error += " No Exit in the map";
        }
        //if there are error message
        if (error != null)
        {
            //activate the error message panel
            _editorMsgPanel.gameObject.SetActive(true);
            //display the error
            _playError.text = error;
            //disable the message after 3 secs
            StartCoroutine(Disable(_playError));
            _testOptnPanel.SetActive(false);
            return true;
        }
        else
        {
            return false;
        }
    }

    // listener for play btn
    public void OnTestData()
    {
        _resultPanel.SetActive(true);
    }

    // set data of test result
    public void SetPlayTestData()
    {
        _health.text = Variables._healthLeft;
        _monster.text = Variables._monstersKilled;
        _minitaure.text = Variables._minitaursKnockedOut;
        _treasure.text = Variables._treasuresCollected;
        _potion.text = Variables._potionsDrunk;
        _step.text = Variables._stepsTaken;
    }

    // when Play test tab is selected
    public void PlayTestTab(bool value)
    {
        if (value)
        {
            _playTestResult.SetActive(true);
            _autoTestResult.SetActive(false);
        }
    }

    //when auto test tab is selected
    public void AutoTestTab(bool value)
    {
        if (value)
        {
            _autoTestResult.SetActive(true);
            _playTestResult.SetActive(false);
        }
    }

    //when runner tab is selected
    public void RunnerTab(bool value)
    {
        if (value)
        {
            SetAutoTestData(Variables.runner);
        }
    }

    //when monster killer tab is selected
    public void MonsterKillerTab(bool value)
    {
        if (value)
        {
            SetAutoTestData(Variables.monsterKiller);
        }
    }

    //when treasure collector tab is selected
    public void TreasureCollectorTab(bool value)
    {
        if (value)
        {
            SetAutoTestData(Variables.treasureCollertor);
        }
    }

    //when completionist tab is selected
    public void CompletionistTab(bool value)
    {
        if (value)
        {
            SetAutoTestData(Variables.complitionist);
        }
    }

    //set data of auto test
    public void SetAutoTestData(string result)
    {
        if (result != null)
        {
            int[] element = CountElenemts();
            string[] data = result.Split(',');
            //time_a.text = data [1];
            health_a.text = data[9];
            healthLost_a.text = data[10];
            alive_a.text = data[11];
            javelin_a.text = data[12];
            monster_a.text = data[13] + "/" + element[0];
            minitaure_a.text = data[14];
            treasure_a.text = data[15] + "/" + element[1];
            potion_a.text = data[16] + "/" + element[2];
            if (data[2] == 1.ToString())
            {
                levelComplete_a.text = "Yes";
            }
            else
            {
                levelComplete_a.text = "No";
            }
        }
        else
        {
            //time_a.text = "";
            health_a.text = "";
            healthLost_a.text = "";
            alive_a.text = "";
            javelin_a.text = "";
            monster_a.text = "";
            minitaure_a.text = "";
            treasure_a.text = "";
            potion_a.text = "";
            levelComplete_a.text = "";
        }
    }

    int[] CountElenemts()
    {
        int monster = 0;
        int potion = 0;
        int treasure = 0;
#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh ();
#endif
        //read the file in a string
        string content = File.ReadAllText(Application.persistentDataPath + "/" + currentLevel + ".txt");
        foreach (char c in content)
        {
            if (c == 'P')
            {
                potion++;
            }
            if (c == 'T')
            {
                treasure++;
            }
            if (c == 'b' || c == 'R' || c == 'o' || c == 'M')
            {
                monster++;
            }
        }
        int[] result = new int[3];
        result[0] = monster;
        result[1] = treasure;
        result[2] = potion;
        return result;
    }

    // listener for hint Btn
    public void OnHint()
    {
        string hint = _resultCompare.Compare();
        //show the hint panel
        _hintPanel.SetActive(true);
        _hintText.text = hint;
        _backResultBtn.GetComponent<Button>().interactable = false;

    }
    //listener for close btn
    public void OnClose()
    {
        _hintPanel.SetActive(false);
        _backResultBtn.GetComponent<Button>().interactable = true;
    }

    //returns map data
    string GetMapText(string level)
    {
        string finalData = null;
        StreamReader streamReader = new StreamReader(Application.persistentDataPath + "/" + level + ".txt");
        string fileData = null;
        while (!streamReader.EndOfStream)
        {
            //store the data in a string
            fileData += streamReader.ReadLine() + '\n';
        }
        streamReader.Close();
        finalData = fileData.Substring(0, fileData.Length - 1);
        return finalData;
    }
}



