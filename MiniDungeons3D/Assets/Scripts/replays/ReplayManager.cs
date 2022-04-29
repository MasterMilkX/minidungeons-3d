// Author: Batu Aytemiz
// Replay Manager.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Bugs: If the player tries to move during Loss screen it increments attempts.
// Timing of the attempted query is pretty bad. Should be refactored to be a coroutine instead.

// SessionReplay -> (in Engine/UIQueue) Attempt -> (in AttemptReplayQueue) ReplayData
// Creating a the basic replay data structure. Currently it has the HeroAction (move/javelin) and timestamp.
public struct ReplayData {
    
    public SimHeroAction HeroAction { get; set; }
    public long TimeStamp { get; set; }
    public long TimeSpent { get; set; }

    public ReplayData(SimHeroAction heroAction) {
        HeroAction = heroAction;
        TimeStamp = ReplayUtils.GetEpochMili();
        TimeSpent = 0;
    }

    public int GetX() {
        return HeroAction.DirectionOrTarget.X;
    }

    public int GetY() {
        return HeroAction.DirectionOrTarget.Y;
    }

    public string GetAction() {
        return HeroAction.ActionType.ToString();
    }

    public override string ToString() {
        return (TimeStamp.ToString() + ", " + HeroAction);
    }
}

// Attempt is the data structure that has the replay information for any given attempt (one level, one life)
public class Attempt {

    public List<ReplayData> AttemptReplay { get; set; }
    public long AttemptStart { get; set; }
    public int AttemptLevel { get; set; }
    public int AttemptNumber { get; set; }
    public long TimeCheck { get; set; }
    public int ReplayCounter { get; set; }

    public Attempt(int attemptLevel, int attemptNumber) {
        AttemptReplay = new List<ReplayData>();
        AttemptStart = ReplayUtils.GetEpochMili();
        AttemptLevel = attemptLevel;
        AttemptNumber = attemptNumber;
        TimeCheck = ReplayUtils.GetEpochMili();
        ReplayCounter = 0;
    }

     public override string ToString() {
        return ("This is level:" + AttemptLevel.ToString() + " attempt:" + AttemptNumber.ToString() + ".");
    }
}

// Session replay is where all the ReplayData objects are sorted and stored. It keeps track of two Dictionaries, one for engine actions and one for UI actions.
// Each level and attempt has its own Queue that stores the corresponding replayDatas.
public class SessionReplay : MonoBehaviour {

    public bool levelChanged = true;
    public bool levelLost = false;

    public bool replaying = false;

    public Stack<Attempt> EngineSession { get; set; }
    public Stack<Attempt> UISession { get; set; }

    public long SessionStart { get; set; }
    public int currentLevel { get; set; }
    public string GUID { get; set; }

    int levelAttempt = 1;
    SqliteDatabase sqlDB;


    public void constructor(SqliteDatabase SQLDB, string guid) {
        EngineSession = new Stack<Attempt>();
        UISession = new Stack<Attempt>();
        SessionStart = ReplayUtils.GetEpochMili();
        sqlDB = SQLDB;
        GUID = guid;
    }

    public IEnumerator PushDataToDB(Stack<Attempt> session, int IsEngine) {

        if (replaying) yield break;
        if (IsEngine == 0) yield break;

        string attemptQuery, replayQuery;

        int attemptLevel, attemptNumber,  directionX, directionY;
        long attemptStartTime, timeStamp, timeSpent;
        string attemptUID, action;

        int yieldcounter = 0;

        while (session.Count != 0) {
            
            Attempt attempt = session.Pop();

            attemptLevel = attempt.AttemptLevel;
            attemptNumber = attempt.AttemptNumber;
            attemptStartTime = attempt.AttemptStart;
            attemptUID = GUID + attemptStartTime.ToString();

            Debug.Log(attemptUID);
            attemptQuery = String.Format("INSERT INTO Attempt (AttemptID, Level, AttemptNumber, StartTime, isEngineReplay) VALUES ( '{0}',{1},{2},'{3}',{4});",
            attemptUID, attemptLevel, attemptNumber, attemptStartTime.ToString(), IsEngine);

            sqlDB.ExecuteNonQuery(attemptQuery);
            List<ReplayData> replayList = attempt.AttemptReplay;

            int counter = 0;
            int yieldTreshold = attempt.AttemptReplay.Count / 2;
            while (attempt.ReplayCounter < attempt.AttemptReplay.Count) {
                yieldcounter++;
                if(yieldcounter == yieldTreshold) {
                    yield return null;
                    yieldcounter = 0;
                }

                Debug.Log(counter++);
                ReplayData replayData = replayList[attempt.ReplayCounter++];
                action = replayData.GetAction();
                directionX = replayData.GetX();
                directionY = replayData.GetY();
                timeStamp = replayData.TimeStamp;
                timeSpent = replayData.TimeSpent;


                replayQuery = String.Format("INSERT INTO ReplayData (AttemptID, Action, DirectionX, DirectionY, TimeStamp, TimeSpent) VALUES ( '{0}','{1}',{2},{3}, '{4}', {5});",
                attemptUID, action, directionX, directionY, timeStamp.ToString(), (-1 * timeSpent));
                sqlDB.ExecuteNonQuery(replayQuery);
            }
            DataTable test = sqlDB.ExecuteQuery("SELECT * FROM ReplayData;");
            Debug.Log(test.Rows.Count);
        }
    }

