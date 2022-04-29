using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ReplayPlayer : MonoBehaviour {

    //Keycodes used for getting the number keys from the keyboard. Should be changed for input field.
    private KeyCode[] keyCodes = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
     };

    bool started = false;

    public int replayAttemptCount = 1;

    GameObject inputField;

    LevelView _LevelView;
    HumanGameManager _HumanGameManager;
    ReplayManager _ReplayManager;

    SqliteDatabase sqlDB;
    int levelNumber;
    public float replayPauseTime = 0.30f;

    public List<Attempt> attempts = new List<Attempt>();

    // THIS IS WHERE THE ACTIVE ATTEMPT LIVES. Engine reads this attempt when working through the simulation.
    public Attempt simulationAttempt;
    
	
    // Does the necesariy instantiations.
	void Start () {
        _LevelView = GetComponent<LevelView>();
        _ReplayManager = GetComponent<ReplayManager>();
        _HumanGameManager = GetComponent<HumanGameManager>();
        inputField = _ReplayManager.inputField;
        sqlDB = _ReplayManager.sqlDB;
    }
    
    // Jumps to the desired attempt number in a given level. Currently input taken from keyboard, has to change.
    public void changeAttemptNumber(int attemptNoInput) {
        //int attemptNoInput = int.Parse(attemptText);
        if (attemptNoInput > 0 && attemptNoInput <= attempts.Count) {
            int attemptNumber = attemptNoInput;
            NextSimulation(attemptNumber);
        } else {
            Debug.Log("Desired attempt cant be found in level. Please enter an attempt number less than " + (attempts.Count + 1).ToString());
        }
    }

    // Starts the replay by loading the given level. 
    public void StartSimulation(string levelText) {
        started = true;
        replayAttemptCount = 1;
        _LevelView.Replaying = true;
        int attemptNumber = 0;
        _HumanGameManager.NewGame();
        levelNumber = int.Parse(levelText);
        createSimAttemptsList(levelNumber);
        simulationAttempt = attempts[attemptNumber];
        simulateAttempt(simulationAttempt);
    }

    // Starts a new level but this time uses a changed attempt number.
    public void NextSimulation(int attemptNumber) {
        _HumanGameManager.NewGame();
        simulationAttempt = attempts[attemptNumber - 1];
        replayAttemptCount = attemptNumber;
        simulationAttempt.ReplayCounter = 0;
        simulateAttempt(simulationAttempt);
        Debug.Log(string.Format("Attempt number {0} has been loaded.", replayAttemptCount));
    }


    // Creates the Attempts list for the given level.
    void createSimAttemptsList(int level) {

        // Cleans the list if a previous list exists (an earlier load for a different level for example)
        if (attempts.Count > 0) attempts.Clear();

        // Grabs the required level from DB.
        string attemptQuery = String.Format("SELECT * FROM Attempt WHERE Level == {0}", level);
        DataTable attemptData = sqlDB.ExecuteQuery(attemptQuery);

        // Creates new Attemps and puts them into the list.
        foreach (DataRow attempt_row in attemptData.Rows) {

            int attemptLevel = (int)attempt_row["Level"];
            //Debug.Log("Printing attempt level: " + attemptLevel.ToString());
            int attemptNumber = (int)attempt_row["AttemptNumber"];
            string attemptID = (string)attempt_row["AttemptID"];

            string replayQuery = String.Format("SELECT * FROM ReplayData WHERE AttemptID == '{0}'", attemptID);
            DataTable replayData = sqlDB.ExecuteQuery(replayQuery);
            if(replayData.Rows.Count == 0) continue;

            Attempt replayAttempt = new Attempt(attemptLevel, attemptNumber);
            attempts.Add(replayAttempt);

            foreach (DataRow data_row in replayData.Rows) {

                if (attemptID == (string)data_row["AttemptID"]) {
                    ReplayData simReplay = createReplayData(data_row);
                    replayAttempt.AttemptReplay.Add(simReplay);
                } else {
                    break;
                }
            }
        }
//        foreach (Attempt attempt in attempts) {
//            Debug.Log(attempt);
//        }
        Debug.Log(String.Format("There are {0} attempts for this level.", attempts.Count));
    }

    // Creates ReplayData class from the raw input from the DB.
    ReplayData createReplayData(DataRow row) {

        // If row[Action] is equal to move actionType is HeroActionTypes.Move else .JavelinThrow
        HeroActionTypes actionType = ((string)row["Action"] == "Move") ? HeroActionTypes.Move : HeroActionTypes.JavelinThrow;
        SimPoint simPoint = new SimPoint((int)row["DirectionX"], (int)row["DirectionY"]);

        SimHeroAction heroAction = new SimHeroAction(actionType, simPoint);
        ReplayData simReplay = new ReplayData(heroAction);
        simReplay.TimeStamp = Int64.Parse((string)row["TimeStamp"]);
        simReplay.TimeSpent = (int)row["TimeSpent"];
        return simReplay;
    }

    // takes an attempt and in the engine loads that attempt.
    void simulateAttempt(Attempt attempt) {
        _HumanGameManager.LoadLevel(attempt.AttemptLevel);
    }

    // Update is called once per frame
    void Update() {
       
        for (int i = 0; i < keyCodes.Length; i++) {
            if (Input.GetKeyDown(keyCodes[i]) && started) {
                int numberPressed = i + 1;
                changeAttemptNumber(numberPressed);
            }
        }
    }
}
