#if UNITY_EDITOR
using UnityEngine;
#endif

public class SimOgreMonster : SimMonsterCharacter{
	
	public int TreasureCollected{get;set;}

	public SimOgreMonster(bool alive, SimPoint priorPoint, SimPoint point, int health, int damage){
		CharacterType = GameCharacterTypes.OgreMonster;

		Alive = alive;
		PriorPoint = priorPoint;
		Point = point;
		Health = health;
		Damage = damage;
	}

	public SimOgreMonster(SimOgreMonster other){
		CharacterType = other.CharacterType;

		Alive = other.Alive;
		PriorPoint = other.PriorPoint;
		Point = other.Point;
		Health = other.Health;
		HealthGained = other.HealthGained;
		HealthLost = other.HealthLost;
		Damage = other.Damage;

		TreasureCollected = other.TreasureCollected;
	}

	public override SimGameCharacter Clone(){
		var clone = new SimOgreMonster(this);
		return clone;
	}

	public override bool Equals(object obj){
		var other = (SimOgreMonster) obj;
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
		if(other.TreasureCollected != TreasureCollected){
			return false;
		}

		return true;
	}

	public override int GetHashCode(){
		int alive = Alive ? 1 : 0;
		return  alive + (int)CharacterType + Point.X ^ Point.Y + Damage * Health * TreasureCollected;
	}

	public override void Interact(SimLevel simLevel, SimGameCharacter other){

		if (other.CharacterType == GameCharacterTypes.Hero)
        {
			simLevel.LogMechanic(Mechanic.OgreHit);
        }
		if(other.CharacterType == GameCharacterTypes.Treasure){
			TreasureCollected++;
			other.Die(simLevel);
			simLevel.LogMechanic(Mechanic.OgreTreasure);
			if(CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.TookTreasure,Point,other.Point));}
		}

		if(IsFightingGameCharacter(other)){
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
		
		if(CacheActions && other.CharacterType == GameCharacterTypes.Trap){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.TrapSpring, Point, other.Point));}
		
		return;
	}

	public override void TakeTurn(SimLevel simLevel){
		if(!Alive){return;}
		
		bool treasureFound = false;
		int dTreasure = 999;
		int treasureIndex = -1;
		
		bool heroFound = false;
		int dHero = 999;

		for(int treasure = 0; treasure < simLevel.Treasures.Count; treasure++){
			if(simLevel.Treasures[treasure].Alive){
				if(simLevel.LineOfSight(Point, simLevel.Treasures[treasure].Point)){
					treasureFound = true;
					int d = simLevel.RayTrace(Point, simLevel.Treasures[treasure].Point).Length;
					if(d < dTreasure){
						treasureIndex = treasure;
						dTreasure = d;
					}
				}
			}
		}
		if(simLevel.LineOfSight(Point,simLevel.SimHero.Point)){
			heroFound = true;
			dHero = simLevel.RayTrace(Point, simLevel.SimHero.Point).Length;
		}

		if(!heroFound && !treasureFound){
			return;
		}
		
		SimPoint[] path = null;
		if(heroFound && !treasureFound){
			//path = simLevel.AStar(Point, simLevel.SimHero.Point);
			//path = simLevel.Lookup(Point, simLevel.SimHero.Point);
			path = PathDatabase.Lookup(Point, simLevel.SimHero.Point, simLevel.LevelString);
		}
		if(!heroFound && treasureFound){
			//Pathfind to potionidenx and move
			//path = simLevel.AStar(Point, simLevel.Treasures[treasureIndex].Point);
			//path = simLevel.Lookup(Point, simLevel.Treasures[treasureIndex].Point, simLevel.LevelString);

			path = PathDatabase.Lookup(Point, simLevel.Treasures[treasureIndex].Point, simLevel.LevelString);
		}
		if(heroFound && treasureFound){
			if(dHero <= dTreasure){
				//path = simLevel.AStar(Point, simLevel.SimHero.Point);
				//path = simLevel.Lookup(Point, simLevel.SimHero.Point);

				path = PathDatabase.Lookup(Point, simLevel.SimHero.Point, simLevel.LevelString);
			} else {
				//path = simLevel.AStar(Point, simLevel.Treasures[treasureIndex].Point);
//				path = simLevel.Lookup(Point, simLevel.Treasures[treasureIndex].Point);

				path = PathDatabase.Lookup(Point, simLevel.Treasures[treasureIndex].Point, simLevel.LevelString);
			}
		}
		if(path != null && path.Length > 1){
			Move(simLevel, MoveToward(Point, path[1]));
		}
		return;
	}

}