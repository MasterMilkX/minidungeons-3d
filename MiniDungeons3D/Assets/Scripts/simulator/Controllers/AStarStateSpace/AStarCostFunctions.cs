public static class AStarCostFunctions{

	public static double ExitCost(SimLevel level){
		int deadCost = level.SimHero.Alive ? 0 : 100;
		return (double)(level.SimHero.StepsTaken + level.SimHero.HealthLost + deadCost);
	}

	public static double RunnerCost(SimLevel level){
		return (double)(level.SimHero.StepsTaken * 1);
	}

	public static double SurvivalistCost(SimLevel level){
		return (double)level.SimHero.HealthLost + (double)level.SimHero.StepsTaken;
	}


	public static double CompletionistCost(SimLevel level)
    {
		int monstersDead = 0;
		for (int i = 0; i < level.Monsters.Count; i++)
		{
			if (!level.Monsters[i].Alive)
			{
				monstersDead++;
			}
		}
		double deadCost = level.SimHero.Alive ? 0d : double.MaxValue;
		double monsterC = 45 * (level.Monsters.Count - monstersDead);
		double treasureC = 45 * (level.Treasures.Count - level.SimHero.TreasureCollected);

		return 0.5 * monsterC + 0.5 * treasureC + deadCost;
	}

	public static double MonsterCost(SimLevel level){
		int monstersDead = 0;
		for(int i = 0; i < level.Monsters.Count; i++){
			if(!level.Monsters[i].Alive){
				monstersDead++;
			}
		}
		double deadCost = level.SimHero.Alive ? 0d : double.MaxValue;
		return 45 * (level.Monsters.Count - monstersDead) + deadCost; //+ (float)level.SimHero.StepsTaken;
	}

	public static double TreasureCost(SimLevel level){
		//return (level.Treasures.Count - level.SimHero.TreasureCollected); //+ (double)level.SimHero.StepsTaken;
		double deadCost = level.SimHero.Alive ? 0d : double.MaxValue;

		return 45 * (level.Treasures.Count - level.SimHero.TreasureCollected) + deadCost;
	}
}