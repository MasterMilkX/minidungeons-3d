public class SimPortal : SimGameCharacter{
	public SimPoint OtherPortalPoint{get;set;}

	public SimPortal(bool alive, SimPoint priorPoint, SimPoint point, int health, int damage){
		CharacterType = GameCharacterTypes.Portal;

		Alive = alive;
		PriorPoint = priorPoint;
		Point = point;
		Health = health;
		Damage = damage;
	}

	public SimPortal(bool alive, SimPoint priorPoint, SimPoint point, int health, int damage, SimPoint otherPortalPoint){
		CharacterType = GameCharacterTypes.Portal;

		Alive = alive;
		PriorPoint = priorPoint;
		Point = point;
		Health = health;
		Damage = damage;

		OtherPortalPoint = otherPortalPoint;
	}

	public SimPortal(SimPortal other){
		CharacterType = other.CharacterType;

		Alive = other.Alive;
		PriorPoint = other.PriorPoint;
		Point = other.Point;
		Health = other.Health;
		HealthGained = other.HealthGained;
		HealthLost = other.HealthLost;
		Damage = other.Damage;

		OtherPortalPoint = other.OtherPortalPoint;
	}

	public override SimGameCharacter Clone(){
		var clone = new SimPortal(this);
		return clone;
	}

	public override bool Equals(object obj){
		var other = (SimPortal) obj;
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
		if(other.OtherPortalPoint != OtherPortalPoint){
			return false;
		}

		return true;
	}

	public override int GetHashCode(){
		int alive = Alive ? 1 : 0;
		return  alive + (int)CharacterType + Point.X ^ Point.Y + Damage * Health * (OtherPortalPoint.X + OtherPortalPoint.Y);
	}

	public override void TakeTurn(SimLevel simLevel){
		return;
	}

	public override void Interact(SimLevel simLevel, SimGameCharacter other){return;}
}