using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine;
#endif
public enum GameCharacterTypes {
	MeleeMonster,
	RangedMonster,
	RangedMonsterAttack,
	OgreMonster,
	BlobMonster,
	MinitaurMonster,
	Portal,
	Trap,
	Potion,
	Treasure,
	Entrance,
	Exit,
	Hero,
	Javelin,
	BloodDecoration,
	NoOne
}

public enum SimCharacterActionTypes{
	Move,BlockedMove,Fight,RangedAttack,BlobMerge,TreasureCollect,TrapSpring,PortalActivate,JavelinThrow,JavelinPickup,ReachedExit,DrankPotion,TookTreasure,Die
}

public struct SimCharacterAction{
	public SimCharacterActionTypes ActionType{get;set;}
	public SimPoint FromPoint {get;set;}
	public SimPoint ToPoint {get;set;}

	public SimCharacterAction(SimCharacterActionTypes actionType, SimPoint fromPoint, SimPoint toPoint) : this(){
		ActionType = actionType;
		FromPoint = fromPoint;
		ToPoint = toPoint;
	}
}

public abstract class SimGameCharacter{

	public GameCharacterTypes CharacterType{get;set;}
	
	public bool Alive {get;set;}
	public SimPoint PriorPoint{get;set;}
	public SimPoint Point{get;set;}
	
	public int Health{get;set;}
	public int HealthGained{get; protected set;}
	public int HealthLost{get; protected set;}

	public int Damage{get;set;}

	public bool CacheActions{get; private set;}
	public List <SimCharacterAction> CachedActions{get;set;}
	public bool InPortal{get;set;}

	public abstract void TakeTurn(SimLevel simLevel);

	public abstract void Interact(SimLevel simLevel, SimGameCharacter other);

	public abstract SimGameCharacter Clone();

	public abstract override bool Equals(object obj);

	public abstract override int GetHashCode();

	public virtual void TakeDamage(SimLevel simLevel, int damage){
		Health -= damage;
		HealthLost += damage;

		if(CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.Fight, PriorPoint, Point));}

		if(Health <= 0){
			Die(simLevel);
			if(CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.Die, Point, Point));}
		}
	}

	public virtual void Move(SimLevel level, SimPoint direction){
		if(!level.MoveIsLegal(this, direction)){
			return;
		} else {
			SimPoint newPoint = Point + direction;
			bool blocked = false;
			foreach(SimGameCharacter character in level.Characters){
				if(character.Alive){
					if(character.Point.Equals(newPoint)){
						Interact(level, character);
						if(CharacterBlocking(character)){
							blocked = true;
							if(CacheActions){CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.BlockedMove, Point, newPoint));}
						}
					}
				}
			}

			if(!blocked && !InPortal){	
				PriorPoint = Point;
				Point = newPoint;
				if(CacheActions){
					//UnityEngine.Debug.Log("Caching a move");
					CachedActions.Add(new SimCharacterAction(SimCharacterActionTypes.Move, PriorPoint, Point));
				}
			} else if (InPortal){
				InPortal = false;
			}
		}

	}

	public static SimPoint MoveToward(SimPoint fro, SimPoint to){
		if(fro.X == to.X && fro.Y > to.Y){
			return SimPoint.North;
		}
		if(fro.X < to.X && fro.Y == to.Y){
			return SimPoint.East;
		}
		if(fro.X == to.X && fro.Y < to.Y){
			return SimPoint.South;
		}
		if(fro.X > to.X && fro.Y == to.Y){
			return SimPoint.West;
		}
		return SimPoint.Zero;
	}

	public virtual void Die(SimLevel simLevel){
		Alive = false;

	}

	public bool CharacterBlocking(SimGameCharacter other){
		if(!other.Alive){return false;}
		if(other.CharacterType == GameCharacterTypes.MinitaurMonster){
			var minitaur = (SimMinitaurMonster)other;
			if(minitaur.KnockoutCounter > 0){
				return false;
			}
		}

		bool blocking = false;
		blocking = blocking || (other.CharacterType == GameCharacterTypes.Hero);
		blocking = blocking || (other.CharacterType == GameCharacterTypes.BlobMonster);
		blocking = blocking || (other.CharacterType == GameCharacterTypes.MeleeMonster);
		blocking = blocking || (other.CharacterType == GameCharacterTypes.MinitaurMonster);
		blocking = blocking || (other.CharacterType == GameCharacterTypes.OgreMonster);
		blocking = blocking || (other.CharacterType == GameCharacterTypes.RangedMonster);
		return blocking;
	}

	public bool IsFightingGameCharacter(SimGameCharacter other){
		if(other.CharacterType == GameCharacterTypes.MinitaurMonster){
			var minitaur = (SimMinitaurMonster)other;
			if(minitaur.KnockoutCounter > 0){
				return false;
			}
		}

		bool result = false;
		
		result = result || other.CharacterType == GameCharacterTypes.BlobMonster;
		result = result || other.CharacterType == GameCharacterTypes.Hero;
		result = result || other.CharacterType == GameCharacterTypes.MeleeMonster;
		result = result || other.CharacterType == GameCharacterTypes.MinitaurMonster;
		result = result || other.CharacterType == GameCharacterTypes.OgreMonster;
		//result = result || other.CharacterType == GameCharacterTypes.RangedMonster;
		result = result || other.CharacterType == GameCharacterTypes.Trap;

		return result;
	}

	public void SetActionCaching(bool cacheActions){
		if(cacheActions){CachedActions = new List<SimCharacterAction>();}else{CachedActions = null;}
		CacheActions = cacheActions;
	}

	public List<SimCharacterAction> FlushCachedActions(){
		List<SimCharacterAction> result = new List<SimCharacterAction>();
		result.AddRange(CachedActions);
		CachedActions.Clear();
		return result;
	}
}
