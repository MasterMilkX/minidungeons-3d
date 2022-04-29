using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using Newtonsoft.Json;

public enum ViewState
{
    Debug, MainMenu, Playing, LevelWon, LevelLost, Options, ReplayMenu
}

public class HumanGameManager : MonoBehaviour
{
    public bool _debug;
    public bool _heatMapping;

    public static ViewState _viewState = ViewState.MainMenu;

    public bool testPlay;

    public SimLevel currentLevel;
    public string levelNumberString;
    public string currentLevelString;

    public bool PlayingClassic;
    public int ClassicCount;

    public LevelView levelView;
    public SimControllerHeroHuman humanController;

    public GameObject[] _UIInterfaces;
    public GameObject _mainMenu;
    public GameObject _levelResults;
    public GameObject _inGame;
    public GameObject _replayMenu;
    public GameObject _levelEditor;
    public GameObject _levelSelect;
    public GameObject _pauseMenu;
    public GameObject _userstudyMenu;

    public GameObject _thankyou;
    public GameObject _skipTutorialButton;


    public GameObject[] _userstudyQuestions;
    public Text[] _userstudyAnswers;
    public Text _userstudyTitle;
    public Text _userstudyMessage;
    public GameObject _tutorialView;

    public Text emailInputField;
    public Text steps, XP;

    public GameObject _jukebox;

    // Replay related variables;
    [HideInInspector]
    public ReplayManager _ReplayManager;
    public ReplayPlayer _ReplayPlayer;

    public levelEditorManager LEM;

    public bool editMode = false;
    public bool replayMode = false;
    public bool autoPlayMode = false;
    public float replayPauseTime = 0.77f;

    bool tutorialLevel = true;
    int tutorialLevelCounter = 1;
    int levelCounter = 0;
    int map_number;
    Guid myuuid;

    List<HumanResultObject> resultObjects;
    public List<int> maps = new List<int>{
        100,
        101,
        102,
        201,
        202,
    };

    Audio_Manager am;

    // Made SimHeroAction public.
    public SimHeroAction nextAction;

    // messages and titles
    public int userStudyIndex = 0;
    public List<UI_Message> userStudyMessages = new List<UI_Message>() {
        new UI_Message("Welcome To Minidungeons 2", "This user study will measure your playstyle against hundreds of others.\nWe will collect a few bits of anonymous data to do so."),
        new UI_Message("Step 1: User Study", ""),
        new UI_Message("Step 1: User Study", "When playing games with an action element, which of the following statements are true for you:"),
        new UI_Message("Step 1: User Study", ""),
        new UI_Message("Step 1: User Study", ""),
        new UI_Message("Step 1: User Study", ""),
        new UI_Message("Gameplay", "The next three levels are to help introduce you to basic game concepts.\nAfter that, the levels will get harder. Good luck!")
};

    // public UIMessage lastMessage = new UIMessage("Step 3: Gameplay", "These last three levels are more challenging.\nHave fun!");

    // Use this for initialization
    void Start()
    {
#if UNITY_IOS
		_debug = false;
#endif
#if UNITY_ANDROID
		_debug = false;
#endif

        UI_Message userStudyMessageObject = userStudyMessages[0];
        _userstudyMessage.text = userStudyMessageObject.Message;
        _userstudyTitle.text = userStudyMessageObject.Title;

        am = _jukebox.GetComponent<Audio_Manager>();

        myuuid = Guid.NewGuid();

        humanController = GetComponent<SimControllerHeroHuman>();
        resultObjects = new List<HumanResultObject>();

        _ReplayManager = GetComponent<ReplayManager>();
        _ReplayPlayer = GetComponent<ReplayPlayer>();

        //----------- code for level editor---------------
        // get the value from Variables class
        testPlay = Variables.isPlayTest;
        //if it is a test play
        if (testPlay)
        {
            _replayMenu.SetActive(true);
            //load the level 
            LoadLevel(Variables.levelName);
            Debug.Log("data: " + currentLevel.SimHero.Health);
        }
        //----------- code for level editor---------------


        if (editMode) return;

        FlushUI();
        _mainMenu.SetActive(true);
        _viewState = ViewState.MainMenu;


        if (_debug)
        {
            NewGame();
        }


        // load Progress
        GameSaver.LoadGame();
    }

