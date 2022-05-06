#if UNITY_EDITOR
using UnityEngine;
#endif

public class SimMinitaurMonster : SimMonsterCharacter
{
    public int KnockoutCounter { get; set; }
    public int TimesKnockedOut { get; set; }

    public SimMinitaurMonster(bool alive, SimPoint priorPoint, SimPoint point, int health, int damage, int knockoutCounter, int timesKnockedOut)
    {
        CharacterType = GameCharacterTypes.MinitaurMonster;

        Alive = alive;
        PriorPoint = priorPoint;
        Point = point;
        Health = health;
        Damage = damage;

        KnockoutCounter = knockoutCounter;
        TimesKnockedOut = timesKnockedOut;
    }

    public SimMinitaurMonster(SimMinitaurMonster other)
    {
        CharacterType = other.CharacterType;

        Alive = other.Alive;
        PriorPoint = other.PriorPoint;
        Point = other.Point;
        Health = other.Health;
        HealthGained = other.HealthGained;
        HealthLost = other.HealthLost;
        Damage = other.Damage;

        KnockoutCounter = other.KnockoutCounter;
        TimesKnockedOut = other.TimesKnockedOut;
    }

    public override SimGameCharacter Clone()
    {
        var clone = new SimMinitaurMonster(this);
        return clone;
    }

    public override bool Equals(object obj)
    {
        var other = (SimMinitaurMonster)obj;
        if (other == null) { return false; }
        if (other.Alive != Alive)
        {
            return false;
        }
        if (other.CharacterType != CharacterType)
        {
            return false;
        }
        if (!other.Point.Equals(Point))
        {
            return false;
        }
        if (other.Health != Health)
        {
            return false;
        }
        if (other.Damage != Damage)
        {
            return false;
        }
        if (other.KnockoutCounter != KnockoutCounter)
        {
            return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        int alive = Alive ? 1 : 0;
        return alive + (int)CharacterType + Point.X ^ Point.Y + Damage * Health * KnockoutCounter;
    }

    public override void TakeTurn(SimLevel simLevel)
    {
        if (KnockoutCounter > 0)
        {
            KnockoutCounter--;
            return;
        }

        //SimPoint[] path = simLevel.AStar(Point, simLevel.SimHero.Point);
        //		SimPoint[] path = simLevel.Lookup(Point, simLevel.SimHero.Point);

        // stops the minitaur from trying to map a point to its own point.  This sometimes occurs...
        if (Point == simLevel.SimHero.Point)
        {
            return;
        }
        try { 
            SimPoint[] path = PathDatabase.Lookup(Point, simLevel.SimHero.Point, simLevel.LevelString);
            if (path != null && path.Length > 1)
            {
                SimPoint direction = MoveToward(Point, path[1]);
                Move(simLevel, direction);
            }
        } catch
        {
            // if path wasnt in db do nothing
            Move(simLevel, this.Point);
        }

        return;
    }

    public override void TakeDamage(SimLevel simLevel, int damage)
    {
        if (KnockoutCounter == 0)
        {
            KnockoutCounter = 5;
            TimesKnockedOut++;
        }
    }

    public override void Interact(SimLevel simLevel, SimGameCharacter other)
    {
        if (other.CharacterType == GameCharacterTypes.Hero)
        {
            simLevel.LogMechanic(Mechanic.MinitaurHit);
        }
        if (IsFightingGameCharacter(other))
        {
            other.TakeDamage(simLevel, Damage);
            TakeDamage(simLevel, other.Damage);
            if (CacheActions) { CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.Fight, Point, other.Point)); }
        }
        if (other.CharacterType == GameCharacterTypes.RangedMonster)
        {
            other.TakeDamage(simLevel, Damage);
            if (CacheActions) { CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.Fight, Point, other.Point)); }
        }
#if UNITY_EDITOR
		if(other.CharacterType == GameCharacterTypes.Hero){
			//camera shake
			/*GameObject camera = GameObject.Find ("MinimapCamera");
            camera.GetComponent<ShakeCamera>().MinorShake(.03f);*/
        }
#endif
        if (CacheActions && other.CharacterType == GameCharacterTypes.Trap) { CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.TrapSpring, Point, other.Point)); }
        return;
    }
}