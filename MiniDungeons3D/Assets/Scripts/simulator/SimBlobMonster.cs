#if UNITY_EDITOR
using UnityEngine;
#endif

public class SimBlobMonster : SimMonsterCharacter{
	
	public int DevelopmentStage{get;set;}
	public int PotionsCollected{get;set;}
	public int BlobMerges{get;set;}

	public SimBlobMonster(bool alive, SimPoint priorPoint, SimPoint point, int health, int damage, int developmentStage){
		CharacterType = GameCharacterTypes.BlobMonster;

		Alive = alive;
		PriorPoint = priorPoint;
		Point = point;
		Health = health;
		Damage = damage;

		DevelopmentStage = developmentStage;
	}

	public SimBlobMonster(SimBlobMonster other){
		CharacterType = other.CharacterType;

		Alive = other.Alive;
		PriorPoint = other.PriorPoint;
		Point = other.Point;
		Health = other.Health;
		HealthGained = other.HealthGained;
		HealthLost = other.HealthLost;
		Damage = other.Damage;

		DevelopmentStage = other.DevelopmentStage;
		PotionsCollected = other.PotionsCollected;
		BlobMerges = other.BlobMerges;
	}

	public override SimGameCharacter Clone(){
		var clone = new SimBlobMonster(this);
		return clone;
	}

	public override bool Equals(object obj){
		var other = (SimBlobMonster) obj;
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
		if(other.DevelopmentStage != DevelopmentStage){
			return false;
		}

		return true;
	}

	public override int GetHashCode(){
		int alive = Alive ? 1 : 0;
		return  alive + (int)CharacterType + Point.X ^ Point.Y + Damage * Health * DevelopmentStage;
	}

	public override void Interact(SimLevel simLevel, SimGameCharacter other){
		if (other.CharacterType == GameCharacterTypes.Hero)
		{
			simLevel.LogMechanic(Mechanic.BlobHit);
		}

		if (other.CharacterType == GameCharacterTypes.BlobMonster){
			var otherBlob = (SimBlobMonster)other;
			DevelopmentStage = DevelopmentStage + otherBlob.DevelopmentStage;
			if(DevelopmentStage > 3){DevelopmentStage = 3;}
			Damage = DevelopmentStage;
			Health = DevelopmentStage;
			otherBlob.Die(simLevel);
			BlobMerges++;
			simLevel.LogMechanic(Mechanic.BlobCombine);
			if(CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.BlobMerge,Point,other.Point));}
			//UnityEngine.Debug.Log("Blobmerge");
			return;
		}

		if(other.CharacterType == GameCharacterTypes.Potion){
			DevelopmentStage = DevelopmentStage + 1;
			if(DevelopmentStage > 3){DevelopmentStage = 3;}
			Damage = DevelopmentStage;
			Health = DevelopmentStage;
			other.Die(simLevel);
			PotionsCollected++;
			simLevel.LogMechanic(Mechanic.BlobPotion);
			if (CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.DrankPotion,Point,other.Point));}
			return;
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

	public override void TakeDamage(SimLevel simLevel, int damage){
		Health -= damage;
		DevelopmentStage = DevelopmentStage - damage;
		Damage = DevelopmentStage;
		if(Health <= 0){
			Die(simLevel);
		}
	}

	public override void TakeTurn(SimLevel simLevel){
		if(!Alive){
			return;
		}
		
		bool potionFound = false;
		int dPotion = 999;
		int potionIndex = -1;
		
		bool heroFound = false;
		int dHero = 999;

		for(int potion = 0; potion < simLevel.Potions.Count; potion++){
			if(simLevel.Potions[potion].Alive){
				if(simLevel.LineOfSight(Point, simLevel.Potions[potion].Point)){
					potionFound = true;
					int d = simLevel.RayTrace(Point, simLevel.Potions[potion].Point).Length;
					if(d < dPotion){
						potionIndex = potion;
						dPotion = d;
					}
				}
			}
		}
		if(simLevel.LineOfSight(Point,simLevel.SimHero.Point)){
			heroFound = true;
			dHero = simLevel.RayTrace(Point, simLevel.SimHero.Point).Length;
		}

		if(!heroFound && !potionFound){
			return;
		}

		SimPoint[] path = null;
		if(heroFound && !potionFound){
			path = PathDatabase.Lookup (Point, simLevel.SimHero.Point, simLevel.LevelString);
			//path = simLevel.AStar(Point, simLevel.SimHero.Point);
//			path = simLevel.Lookup(Point, simLevel.SimHero.Point);

		}
		if(!heroFound && potionFound){
			//Pathfind to potionidenx and move
			//path = simLevel.AStar(Point, simLevel.Potions[potionIndex].Point);
			//path = simLevel.Lookup(Point, simLevel.Potions[potionIndex].Point);

			path = PathDatabase.Lookup (Point, simLevel.Potions[potionIndex].Point, simLevel.LevelString);
		}
		if(heroFound && potionFound){
			if(dHero <= dPotion){
				//path = simLevel.AStar(Point, simLevel.SimHero.Point);
				//path = simLevel.Lookup(Point, simLevel.SimHero.Point);

				path = PathDatabase.Lookup(Point, simLevel.SimHero.Point, simLevel.LevelString);
			} else {
				//path = simLevel.AStar(Point, simLevel.Potions[potionIndex].Point);
				//path = simLevel.Lookup(Point, simLevel.Potions[potionIndex].Point);
				path = PathDatabase.Lookup(Point, simLevel.Potions[potionIndex].Point, simLevel.LevelString);
				
			}
		}
		if(path != null && path.Length > 1){
			Move(simLevel, MoveToward(Point, path[1]));
		}
		return;
	}
}