    public void LoadLevelEditor(string levelName, bool startup)
    {

        levelView = GetComponent<LevelView>();
        levelView.ClearView();
        if (startup)
        {
            currentLevelString = Resources.Load<TextAsset>("AsciiMaps/" + levelName).text;
        }
        else
        {
            currentLevelString = Resources.Load<TextAsset>("AsciiMaps/PlayerMaps/" + levelName).text;
        }
        Debug.Log("Level String: " + currentLevelString);
        currentLevel = new SimLevel(currentLevelString);
        levelView.Initialize(currentLevel);
    }

    IEnumerator LoadUpLevel(int levelNum)
    {
        yield return new WaitForSeconds(0.2f);
        _inGame.SetActive(true);
        levelView = GetComponent<LevelView>();
        levelView.ClearView();
        levelView.Initialize(currentLevel);
        humanController.Initialize(currentLevel);

        FlushUI();

        PlayingClassic = true;
        ClassicCount = levelNum;
        _inGame.SetActive(true);
        _viewState = ViewState.Playing;
    }
    public void LoadLevel(int levelNum, bool tutorial = false)
    {
        _inGame.SetActive(false);
        levelNumberString = levelNum.ToString();


        string mapData = "";
        if (!tutorial)
        {
            mapData = Resources.Load<TextAsset>("AsciiMaps/TutorialMaps/Map" + levelNum).text;
        }
        else
        {
            mapData = Resources.Load<TextAsset>("AsciiMaps/Toots/Map" + levelNum).text;
        }
        string finalData = null;
        // split into lines
        string[] lines = mapData.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            //for each lines remove the last blank space
            finalData = finalData + lines[i].Trim();
            if (i != lines.Length - 1)
            {
                finalData = finalData + '\n';
            }
        }
        currentLevel = new SimLevel(finalData);

