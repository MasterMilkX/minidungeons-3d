using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine;
#endif
public class SimHero : SimGameCharacter{
	
	public SimHeroAction NextHeroAction{get;set;}
	public bool NextActionReady{get;set;}
	public int Javelins{get;set;}

	public int StepsTaken{get;set;}
	public int JavelinsThrown{get;set;}
	public int TreasureCollected{get;set;}
	public int PotionsCollected{get;set;}
	//ShakeCamera shake;
	
	public SimHero(bool alive, SimPoint priorPoint, SimPoint point, int health, int healthGained, int healthLost, int damage, int javelins, int stepsTaken, int javelinsThrown, int treasureCollected, int potionsCollected){
		CharacterType = GameCharacterTypes.Hero;

		Alive = alive;
		PriorPoint = priorPoint;
		Point = point;
		Health = health;
		HealthGained = healthGained;
		HealthLost = healthLost;

		Damage = damage;

		Javelins = javelins;
		StepsTaken = stepsTaken;
		JavelinsThrown = javelinsThrown;
		TreasureCollected = treasureCollected;
		PotionsCollected = potionsCollected;
	}

	public SimHero(SimHero other){
		CharacterType = other.CharacterType;

		Alive = other.Alive;
		PriorPoint = other.PriorPoint;
		Point = other.Point;
		Health = other.Health;
		HealthGained = other.HealthGained;
		HealthLost = other.HealthLost;
		Damage = other.Damage;

		NextHeroAction = other.NextHeroAction;
		NextActionReady = other.NextActionReady;
		Javelins = other.Javelins;
		StepsTaken = other.StepsTaken;
		JavelinsThrown = other.JavelinsThrown;
		TreasureCollected = other.TreasureCollected;
		PotionsCollected = other.PotionsCollected;
	}

	public override SimGameCharacter Clone(){
		var clone = new SimHero(this);
		return clone;
	}

	public override bool Equals(object obj){
		var other = (SimHero) obj;
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
		if(other.Javelins != Javelins){
			return false;
		}

		return true;
	}

	public override int GetHashCode(){
		int alive = Alive ? 1 : 0;
		return  alive + (int)CharacterType + Point.X ^ Point.Y + Damage * Health * Javelins;
	}

	public void SetNextAction(SimHeroAction action){
		NextHeroAction = action;
		NextActionReady = true;
	}

	public override void TakeTurn(SimLevel simLevel){
		if(!NextActionReady){throw new Exception("SOMETHING IS WRONG");}

		if(NextHeroAction.ActionType == HeroActionTypes.JavelinThrow){
			ThrowJavelin(simLevel, NextHeroAction);
		}

		if(NextHeroAction.ActionType == HeroActionTypes.Move){
			Move(simLevel, NextHeroAction.DirectionOrTarget);
		}

		NextActionReady = false;
		StepsTaken++;
		return;
	}

