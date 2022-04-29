using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UILevelReport : MonoBehaviour
{

    public Text _title;
    public Text _healthLeft;
    public Text _monstersKilled;
    public Text _minitaursKnockedOut;
    public Text _treasuresCollected;
    public Text _potionsDrunk;
    public Text _stepsTaken;

    // Replay Variables
    GameObject LevelView;
    ReplayPlayer _ReplayPlayer;
    HumanGameManager _HumanGameManager;

    public Text _totalTimeTaken;
    public Text _attemptNumber;
    public Text _attemptID;

    void Awake()
    {
        LevelView = GameObject.Find("LevelView");
        _HumanGameManager = LevelView.GetComponent<HumanGameManager>();
        _ReplayPlayer = LevelView.GetComponent<ReplayPlayer>();
    }


    public void UpdateTexts(SimLevel level, bool won)
    {
        string state = won ? "Won" : "Lost";
        _title.text = "Level " + state;
        _healthLeft.text = "Health: " + level.SimHero.Health;

        int kills = 0;
        int monsters = 0;
        int minitaurKnockouts = 0;
        int treasuresInLevel = 0;
        int potionsInLevel = 0;
        foreach (SimGameCharacter character in level.Characters)
        {
            if (character.CharacterType == GameCharacterTypes.BlobMonster || character.CharacterType == GameCharacterTypes.MeleeMonster || character.CharacterType == GameCharacterTypes.OgreMonster || character.CharacterType == GameCharacterTypes.RangedMonster)
            {
                monsters++;
                if (!character.Alive)
                {
                    kills++;
                }
            }
            if (character.CharacterType == GameCharacterTypes.MinitaurMonster)
            {
                var minitaur = (SimMinitaurMonster)character;
                minitaurKnockouts += minitaur.TimesKnockedOut;
            }
            if (character.CharacterType == GameCharacterTypes.Treasure)
            {
                treasuresInLevel++;
            }
            if (character.CharacterType == GameCharacterTypes.Potion)
            {
                potionsInLevel++;
            }
        }
        _monstersKilled.text = "Monsters: " + kills + "/" + monsters;
        _minitaursKnockedOut.text = "Minitaurs: " + minitaurKnockouts;
        _treasuresCollected.text = "Treasures: " + level.SimHero.TreasureCollected + "/" + treasuresInLevel;
        _potionsDrunk.text = "Potions: " + level.SimHero.PotionsCollected + "/" + potionsInLevel;
        _stepsTaken.text = "Steps: " + level.SimHero.StepsTaken;

        // Replay related functions

        if (_HumanGameManager.replayMode)
        {
            float totalTime = 0f;
            Attempt attempt = _ReplayPlayer.simulationAttempt;

            foreach (ReplayData replay in attempt.AttemptReplay) totalTime += replay.TimeSpent;

            _attemptNumber.text = "Attempt #: " + attempt.AttemptNumber.ToString();
            _totalTimeTaken.text = "TotalTime: " + totalTime.ToString() + "ms";
            _attemptID.text = "AttemptID: " + attempt.AttemptStart.ToString();
        }

        if (_HumanGameManager.testPlay)
        {
            Variables._healthLeft = "" + level.SimHero.Health;
            Variables._monstersKilled = kills + "/" + monsters;
            Variables._minitaursKnockedOut = "" + minitaurKnockouts;
            Variables._treasuresCollected = level.SimHero.TreasureCollected + "/" + treasuresInLevel;
            Variables._potionsDrunk = level.SimHero.PotionsCollected + "/" + potionsInLevel;
            Variables._stepsTaken = "" + level.SimHero.StepsTaken;
        }
    }
}