        StartCoroutine(LoadUpLevel(levelNum));
    }

    public void LoadLevel(string levelString)
    {
        levelView = GetComponent<LevelView>();
        levelView.ClearView();


        StreamReader streamReader = new StreamReader(Application.persistentDataPath + "/" + levelString + ".txt");
        string fileData = null;
        while (!streamReader.EndOfStream)
        {
            //store the data in a string
            fileData += streamReader.ReadLine() + '\n';
        }
        streamReader.Close();
        currentLevelString = fileData.Substring(0, fileData.Length - 1);

        //currentLevelString = Resources.Load<TextAsset>("AsciiMaps/PlayerMaps/" + levelString).text;

        Debug.Log("level : " + currentLevelString);
        currentLevel = new SimLevel(currentLevelString);
        levelView.Initialize(currentLevel);

        humanController.Initialize(currentLevel);
        FlushUI();
        _inGame.SetActive(true);
        _viewState = ViewState.Playing;
    }

    void Update()
    {
        if (_viewState == ViewState.Playing)
        {
            if (currentLevel.SimLevelState == SimLevelState.Playing)
            {
                if (levelView._okForInput)
                {
                    if (replayMode)
                    {
                        if (!autoPlayMode && Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
                        {
                            Attempt currAttempt = _ReplayPlayer.simulationAttempt;
                            if (currAttempt.AttemptReplay.Count < currAttempt.ReplayCounter)
                            {
                                Debug.Log("Counter longer than list. Replay is corrupt.");
                                return;
                            }
                            currentLevel.RunTurn(currAttempt.AttemptReplay[currAttempt.ReplayCounter++].HeroAction);
                            levelView.RefreshView(currentLevel);
                        }
                    }
                    else if (humanController.HasAction)
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
                levelView.sbH.UpdateValue(currentLevel.SimHero.Health);
                levelView.sbT.UpdateValue(currentLevel.SimHero.TreasureCollected);
                levelView.sbM.UpdateValue(currentLevel.GetMonsterKilled());
                steps.text = currentLevel.SimHero.StepsTaken.ToString();
                XP.text = currentLevel.GetMonsterKilled().ToString();

                _inGame.GetComponentInChildren<HeartsAndJavelin>().UpdateHeartsAndJavelin(currentLevel);
            }
            if (currentLevel.SimLevelState == SimLevelState.Won && !_heatMapping && !testPlay)
            {
                am.PlaySFX(5);
                if (replayMode)
                {
                    FlushUI();
                    _replayMenu.SetActive(true);
                    _replayMenu.transform.GetComponentInChildren<UILevelReport>().UpdateTexts(currentLevel, true);
                    _viewState = ViewState.ReplayMenu;
                    return;
                }
                // save our classic map progress if it is a classic map 
                if (PlayingClassic)
                {
                    GameSaver.IncremenentProgress(ClassicCount);
                }
                FlushUI();
                // _levelResults.SetActive(true);
                // _levelResults.transform.GetComponentInChildren<UILevelReport>().UpdateTexts(currentLevel, true);
                Debug.Log("count: " + tutorialLevelCounter + " | " + tutorialLevel);
                if (tutorialLevel && tutorialLevelCounter < 3)
                {
                    // next tutorial
                    tutorialLevelCounter += 1;
                    LoadLevel(tutorialLevelCounter, tutorial: true);

                }
                else if ((tutorialLevel && tutorialLevelCounter >= 3))
                {
                    // get random level and play
                    _skipTutorialButton.SetActive(false);
                    System.Random random = new System.Random();
                    int map_idx = random.Next(maps.Count);
                    tutorialLevel = false;
                    levelCounter += 1;
                    map_number = maps[map_idx];
                    LoadLevel(map_number);
                    maps.RemoveAt(map_idx);
                }
                else if (!tutorialLevel && levelCounter < 3)
                {
                    // get random level and play
                    resultObjects.Add(BuildResultObject());
                    System.Random random = new System.Random();
                    int map_idx = random.Next(maps.Count);
                    tutorialLevel = false;
                    levelCounter += 1;
                    map_number = maps[map_idx];

                    LoadLevel(map_number);
                    maps.RemoveAt(map_idx);

                }
                else
                {
                    resultObjects.Add(BuildResultObject());
                    // Save data
                    SendData();
                    _viewState = ViewState.LevelWon;
                    _inGame.SetActive(false);
                    _thankyou.SetActive(true);

                }

                // SessionReplay _sessionReplay = _ReplayManager.sessionReplay;


                //TODO put this back in
                //StartCoroutine(_sessionReplay.PushDataToDB(_sessionReplay.EngineSession, 1));
            }

            if (currentLevel.SimLevelState == SimLevelState.Lost && !_heatMapping && !testPlay)
            {
                am.PlaySFX(6);
                if (tutorialLevel)
                {
                    LoadLevel(tutorialLevelCounter, tutorial: true);
                }
                else if (!tutorialLevel && levelCounter < 3)
                {
                    // get random level and play
                    System.Random random = new System.Random();
                    int map_idx = random.Next(maps.Count);
                    tutorialLevel = false;
                    levelCounter += 1;
                    LoadLevel(maps[map_idx]);
                    maps.RemoveAt(map_idx);
                    resultObjects.Add(BuildResultObject());
                }
                else
                {
                    resultObjects.Add(BuildResultObject());
                    // Save data
                    SendData();
                    _viewState = ViewState.LevelWon;
                    _inGame.SetActive(false);
                    _thankyou.SetActive(true);
                }
                // StartCoroutine(UploadData());
                // _levelResults.SetActive(true);
                // _levelResults.transform.GetComponentInChildren<UILevelReport>().UpdateTexts(currentLevel, false);
                // _viewState = ViewState.LevelLost;
                // SessionReplay _sessionReplay = _ReplayManager.sessionReplay;

                //TODO: Put this back
                //StartCoroutine(_sessionReplay.PushDataToDB(_sessionReplay.EngineSession, 1));
            }

            //----------- code for level editor---------------
            // if loose the level in play test
            if (currentLevel.SimLevelState == SimLevelState.Lost && !_heatMapping && testPlay)
            {
                _replayMenu.SetActive(true);
                _replayMenu.transform.GetComponentInChildren<UILevelReport>().UpdateTexts(currentLevel, false);
                GoBackToEditor();
            }

            //if win the level in play test
            if (currentLevel.SimLevelState == SimLevelState.Won && !_heatMapping && testPlay)
            {
                _replayMenu.SetActive(true);
                _replayMenu.transform.GetComponentInChildren<UILevelReport>().UpdateTexts(currentLevel, true);
                GoBackToEditor();
            }
            //----------- code for level editor---------------
        }

        if (replayMode && Input.GetKeyDown(KeyCode.P))
        {
            autoPlayMode = !autoPlayMode;
            if (autoPlayMode) StartCoroutine(autoPlay());
        }
        // Replay related functions.
        if (Input.GetKeyDown(KeyCode.DownArrow) && autoPlayMode) changeReplaySpeed(true); // Speeds down.
        else if (Input.GetKeyDown(KeyCode.UpArrow) && autoPlayMode) changeReplaySpeed(false); // Speed up.

        if (Input.GetKeyDown(KeyCode.M) && (_viewState == ViewState.Playing || _viewState == ViewState.MainMenu))
        {
            Mute();
        }
    }

    void OnGUI()
    {
        if (_debug)
        {
            DebugGUI();
        }
    }

    void DebugGUI()
    {
        string guiString = "";
        guiString += "Hero health: " + currentLevel.SimHero.Health + "\n";
        guiString += "Hero gained: " + currentLevel.SimHero.HealthGained + "\n";
        guiString += "Hero lost: " + currentLevel.SimHero.HealthLost + "\n";
        guiString += "Hero steps taken: " + currentLevel.SimHero.StepsTaken + "\n";
        guiString += "Javelins thrown: " + currentLevel.SimHero.JavelinsThrown + "\n";
        foreach (SimGameCharacter character in currentLevel.Characters)
        {
            if (character.CharacterType == GameCharacterTypes.MinitaurMonster)
            {
                var monster = (SimMinitaurMonster)character;
                guiString += "Minitaur knockout: " + monster.KnockoutCounter + "\n";
            }
            if (character.CharacterType == GameCharacterTypes.MeleeMonster || character.CharacterType == GameCharacterTypes.RangedMonster || character.CharacterType == GameCharacterTypes.OgreMonster || character.CharacterType == GameCharacterTypes.BlobMonster)
            {
                var monster = (SimMonsterCharacter)character;
                guiString += monster.CharacterType + " health: " + monster.Health + "\n";
            }
            if (character.CharacterType == GameCharacterTypes.OgreMonster)
            {
                var ogre = (SimOgreMonster)character;
                guiString += "Ogre treasures: " + ogre.TreasureCollected + "\n";
            }
            if (character.CharacterType == GameCharacterTypes.BlobMonster)
            {
                var blob = (SimBlobMonster)character;
                guiString += "Blob potions: " + blob.PotionsCollected + "\n";
                guiString += "Blob merges: " + blob.BlobMerges + "\n";
            }
        }
        GUI.Label(new Rect(0f, 0f, 300f, 1000f), guiString);

        levelNumberString = GUI.TextField(new Rect(Screen.width - 100f, 40f, 100f, 40f), levelNumberString);
        if (GUI.Button(new Rect(Screen.width - 100f, 80f, 100f, 40f), "Go to level"))
        {
            int levelNum = int.Parse(levelNumberString);
            LoadLevel(levelNum);
            humanController.Initialize(currentLevel);
        }
        if (GUI.Button(new Rect(Screen.width - 100f, 120f, 100f, 40f), " Next level"))
        {
            int levelNum = int.Parse(levelNumberString) + 1;
            LoadLevel(levelNum);
            humanController.Initialize(currentLevel);
        }
        if (GUI.Button(new Rect(Screen.width - 100f, 160f, 100f, 40f), "HeatMap"))
        {
            _heatMapping = true;
            GetComponent<HeatMapper>().InitializeAndRender(levelView);
        }
        GetComponent<HeatMapper>()._actionString = GUI.TextField(new Rect(Screen.width - 100f, 200f, 100f, 80f), GetComponent<HeatMapper>()._actionString);
        if (GUI.Button(new Rect(Screen.width - 100f, 280f, 100f, 40f), "Save screenshot"))
        {
            //StartCoroutine(SavePhoto());
            System.DateTime now = System.DateTime.Now;
            string dateTimeFormat = "yyyy-MM-dd_HH-mm-ss";
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/experimentResults/heatmaps/" + now.ToString(dateTimeFormat) + "_SavedHeatmap.png");
        }
        if (GUI.Button(new Rect(Screen.width - 100f, 320f, 100f, 40f), "Clear"))
        {
            GetComponent<HeatMapper>().ClearHeatMap();
        }
        if (GUI.Button(new Rect(Screen.width - 100f, 360f, 100f, 40f), "Auto generate heatmaps"))
        {
            //StartCoroutine(SavePhoto());
            var dir = new DirectoryInfo(Application.dataPath + "/Resources/experimentResults");
            FileInfo[] fileInfos = dir.GetFiles("*actions_run1.txt");
            Debug.Log(fileInfos.Length);
            StartCoroutine(SaveHeatmaps(fileInfos));
        }
    }

    IEnumerator SaveHeatmaps(FileInfo[] fileInfos)
    {
        for (int i = 0; i < fileInfos.Length; i++)
        {
            //for(int i = 0; i < 5; i++){
            FileInfo fileInfo = fileInfos[i];
            string[] splitName = fileInfo.Name.Split('_');
            Debug.Log(splitName[7]);
            string mapName = splitName[7];
            mapName = mapName.Replace("map", "");
            LoadLevel(int.Parse(mapName));
            string fileName = fileInfo.Name;
            fileName = fileInfo.Name.Replace(".txt", "");
            GetComponent<HeatMapper>()._actionString = Resources.Load<TextAsset>("experimentResults/" + fileName).text;
            GetComponent<HeatMapper>().InitializeAndRender(levelView);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/Resources/experimentResults/heatmaps/" + fileInfo.Name + ".png");
            GetComponent<HeatMapper>().ClearHeatMap();
            yield return new WaitForEndOfFrame();
        }
    }

    public void NewGame()
    {
        LoadLevel(1);
        humanController.Initialize(currentLevel);
        FlushUI();
        _inGame.SetActive(true);
        _viewState = ViewState.Playing;

    }
    public void StartLevel(string levelName)
    {
        LoadLevel(levelName);
        humanController.Initialize(currentLevel);
        FlushUI();
        _inGame.SetActive(true);
        _viewState = ViewState.Playing;

    }
    public void TestLevel(string levelName)
    {
        FlushUI();
        _inGame.SetActive(true);
        LoadLevelEditor(levelName, false);
        humanController.Initialize(currentLevel);
        _viewState = ViewState.Playing;
    }

    public void NextLevel()
    {
        int levelToLoad = int.Parse(levelNumberString) + 1;

        if (levelToLoad > 11)
        {
            MainMenu();
            return;
        }
        else if (replayMode)
        {
            FlushUI();
            _inGame.SetActive(true);
            _ReplayPlayer.StartSimulation(levelToLoad.ToString());
            return;
        }
        else
        {
            FlushUI();
            _inGame.SetActive(true);
            LoadLevel(levelToLoad);
            humanController.Initialize(currentLevel);
            _viewState = ViewState.Playing;
        }

    }

    public void MainMenu()
    {
        FlushUI();
        _mainMenu.SetActive(true);
        _viewState = ViewState.MainMenu;
    }

    public void NextUserStudyMessage()
    {
        if (userStudyIndex < userStudyMessages.Count - 1)
        {
            userStudyIndex = 6;
            UI_Message userStudyMessageObject = userStudyMessages[userStudyIndex];
            _userstudyMessage.text = userStudyMessageObject.Message;
            _userstudyTitle.text = userStudyMessageObject.Title;

            FlushQuestions();
            if (userStudyIndex == 1)
            {
                _userstudyQuestions[0].SetActive(true);
            }
            else if (userStudyIndex == 3)
            {
                _userstudyQuestions[1].SetActive(true);
                _userstudyQuestions[2].SetActive(true);
                _userstudyQuestions[3].SetActive(true);
            }
            else if (userStudyIndex == 4)
            {
                _userstudyQuestions[4].SetActive(true);
                _userstudyQuestions[5].SetActive(true);
                _userstudyQuestions[6].SetActive(true);
            }
            else if (userStudyIndex == 5)
            {
                _userstudyQuestions[7].SetActive(true);
                _userstudyQuestions[8].SetActive(true);
                _userstudyQuestions[9].SetActive(true);
            }
        }
        else
        {
            // play first tutorial level

            // get random level and play
            // System.Random random = new System.Random();
            // int map_idx = random.Next(maps.Count);

            LoadLevel(1, tutorial: true);
        }
    }

    public void FlushQuestions()
    {
        foreach (GameObject question in _userstudyQuestions)
        {
            if (question.activeSelf)
            {

                question.SetActive(false);
            }
        }
    }

    public void Retry()
    {
        int levelToLoad = int.Parse(levelNumberString);
        FlushUI();
        _inGame.SetActive(true);
        LoadLevel(levelToLoad);
        humanController.Initialize(currentLevel);
        _viewState = ViewState.Playing;

        if (replayMode)
        {
            autoPlayMode = false;
            _ReplayPlayer.NextSimulation(_ReplayPlayer.simulationAttempt.AttemptNumber);
        }
    }

    public void RetryNextAttempt()
    {
        int levelToLoad = int.Parse(levelNumberString);
        FlushUI();
        _inGame.SetActive(true);
        LoadLevel(levelToLoad);
        humanController.Initialize(currentLevel);
        _viewState = ViewState.Playing;
        autoPlayMode = false;

        if (_ReplayPlayer.replayAttemptCount + 1 > _ReplayPlayer.attempts.Count)
        {
            Debug.Log("No new attempt. First attempt has been loaded.");
            _ReplayPlayer.replayAttemptCount = 1;
            _ReplayPlayer.NextSimulation(_ReplayPlayer.replayAttemptCount);
            return;
        }
        _ReplayPlayer.NextSimulation(_ReplayPlayer.replayAttemptCount + 1);
    }

    public void Options()
    {

    }

    public void LevelEditor()
    {
        FlushUI();
        //_levelEditor.SetActive(true);

        Destroy(GameObject.Find("LevelView"));
        enabled = false;
        SceneManager.LoadScene("LevelEditorScene");
    }

    public void Mute()
    {

        am.ToggleMute();
    }
    public IEnumerator autoPlay()
    {
        while (currentLevel.SimLevelState == SimLevelState.Playing && autoPlayMode)
        {
            Attempt currAttempt = _ReplayPlayer.simulationAttempt;
            currentLevel.RunTurn(currAttempt.AttemptReplay[currAttempt.ReplayCounter++].HeroAction);
            levelView.RefreshView(currentLevel);
            yield return new WaitForSeconds(replayPauseTime);
        }
        yield return null;
    }

    // Replay related functions
    public void changeReplaySpeed(bool downPress)
    {
        float stepSize = 0.15f;
        if (0.01f > (replayPauseTime - stepSize) && !downPress)
        {
            replayPauseTime = 0.04f;
            Debug.Log(replayPauseTime);
            return;
        }
        replayPauseTime += (downPress) ? stepSize : -stepSize;
        Debug.Log(replayPauseTime);
    }

    public void ShowLevelSelect()
    {
        FlushUI();
        _levelSelect.SetActive(true);
    }

    public void Restart()
    {
        Application.ExternalEval("document.location.reload(true)");
    }

    public void SubmitEmail()
    {
        // Debug.Log("submitted email: " + emailInputField.text);
        // StartCoroutine(UploadEmail());
        Restart();
        // SceneManager.LoadScene("mvcTest");

    }
    public void SkipTutorial()
    {
        _skipTutorialButton.SetActive(false);

        tutorialLevelCounter = 3;
        currentLevel.SimLevelState = SimLevelState.Won;
    }
    IEnumerator UploadEmail()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", emailInputField.text);

        using (UnityWebRequest www = UnityWebRequest.Post("http://128.122.34.211:5000/emails", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
    public void SendData()
    {
        Debug.Log("Beginning form sending...");
        StartCoroutine(UploadData());
    }
    IEnumerator UploadData()
    {
        TotalResultObject tro = new TotalResultObject(resultObjects);

        // convert json string to byte
        var formData = JsonConvert.SerializeObject(tro);
        Debug.Log(formData);

        using (UnityWebRequest www = UnityWebRequest.Put("http://128.122.34.211:5000/results", formData))
        {

            www.SetRequestHeader("Content-Type", "application/json");
            Debug.Log("Submitted request");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }

    }

    public HumanResultObject BuildResultObject()
    {
        List<Mech> mechanics = currentLevel.Logger.Mechanics;
        foreach (Mech mechanic in mechanics)
        {
            Debug.Log(mechanic);
        }

        Dictionary<Mechanic, int> frequencies = currentLevel.Logger.GetFrequencies();
        foreach (KeyValuePair<Mechanic, int> kvp in frequencies)
        {
            Debug.Log("Key = " + kvp.Key + ", Value = " + kvp.Value);
        }
        // Debug.Log(frequencies);
        LevelReport levelReport = humanController.GetResults(currentLevel);
        List<SimPoint> positions = humanController.positions;
        List<string> levelStates = humanController.levelStates;


        HumanResultObject hro = new HumanResultObject(mechanics, frequencies, levelReport, positions, levelStates, map_number);
        return hro;

    }
    void FlushUI()
    {
        PlayingClassic = false;
        foreach (GameObject face in _UIInterfaces)
        {
            face.SetActive(false);
        }
    }

    public void ShowPauseMenu()
    {

    }

    public void HidePauseMenu()
    {

    }

    //----------- code for level editor---------------


    //action listener for quit btn
    public void OnQuit()
    {
        // if quit btn is pressed while play testing
        if (testPlay)
        {
            GoBackToEditor();
        }
        else
        {

            FlushUI();
            _mainMenu.SetActive(true);
            _viewState = ViewState.MainMenu;
        }
    }

    // quit the play mode, go back to the level editor scene
    void GoBackToEditor()
    {
        humanController.JavelinOff();
        FlushUI();
        _viewState = ViewState.MainMenu;
        Destroy(GameObject.Find("LevelView"));
        enabled = false;
        Variables.isPlayTest = false;
        Variables.playTested = true;
        SceneManager.LoadScene("LevelEditorScene");
    }
    //----------- code for level editor---------------

    public void Close()
    {
        Application.Quit();
    }
}
