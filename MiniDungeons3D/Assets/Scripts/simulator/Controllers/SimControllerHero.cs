using System;
using System.Collections.Generic;

public class MapReport
{

    public static readonly string MapReportHeader = "Controller_Type,Utility_Function,NativeUtility,Time_mS,utilE,utilR,utilS,utilMK,utilTC,Turns,HP_left,Damage_Taken,Deaths,Javelin_throws,Monsters_Killed,Minitaurs_Knocked,Treasures_Collected,Potions_Collected";

    //public string CSVFriendlyReport { get; set; }
    public LevelReport LevelReport { get; set; }
    public List<string> StateReport { get; set; }
    public SimPoint startLocation;
    public List<SimPoint> Positions { get; set; }
    public List<SimHeroAction> ActionReport { get; set; }
    public List<string> LevelStates { get; set; }
    public Dictionary<Mechanic, int> Frequencies { get; set; }
    //public string UtilityFunction { get; set; }
    public int Map = -1;
    public string Label;
    public int Run = -1;
    public List<Mech> Mechanics { get; set; }

    public Dictionary<string, int> Agreement { get; set; }

}

public class LevelReport
{
    public double utilityGained;
    public double exitUtility;
    public double runnerUtility;
    public double loitererUtility;
    public double completionistUtility;
    public double monsterKillerUtility;
    public double treasureCollectorUtility;
    public int turnsTaken;
    public int health;
    public int healthLost;
    public bool alive;
    public int javelinsThrown;
    public int monsterKills;
    public int minitaurKnockouts;
    public int treasuresCollected;
    public int potionsTaken;
    public int totalMonsters;
    public int totalTreasures;
    public int totalPotions;
    public float timeTaken;

    public int endDistToExit;
    public int startDistanceToExit;
    public int longestPath;
}

public class NextActionParameterObject
{

    public SimLevel _level;
    public SimHeroAction _nextAction;

    public NextActionParameterObject(SimLevel level, ref SimHeroAction nextAction)
    {
        _level = level;
        _nextAction = nextAction;
    }
}

public abstract class SimControllerHero
{
    public abstract MapReport PlayLevel(string levelString, UtilityFunctions utilityFunction, int msPerMove, int maxTurns);

    public abstract SimHeroAction NextAction(SimLevel level, int msPerMove);

    public static LevelReport LevelReport(string controllerType, Func<SimLevel, double> utilityFunction, double utilityGained, float timeTaken, int turnsTaken, SimLevel level)
    {
        int monsterKills = 0;
        int minitaurKnockOuts = 0;
        for (int i = 0; i < level.Characters.Length; i++)
        {
            if (level.Characters[i].CharacterType == GameCharacterTypes.MeleeMonster || level.Characters[i].CharacterType == GameCharacterTypes.RangedMonster || level.Characters[i].CharacterType == GameCharacterTypes.BlobMonster || level.Characters[i].CharacterType == GameCharacterTypes.OgreMonster)
            {
                if (!level.Characters[i].Alive)
                {
                    monsterKills++;
                }
            }
            if (level.Characters[i].CharacterType == GameCharacterTypes.MinitaurMonster)
            {
                SimMinitaurMonster minitaur = (SimMinitaurMonster)level.Characters[i];
                minitaurKnockOuts += minitaur.TimesKnockedOut;
            }
        }
        SimUtilityCalculator suc = new SimUtilityCalculator();
        LevelReport result = new LevelReport();
        result.utilityGained = utilityGained;
        result.timeTaken = timeTaken;
        result.exitUtility = suc.ExitUtility(level);
        result.runnerUtility = suc.RunnerUtility(level);
        result.loitererUtility = suc.LoitererUtility(level);
        result.completionistUtility = suc.CompletionistUtility(level);
        result.monsterKillerUtility = suc.MonsterKillerUtility(level);
        result.treasureCollectorUtility = suc.TreasureCollectorUtility(level);
        result.turnsTaken = turnsTaken;
        result.health = level.SimHero.Health;
        result.healthLost = level.SimHero.HealthLost;
        result.alive = level.SimHero.Health > 0 ? true : false;
        result.javelinsThrown = level.SimHero.JavelinsThrown;
        result.monsterKills = monsterKills;
        result.minitaurKnockouts = minitaurKnockOuts;
        result.treasuresCollected = level.SimHero.TreasureCollected;
        result.potionsTaken = level.SimHero.PotionsCollected;
        result.totalMonsters = level.Monsters.Count;
        result.totalPotions = level.Potions.Count;
        result.totalTreasures = level.Treasures.Count;
        result.endDistToExit = PathDatabase.Lookup(level.SimHero.Point, level.SimExit.Point, level).Length;
        result.longestPath = PathDatabase.GetLongestPath(level).Length;
        return result;
    }

    public static string StateReport(List<string> stateList)
    {
        string result = "";
        foreach (string state in stateList)
        {
            result += state;
            result += "\n";
        }
        return result;
    }

    public static string ActionReport(List<SimHeroAction> actionList)
    {
        string result = "";
        return result;
    }
}