	public void ThrowJavelin(SimLevel level, SimHeroAction heroAction){
		foreach(SimGameCharacter monster in level.Monsters){
			if(monster.Point.Equals(heroAction.DirectionOrTarget)){
				if(level.LineOfSight(level.SimHero.Point, heroAction.DirectionOrTarget) && level.SimHero.Javelins > 0){
					var javelin = new SimJavelin(true, SimPoint.Zero, NextHeroAction.DirectionOrTarget, 999, 1);
					monster.TakeDamage(level, javelin.Damage);
					level.Javelins.Add(javelin);
					level.SimHero.Javelins--;
					level.SimHero.JavelinsThrown++;
					level.LogMechanic(Mechanic.JavelinThrow);
					if(CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.JavelinThrow,Point,heroAction.DirectionOrTarget));}
				}
			}
		}
	}

	public override void Move(SimLevel level, SimPoint direction){
		base.Move(level, direction);
		var pickups = new List<SimJavelin>();
		foreach(SimJavelin javelin in level.Javelins){
			if(javelin.Point.Equals(Point)){
				pickups.Add(javelin);
				Javelins++;
				if(CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.JavelinPickup, PriorPoint, Point));}
			}
		}
		foreach(SimJavelin removeMe in pickups){
			level.Javelins.Remove(removeMe);
		}
	}

	public override void Interact(SimLevel simLevel, SimGameCharacter other){
		
		if(!Alive || !other.Alive){return;}
		this.LogInteraction(simLevel, other);

		if (IsFightingGameCharacter(other)){
			other.TakeDamage(simLevel, Damage);
			TakeDamage(simLevel, other.Damage);
			//camera shake
#if UNITY_EDITOR
			GameObject camera = GameObject.Find ("Main Camera");
            camera.GetComponent<ShakeCamera>().MinorShake(.03f);
#endif
          
            if (CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.Fight, Point, other.Point));}

		}
		if(other.CharacterType == GameCharacterTypes.RangedMonster){
			other.TakeDamage(simLevel, Damage);
			//TakeDamage(simLevel, other.Damage);
			if(CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.Fight, Point, other.Point));}	
		}
		if(other.CharacterType == GameCharacterTypes.Javelin){
			var javelin = (SimJavelin)other;
			simLevel.LogMechanic(Mechanic.JavelinPickup);
			simLevel.Javelins.Remove(javelin);
			Javelins++;
			if(CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.JavelinPickup, Point, other.Point));}
		}
		if(other.CharacterType == GameCharacterTypes.Exit){
			simLevel.LogMechanic(Mechanic.ReachStairs);
			simLevel.SimLevelState = SimLevelState.Won;
			if(CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.ReachedExit, Point, other.Point));}
		}
		if(other.CharacterType == GameCharacterTypes.Potion){
			var potion = (SimPotion)other;
			Health += potion.Healing;
			HealthGained++;
			PotionsCollected++;
			potion.Die(simLevel);
			simLevel.LogMechanic(Mechanic.CollectPotion);
			if (CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.DrankPotion, Point, other.Point));}
		}
		if(other.CharacterType == GameCharacterTypes.Treasure){
			other.Die(simLevel);
			TreasureCollected++;
			simLevel.LogMechanic(Mechanic.CollectTreasure);
			if (CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.TookTreasure, Point, other.Point));}
		}
		if(other.CharacterType == GameCharacterTypes.Portal){
			var portal = (SimPortal)other;
			PriorPoint = Point;
			Point = portal.OtherPortalPoint;
			InPortal = true;
			simLevel.LogMechanic(Mechanic.UsePortal);
			if (CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.PortalActivate, portal.Point, portal.OtherPortalPoint));}
			//if(CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.Move, Point, other.Point));}
		}
		if(CacheActions && other.CharacterType == GameCharacterTypes.Trap){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.TrapSpring, Point, other.Point));}

		return;
	}

	public override void Die(SimLevel simLevel){
		//camera shake
#if UNITY_ENGINE
		Debug.Log("Dead");
		GameObject camera = GameObject.Find ("Main Camera");
        camera.GetComponent<ShakeCamera>().LongShake(.05f);
#endif
		simLevel.LogMechanic(Mechanic.Die);
		Alive = false;
		simLevel.SimLevelState = SimLevelState.Lost;
	}

	public void LogInteraction(SimLevel level, SimGameCharacter other)
    {
		if (other.CharacterType == GameCharacterTypes.BlobMonster)
		{
			level.LogMechanic(Mechanic.BlobHit);
        }
		else if (other.CharacterType == GameCharacterTypes.MeleeMonster)
        {
			level.LogMechanic(Mechanic.GoblinMeleeHit);
        }
		else if (other.CharacterType == GameCharacterTypes.RangedMonster)
        {
			level.LogMechanic(Mechanic.GoblinRangedHit);
        }
		else if (other.CharacterType == GameCharacterTypes.OgreMonster)
        {
			level.LogMechanic(Mechanic.OgreHit);
        }
		else if (other.CharacterType == GameCharacterTypes.MinitaurMonster)
        {
			level.LogMechanic(Mechanic.MinitaurHit);
        }
		else if (other.CharacterType == GameCharacterTypes.Trap)
        {
			level.LogMechanic(Mechanic.TriggerTrap);
        }
    }
		
}