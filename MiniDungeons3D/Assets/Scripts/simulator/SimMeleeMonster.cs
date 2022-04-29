#if UNITY_EDITOR
using UnityEngine;
# endif
public class SimMeleeMonster : SimMonsterCharacter{
	
	public SimMeleeMonster(bool alive, SimPoint priorPoint, SimPoint point, int health, int damage){
		CharacterType = GameCharacterTypes.MeleeMonster;

		Alive = alive;
		PriorPoint = priorPoint;
		Point = point;
		Health = health;
		Damage = damage;
	}

	public SimMeleeMonster(SimMeleeMonster other){
		CharacterType = other.CharacterType;

		Alive = other.Alive;
		PriorPoint = other.PriorPoint;
		Point = other.Point;
		Health = other.Health;
		HealthGained = other.HealthGained;
		HealthLost = other.HealthLost;
		Damage = other.Damage;
	}

	public override SimGameCharacter Clone(){
		var clone = new SimMeleeMonster(this);
		return clone;
	}

	public override bool Equals(object obj){
		var other = (SimMeleeMonster) obj;
		if(other == null){return false;}
		if(other.Alive != Alive){
			return false;
		}
		if(other.CharacterType != CharacterType){
			return false;
		}
		if(!other.Point.Equals(Point)){
			return false;
		}
		if(other.Health != Health){
			return false;
		}
		if(other.Damage != Damage){
			return false;
		}

		return true;
	}

	public override int GetHashCode(){
		int alive = Alive ? 1 : 0;
		return  alive + (int)CharacterType + Point.X ^ Point.Y + Damage * Health;
	}

	public override void TakeTurn(SimLevel simLevel){
		if(!Alive){return;}

		if(simLevel.LineOfSight(Point, simLevel.SimHero.Point)){
//			SimPoint[] path = simLevel.AStar(Point, simLevel.SimHero.Point);
//			SimPoint[] path = simLevel.Lookup(Point, simLevel.SimHero.Point);
			if (Point == simLevel.SimHero.Point) {
				return;
			}
			SimPoint[] path = PathDatabase.Lookup(Point, simLevel.SimHero.Point, simLevel.LevelString);
			if(path.Length > 1){
				Move(simLevel,MoveToward(Point, path[1]));
			}
		}
		return;
	}

	public override void Interact(SimLevel simLevel, SimGameCharacter other){
		if (other.CharacterType == GameCharacterTypes.Hero)
		{
			simLevel.LogMechanic(Mechanic.GoblinMeleeHit);
		}
		if (IsFightingGameCharacter(other)){
			other.TakeDamage(simLevel, Damage);
			TakeDamage(simLevel, other.Damage);
			if(CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.Fight,Point,other.Point));}

		}
		if(other.CharacterType == GameCharacterTypes.RangedMonster){
			other.TakeDamage(simLevel, Damage);
			if(CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.Fight, Point, other.Point));}
		}
#if UNITY_EDITOR
		if(other.CharacterType == GameCharacterTypes.Hero){
			//camera shake
			GameObject camera = GameObject.Find ("Main Camera");
            camera.GetComponent<ShakeCamera>().MinorShake(.03f);
        }
#endif
		if (CacheActions && other.CharacterType == GameCharacterTypes.Trap){
			CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.TrapSpring, Point, other.Point));
		}

		return;
	}
}