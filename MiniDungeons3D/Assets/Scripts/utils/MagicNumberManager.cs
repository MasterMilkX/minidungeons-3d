using System;
using System.Collections;

public class MagicNumberValues : ICloneable
{
    public int MeleeMonsterHealth = 1;
    public int MeleeMonsterDamage = 1;
    public int MeleeMonsterVision = 5;

    public int RangedMonsterHealth = 1;
    public int RangedMonsterDamage = 1;
    public int RangedMonsterAttackRange = 5;
    public int RangedMonsterVision = 10;

    public float RangedAttackDuration = 0.3f;
    public float RangedAttackSpeed = 8.0f;

    public int OgreMonsterHealth = 2;
    public int OgreMonsterDamage = 2;
    public int OgreMonsterVision = 5;

    public int BlobMonsterHealth = 1;
    public int BlobMonsterDamage = 1;
    public int BlobMonsterVision = 5;
    public int BlobMonsterMaxHealth = 3;
    public int BlobMonsterMaxDamage = 3;
    public int BlobMonsterMinStage = 1;
    public int BlobMonsterMaxStage = 3;

    public int MinitaurMonsterHealth = 1;
    public int MinitaurMonsterDamage = 1;
    public int MinitaurMonsterKnockoutTurns = 5;

    public int HeroMaxHealth = 10;
    public int HeroHealth = 10;
    public int HeroDamage = 1;
    public int heroThrowRange = 10;

    public int PotionHealing = 2;
    public int PotionHealth = 1;

    public int RewardValue = 1;
    public int RewardHealth = 1;

    public int TeleportHealth = 1;

    public int TrapDamage = 1;
    public int TrapHealth = 1;

    public float SecondsDelayAfterDeath = 2f;

    public int SimulationMaxTurns = 100;

    public object Clone()
    {
        return new MagicNumberValues();
    }
}

public class MagicNumberManager
{//: MonoBehaviour {

    private MagicNumberValues values;

    private static MagicNumberManager _instance;

    //public GameManager gameManager;

    public MagicNumberManager()
    {
        values = new MagicNumberValues();
    }

    public static MagicNumberManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MagicNumberManager();
            }
            return _instance;
        }
    }

    //Everyone needs values to rely on.
    public MagicNumberValues GetValues()
    {
        return values;
    }
}
