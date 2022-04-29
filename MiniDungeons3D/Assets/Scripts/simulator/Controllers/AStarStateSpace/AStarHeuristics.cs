public static class AStarHeuristics{
	
	public static double ExitHeuristic(SimLevel level){
		int distance = level.AStar(level.SimHero.Point, level.SimExit.Point).Length-1;
		return (double)distance;
	}

	public static double RunnerHeuristic(SimLevel level){
		int distance = level.AStar(level.SimHero.Point, level.SimExit.Point).Length-1;
		return (double)distance;
	}

	public static double SurvivalistHeuristic(SimLevel level){
		int potionsAlive = 0;
		int potionsDead = 0;
		int nearestPotionIndex = -1;
		int nearestPotionDistance = int.MaxValue;
		for(int i = 0; i < level.Characters.Length; i++){
			if(level.Characters[i].CharacterType == GameCharacterTypes.Potion){
				if(level.Characters[i].Alive){
					potionsAlive++;
					int dist = level.AStar(level.SimHero.Point, level.Characters[i].Point).Length;
					if(dist < nearestPotionDistance){
						nearestPotionDistance = dist;
						nearestPotionIndex = i;
					}
				} else {
					potionsDead++;
				}
			}
		}
		double potionH = nearestPotionIndex >= 0 ? (double)nearestPotionDistance : 0d;
		
		return potionH * 0.5d + (double)level.AStar(level.SimHero.Point, level.SimExit.Point).Length * 0.5d;
	}

	public static double CompletionistHeuristic(SimLevel level)
	{
		int nearestMonsterIndex = -1;
		int nearestMonsterDistance = int.MaxValue;
		int nearestTreasureIndex = -1;
		int nearestTreasureDistance = int.MaxValue;
		for (int i = 0; i < level.Characters.Length; i++)
		{
			if (level.Characters[i].CharacterType == GameCharacterTypes.MeleeMonster || level.Characters[i].CharacterType == GameCharacterTypes.RangedMonster || level.Characters[i].CharacterType == GameCharacterTypes.OgreMonster || level.Characters[i].CharacterType == GameCharacterTypes.BlobMonster)
			{
				if (level.Characters[i].Alive)
				{
					int dist = level.AStar(level.SimHero.Point, level.Characters[i].Point).Length;
					if (dist < nearestMonsterDistance)
					{
						nearestMonsterDistance = dist;
						nearestMonsterIndex = i;
					}
				}
			}
			else if (level.Characters[i].CharacterType == GameCharacterTypes.Treasure)
			{
				if (level.Characters[i].Alive)
				{
					int dist = level.AStar(level.SimHero.Point, level.Characters[i].Point).Length;
					if (dist < nearestTreasureDistance)
					{
						nearestTreasureDistance = dist;
						nearestTreasureIndex = i;
					}

				}
			}
		}
		double monsterH = nearestMonsterIndex >= 0 ? (double)nearestMonsterDistance : 0d;
		double treasureH = nearestTreasureIndex >= 0 ? (double)nearestTreasureDistance : 0d;
		//if (nearestTreasureIndex >= 0 || nearestMonsterIndex >= 0)
		//{
		//	return 0.5 * (double)nearestTreasureDistance + 0.5 * (double) nearestMonsterDistance;

		//}
		//else
		//{
		//	return (double)level.AStar(level.SimHero.Point, level.SimExit.Point).Length;

		//}
		return 0.4 * (double)nearestTreasureDistance + 0.4 * (double)nearestMonsterDistance * 0.1 * (double)level.AStar(level.SimHero.Point, level.SimExit.Point).Length; ;
	}

	public static double MonsterKillerHeuristic(SimLevel level){

		int monstersAlive = 0;
		int monstersDead = 0;
		int nearestMonsterIndex = -1;
		int nearestMonsterDistance = int.MaxValue;
		int nearestPotionIndex = -1;
		int nearestPotionDistance = int.MaxValue;

		for(int i = 0; i < level.Characters.Length; i++){
			if(level.Characters[i].CharacterType == GameCharacterTypes.MeleeMonster || level.Characters[i].CharacterType == GameCharacterTypes.RangedMonster || level.Characters[i].CharacterType == GameCharacterTypes.OgreMonster || level.Characters[i].CharacterType == GameCharacterTypes.BlobMonster){
				if(level.Characters[i].Alive){

					try
					{
						int dist = level.AStar(level.SimHero.Point, level.Characters[i].Point).Length;
						if (dist < nearestMonsterDistance)
						{
							nearestMonsterDistance = dist;
							nearestMonsterIndex = i;
						}
					}
					catch
                    {
						// do nothing
                    }
				}
			}
			/*else if (level.Characters[i].CharacterType == GameCharacterTypes.Potion)
            {
				if (level.Characters[i].Alive)
                {
					int dist = level.AStar(level.SimHero.Point, level.Characters[i].Point).Length;
					if (dist < nearestPotionDistance)
                    {
						nearestPotionDistance = dist;
						nearestPotionIndex = i;
                    }
                }
            }*/
		}
        //double monsterH = nearestMonsterIndex >= 0 ? (double) nearestMonsterDistance : 0d;
        if (nearestMonsterIndex >= 0)
        {

            return (double)nearestMonsterDistance;

        }
        else
        {
            return (double)level.AStar(level.SimHero.Point, level.SimExit.Point).Length;

        }
        //return monsterH * 0.5d + (double)level.AStar(level.SimHero.Point, level.SimExit.Point).Length * 0.5d;

        //return monsterH;
    }

	public static double TreasureHeuristic(SimLevel level){
		int treasuresAlive = 0;
		int treasuresDead = 0;
		int nearestTreasureIndex = -1;
		int nearestTreasureDistance = int.MaxValue;
		for(int i = 0; i < level.Characters.Length; i++){
			if(level.Characters[i].CharacterType == GameCharacterTypes.Treasure){
				if(level.Characters[i].Alive){
					treasuresAlive++;
					try
					{
						int dist = level.AStar(level.SimHero.Point, level.Characters[i].Point).Length;
						if (dist < nearestTreasureDistance)
						{
							nearestTreasureDistance = dist;
							nearestTreasureIndex = i;
						}
					}
					catch
                    {
						// do nothing
                    }
					
				} else {
					treasuresDead++;
				}
			}
		}
        //double treasureH = nearestTreasureIndex >= 0 ? (double)nearestTreasureDistance : 0d;
        if (nearestTreasureIndex >= 0)
        {
            return (double)nearestTreasureDistance;

        }
        else
        {
            return (double)level.AStar(level.SimHero.Point, level.SimExit.Point).Length;

        }
        //return treasureH * 0.5d + (double)level.AStar(level.SimHero.Point, level.SimExit.Point).Length * 0.5d;
    }
}