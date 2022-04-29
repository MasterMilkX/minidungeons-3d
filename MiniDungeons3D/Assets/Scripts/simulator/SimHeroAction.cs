public enum HeroActionTypes{
		None,
		Move,
		JavelinThrow
}

public struct SimHeroAction {

	public HeroActionTypes ActionType{get;set;}
	public SimPoint DirectionOrTarget{get;set;}

	public SimHeroAction(HeroActionTypes actionType, SimPoint directionOrTarget) : this(){
		ActionType = actionType;
		DirectionOrTarget = directionOrTarget;
	}

	public override string ToString(){
		return (ActionType + "," + DirectionOrTarget.X + "," + DirectionOrTarget.Y);
	}

	public override bool Equals (object obj)
	{
		SimHeroAction other = (SimHeroAction)obj;
		return ActionType.Equals (other.ActionType) && DirectionOrTarget.Equals (other.DirectionOrTarget);
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	public Mechanic GetMechanicVersion()
    {
		if (ActionType == HeroActionTypes.Move && DirectionOrTarget.X > 0 ){
			return Mechanic.MoveRight;
        }
		else if (ActionType == HeroActionTypes.Move && DirectionOrTarget.X < 0)
        {
			return Mechanic.MoveLeft;
        }
		else if (ActionType == HeroActionTypes.Move && DirectionOrTarget.Y > 0)
        {
			return Mechanic.MoveUp;
        }
		else if (ActionType == HeroActionTypes.Move && DirectionOrTarget.Y < 0)
        {
			return Mechanic.MoveDown;
        }
		else if (ActionType == HeroActionTypes.JavelinThrow)
        {
			return Mechanic.JavelinThrow;
        }
        else
        {
			return Mechanic.None;
        }
	}
}