    // Adds a new attempt for the respsective sessions. 
    void NewAttempt() {
        EngineSession.Push(new Attempt(currentLevel, levelAttempt));
        UISession.Push(new Attempt(currentLevel, levelAttempt));
    }

    // Adds the action to the respective attempt with a timestamp.
    public void AddEngineAction(SimHeroAction nextAction) {

        ReplayData replayData = new ReplayData(nextAction);
        long TimeSpent = UISession.Peek().TimeCheck - replayData.TimeStamp;
        UISession.Peek().TimeCheck = ReplayUtils.GetEpochMili();
        replayData.TimeSpent = TimeSpent;
        EngineSession.Peek().AttemptReplay.Add(replayData);

    }

    public void AddUIAction(SimHeroAction nextAction) {
        if (levelChanged) {
            levelAttempt = 1;
            NewAttempt();
            levelChanged = false;
        }
        if (levelLost) {
            levelAttempt++;
            levelLost = false;
            NewAttempt();
        }
        ReplayData replayData = new ReplayData(nextAction);
        long TimeSpent = UISession.Peek().TimeCheck - replayData.TimeStamp;
        UISession.Peek().TimeCheck = ReplayUtils.GetEpochMili();
        replayData.TimeSpent = TimeSpent;
        UISession.Peek().AttemptReplay.Add(replayData);
    }
}

public class ReplayManager : MonoBehaviour {

    HumanGameManager _HumanGameManager;
    SimControllerHeroHuman _SimControllerHeroHuman;
    ReplayPlayer _replayPlayer;
    DeviceHash _DeviceHash;

    public SessionReplay sessionReplay;
    public SqliteDatabase sqlDB;


    public GameObject inputField;
    GameObject replayManager;

    bool replayMode = false;

    string GUID;


    // Use this for initialization
    void Start () {

        _DeviceHash = new DeviceHash(Application.persistentDataPath);
        GUID = _DeviceHash.Get();

        sqlDB = new SqliteDatabase("replays.db");
        replayManager = new GameObject();
        replayManager.name = "SessionReplay";
        sessionReplay = replayManager.AddComponent<SessionReplay>();
        sessionReplay.constructor(sqlDB, GUID);

        _HumanGameManager = GetComponent<HumanGameManager>();
        _SimControllerHeroHuman = GetComponent<SimControllerHeroHuman>();
        _replayPlayer = GetComponent<ReplayPlayer>();


    }

    // Starts the replay mode by setting some bools.
    void StartReplay() {
        inputField.SetActive(true);
        _replayPlayer.enabled = true;
        _SimControllerHeroHuman.replayMode = true;
        _HumanGameManager.replayMode = true;
    }

	public void VisualizeReplay()
	{
		_replayPlayer.enabled = true;
        _SimControllerHeroHuman.replayMode = true;
        _HumanGameManager.replayMode = true;
	}
    // Makes sure all the states are correct.
    void Update () {
        if (_SimControllerHeroHuman.Initialized) {
            if(sessionReplay.currentLevel.ToString() != _HumanGameManager.levelNumberString) {
                sessionReplay.currentLevel = int.Parse(_HumanGameManager.levelNumberString);
                sessionReplay.levelChanged = true;
            }
            if (_HumanGameManager.currentLevel.SimLevelState == SimLevelState.Lost) {
                sessionReplay.levelLost = true;
            }
        }

        //starts the debug mode. Use this in the MENU.
        if (Input.GetKey(KeyCode.R)) {
			Debug.Log("Pressed R");
            StartReplay();
            sessionReplay.replaying = true;
        }
    }
}
