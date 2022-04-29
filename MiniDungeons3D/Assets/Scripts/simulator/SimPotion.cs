public class SimPotion : SimGameCharacter {
	public int Healing{get;set;}

	public SimPotion(bool alive, SimPoint priorPoint, SimPoint point, int health, int damage, int healing){
		CharacterType = GameCharacterTypes.Potion;

		Alive = alive;
		PriorPoint = priorPoint;
		Point = point;
		Health = health;
		Damage = damage;

		Healing = healing;
	}

	public SimPotion(SimPotion other){
		CharacterType = other.CharacterType;

		Alive = other.Alive;
		PriorPoint = other.PriorPoint;
		Point = other.Point;
		Health = other.Health;
		HealthGained = other.HealthGained;
		HealthLost = other.HealthLost;
		Damage = other.Damage;

		Healing = other.Healing;
	}

	public override SimGameCharacter Clone(){
		var clone = new SimPotion(this);
		return clone;
	}

	public override bool Equals(object obj){
		var other = (SimPotion) obj;
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
		if(other.Healing != Healing){
			return false;
		}

		return true;
	}

	public override int GetHashCode(){
		int alive = Alive ? 1 : 0;
		return  alive + (int)CharacterType + Point.X ^ Point.Y + Damage * Health * Healing;
	}

	public override void TakeTurn(SimLevel simLevel){
		return;
	}

	public override void Interact(SimLevel simLevel, SimGameCharacter other){return;}
}