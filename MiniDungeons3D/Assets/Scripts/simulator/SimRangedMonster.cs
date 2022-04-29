#if UNITY_EDITOR
using UnityEngine;
#endif
public class SimRangedMonster : SimMonsterCharacter{
	
	public int Range{get;set;}

	public SimRangedMonster(bool alive, SimPoint priorPoint, SimPoint point, int health, int damage, int range){
		CharacterType = GameCharacterTypes.RangedMonster;

		Alive = alive;
		PriorPoint = priorPoint;
		Point = point;
		Health = health;
		Damage = damage;

		Range = range;
	}

	public SimRangedMonster(SimRangedMonster other){
		CharacterType = other.CharacterType;

		Alive = other.Alive;
		PriorPoint = other.PriorPoint;
		Point = other.Point;
		Health = other.Health;
		HealthGained = other.HealthGained;
		HealthLost = other.HealthLost;
		Damage = other.Damage;

		Range = other.Range;
	}

	public override SimGameCharacter Clone(){
		var clone = new SimRangedMonster(this);
		return clone;
	}

	public override bool Equals(object obj){
		var other = (SimRangedMonster) obj;
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
		if(other.Range != Range){
			return false;
		}

		return true;
	}

	public override int GetHashCode(){
		int alive = Alive ? 1 : 0;
		return  alive + (int)CharacterType + Point.X ^ Point.Y + Damage * Health * Range;
	}

	public override void Interact(SimLevel simLevel, SimGameCharacter other){
#if UNITY_EDITOR
		if(other.CharacterType == GameCharacterTypes.Hero){
			//camera shake
			GameObject camera = GameObject.Find ("Main Camera");
            camera.GetComponent<ShakeCamera>().MinorShake(.03f);
        }
#endif
		if(CacheActions && other.CharacterType == GameCharacterTypes.Trap){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.TrapSpring, Point, other.Point));}
		return;
	}

	public override void TakeTurn(SimLevel simLevel){
		if(!Alive){return;}

		if(simLevel.LineOfSight(Point, simLevel.SimHero.Point)){
			int dHero = simLevel.RayTrace(Point, simLevel.SimHero.Point).Length;
			if(dHero < Range){
				simLevel.SimHero.TakeDamage(simLevel, Damage);
				simLevel.LogMechanic(Mechanic.GoblinRangedHit);

				//camera shake
#if UNITY_EDITOR
				GameObject camera = GameObject.Find ("Main Camera");
				camera.GetComponent<ShakeCamera> ().MinorShake (.03f);
#endif
				if(CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.RangedAttack,Point,simLevel.SimHero.Point));}
			} else {
				
				//SimPoint[] path = simLevel.AStar(Point, simLevel.SimHero.Point);
//				SimPoint[] path = simLevel.Lookup(Point, simLevel.SimHero.Point);

				SimPoint[] path = PathDatabase.Lookup(Point, simLevel.SimHero.Point, simLevel.LevelString);
				Move(simLevel, MoveToward(Point, path[1]));
			}
		}

		return;
	}
}