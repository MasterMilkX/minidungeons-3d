public class SimTrap : SimGameCharacter{
	public SimTrap(bool alive, SimPoint priorPoint, SimPoint point, int health, int damage){
		CharacterType = GameCharacterTypes.Trap;

		Alive = alive;
		PriorPoint = priorPoint;
		Point = point;
		Health = health;
		Damage = damage;
	}

	public SimTrap(SimTrap other){
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
		var clone = new SimTrap(this);
		return clone;
	}

	public override bool Equals(object obj){
		var other = (SimTrap) obj;
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
		return;
	}

	public override void Interact(SimLevel simLevel, SimGameCharacter other){return;}

	public override void TakeDamage(SimLevel simLevel, int damage){
	}
}