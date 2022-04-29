using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HTTPManager : MonoBehaviour {

    string url;

    LevelView _LevelView;
    HumanGameManager _HumanGameManager;
    ReplayManager _ReplayManager;

    SqliteDatabase sqlDB;


    void Start() {
        _LevelView = GetComponent<LevelView>();
        _ReplayManager = GetComponent<ReplayManager>();
        _HumanGameManager = GetComponent<HumanGameManager>();
        sqlDB = new SqliteDatabase("replays.db");
        StartCoroutine(migrateAttempts());
        StartCoroutine(migrateReplays());
    }

    IEnumerator migrateAttempts() {

        string attemptQuery = "SELECT * FROM Attempt";
        DataTable attemptData = sqlDB.ExecuteQuery(attemptQuery);
        string attemptIDs = "", attemptLevels = "", attemptNumbers = "", startTimes = "", isEngineReplays = "";

        if (attemptData.Rows.Count == 0) {
            Debug.Log("Nothing to send! No attempts in cache");
            yield break;
        }


        // Creates new Attemps and puts them into the list.
        foreach (DataRow attempt_row in attemptData.Rows) {
            string attemptID = (string)attempt_row["AttemptID"];
            string attemptLevel = ((int)attempt_row["Level"]).ToString();
            string attemptNumber = ((int)attempt_row["AttemptNumber"]).ToString();
            string startTime = (string)attempt_row["StartTime"];
            string isEngineReplay = ((int)attempt_row["isEngineReplay"]).ToString();

            attemptIDs = String.Concat(attemptIDs, ",", attemptID);
            attemptLevels = String.Concat(attemptLevels, ",", attemptLevel);
            attemptNumbers = String.Concat(attemptNumbers, ",", attemptNumber);
            startTimes = String.Concat(startTimes, ",", startTime);
            isEngineReplays = String.Concat(isEngineReplays, ",", isEngineReplay);
            Debug.Log("Shits happenin at attempt...");
            yield return null;
        }
        attemptIDs = attemptIDs.Remove(0, 1);
        attemptLevels = attemptLevels.Remove(0, 1);
        attemptNumbers = attemptNumbers.Remove(0, 1);
        startTimes = startTimes.Remove(0, 1);
        isEngineReplays = isEngineReplays.Remove(0, 1);

        StartCoroutine(postAttempt(attemptIDs, attemptLevels, attemptNumbers, startTimes, isEngineReplays)); 
    }


    IEnumerator postAttempt(string AttemptIDs, string Levels, string AttemptNumbers, string StartTimes, string isEngineReplays) {
        yield return new WaitForEndOfFrame();
        Debug.Log("In post attempts");
        WWWForm form = new WWWForm();
        form.AddField("AttemptIDs", AttemptIDs);
        form.AddField("Levels", Levels);
        form.AddField("AttemptNumbers", AttemptNumbers);
        form.AddField("StartTimes", StartTimes);
        form.AddField("isEngineReplays", isEngineReplays);

        url = "http://localhost:8081/insertAttempt";
        WWW www = new WWW(url, form);
        yield return www;

        if (!string.IsNullOrEmpty(www.error)) {
            print(www.error);
        } else {
            print(www.text);
            if (www.text == "1") {
                StartCoroutine(deleteAttempts(AttemptIDs));
            } else if (www.text.Split('|')[1] == "ER_DUP_ENTRY"){
                Debug.Log("In here!");
                StartCoroutine(deleteAttempts(AttemptIDs));
            }else{
                Debug.Log("SOMETHING WENT WRONG PLEASE TRY AGAIN!");
            }
        }
    }

    IEnumerator deleteAttempts(string AttemptIDs) {
        string[] IDlist = AttemptIDs.Split(',');
        Debug.Log(IDlist.Length);
        int delete = 0;
        foreach (string id in IDlist) {
            delete++;
            string deleteQuery = String.Format("DELETE FROM attempt WHERE AttemptID == '{0}'", id);
            sqlDB.ExecuteNonQuery(deleteQuery);
            print("one attempt deleted");
            yield return null;
        }
        print(delete);
        Debug.Log("All attempts deleted.");
    }

    IEnumerator migrateReplays() {

        string replayQuery = "SELECT * FROM ReplayData";
        DataTable replayData = sqlDB.ExecuteQuery(replayQuery);
        string replayIDs = "", attemptIDs = "", actions = "", directionXs = "", directionYs = "", timeStamps = "", timeSpents = "";

        if (replayData.Rows.Count == 0) {
            Debug.Log("Nothing to send! No replays in cache");
            yield break;
        }


        // Creates new Attemps and puts them into the list.
        foreach (DataRow attempt_row in replayData.Rows) {
            string replayID = ((int)attempt_row["ReplayID"]).ToString();
            string attemptID = (string)attempt_row["AttemptID"];
            string action = (string)attempt_row["Action"];
            string directionX = ((int)attempt_row["DirectionX"]).ToString();
            string directionY = ((int)attempt_row["DirectionY"]).ToString();
            string timeStamp = (string)attempt_row["TimeStamp"];
            string timeSpent = ((int)attempt_row["TimeSpent"]).ToString();

            replayIDs = String.Concat(replayIDs, ",", replayID);
            attemptIDs = String.Concat(attemptIDs, ",", attemptID);
            actions = String.Concat(actions, ",", action);
            directionXs = String.Concat(directionXs, ",", directionX);
            directionYs = String.Concat(directionYs, ",", directionY);
            timeStamps = String.Concat(timeStamps, ",", timeStamp);
            timeSpents = String.Concat(timeSpents, ",", timeSpent);
            Debug.Log("Shits happenin at replay...");
            yield return null;
        }

        replayIDs = replayIDs.Remove(0, 1);
        attemptIDs = attemptIDs.Remove(0, 1);
        actions = actions.Remove(0, 1);
        directionXs = directionXs.Remove(0, 1);
        directionYs = directionYs.Remove(0, 1);
        timeStamps = timeStamps.Remove(0, 1);
        timeSpents = timeSpents.Remove(0, 1);
        Debug.Log(directionYs);

        StartCoroutine(postReplay(replayIDs, attemptIDs, actions, directionXs, directionYs, timeStamps, timeSpents));
    }

    IEnumerator postReplay(string ReplayIDs, string AttemptIDs, string Actions, string DirectionXs, string DirecionYs, string TimeStamps, string TimeSpents) {
        yield return new WaitForEndOfFrame();

        WWWForm form = new WWWForm();
        form.AddField("AttemptIDs", AttemptIDs);
        form.AddField("Actions", Actions);
        form.AddField("DirectionXs", DirectionXs);
        form.AddField("DirectionYs", DirecionYs);
        form.AddField("TimeStamps", TimeStamps);
        form.AddField("TimeSpents", TimeSpents);

        Debug.Log("In postReplay");
        url = "http://localhost:8081/insertReplay";
        WWW www = new WWW(url, form);
        yield return www;

        if (!string.IsNullOrEmpty(www.error)) {
            print(www.error);
        } else {
            print(www.text);
            if (www.text == "1") {
                Debug.Log("Success!");
                StartCoroutine(deleteReplays(ReplayIDs));
            } else if (www.text.Split('|')[1] == "ER_DUP_ENTRY") {
                Debug.Log("In er dup entry");
                StartCoroutine(deleteReplays(ReplayIDs));
            } else {
                Debug.Log("SOMETHING WENT WRONG PLEASE TRY AGAIN!");
            }
        }
    }

    IEnumerator deleteReplays(string ReplayIDs) {
        string[] IDlist = ReplayIDs.Split(',');
        Debug.Log(IDlist.Length);
        int delete = 0;
        foreach (string id in IDlist) {
            delete++;
            string deleteQuery = String.Format("DELETE FROM ReplayData WHERE ReplayID == '{0}'", id);
            sqlDB.ExecuteNonQuery(deleteQuery);
            print("one attempt deleted");
            yield return null;
        }
        print(delete);
        Debug.Log("All attempts deleted.");
    }
}
