using System.Collections.Generic;

public static class ActionTraceUtils{
	
	public static List<SimHeroAction> ReadActionString(string actionString){
		var result = new List<SimHeroAction>();
		string[] actionStrings = actionString.Split('\n');
		foreach(string actionTuple in actionStrings){
			string[] parts = actionTuple.Split(',');
			if(parts[0].Equals("Move")){
				result.Add(new SimHeroAction(HeroActionTypes.Move, new SimPoint(int.Parse(parts[1]),int.Parse(parts[2]))));
			} else if(parts[0].Equals("JavelinThrow")){
				result.Add(new SimHeroAction(HeroActionTypes.JavelinThrow, new SimPoint(int.Parse(parts[1]),int.Parse(parts[2]))));
			}
		}
		return result;
	}

}