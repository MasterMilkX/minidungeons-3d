using System;
using System.Collections.Generic;
using System.Linq;

public enum SimLevelState{
	Playing,
	Lost,
	Won,
	Timeout
}

public class SimLevel {

	private SimPoint[] _directions = {SimPoint.North,SimPoint.East,SimPoint.South,SimPoint.West};

	public SimLevelState SimLevelState{get;set;}


	public Logger Logger { get; set; }
    // incremenet the values in LevelEditor.
    public int startingExitCount = 0, startingEnteranceCount = 0;

	public SimMapNode[,] BaseMap{get;set;}
	public SimGameCharacter[] Characters;
	
	public List<SimJavelin> Javelins;
	public List<SimMonsterCharacter> Monsters;
	public List<SimPortal> Portals;
	public List<SimPotion> Potions;
	public List<SimTreasure> Treasures;
	public List<SimTrap> Traps;

	public int turnCount;

//	public HashSet<Pairing> PathDatabase { get; set; }

	public SimHero SimHero{get;set;}
	public SimEntrance SimEntrance{get;set;}
	public SimExit SimExit{get;set;}

	public string LevelString {get;set;}
	public SimLevel Clone(){
		SimLevel clone = new SimLevel(this);
		return clone;
	}

	public override bool Equals(object obj){
		var other = (SimLevel) obj;
		if(other == null){return false;}
		
		for(int x = 0; x < BaseMap.GetLength(0); x++){
			for(int y = 0; y < BaseMap.GetLength(1); y++){
				if(BaseMap[x,y] != other.BaseMap[x,y]){
					return false;
				}
			}
		}

		for(int i = 0; i < Characters.Length; i++){
			if(!Characters[i].Equals(other.Characters[i])){return false;}
		}

		return true;
	}

	public override int GetHashCode()
    {
    	int mapHash = BaseMap.GetLength(0) ^ BaseMap.GetLength(1);
    	int characterHash = 0;
    	for(int i = 0; i < Characters.Length; i++){
    		characterHash = Characters[i].GetHashCode();
    	}
        return mapHash + characterHash;
    }

	public SimLevel (string levelString){
		SimLevelState = SimLevelState.Playing;

		BaseMap = ParseBaseMap(levelString);
		Characters = ParseCharacterMap(levelString);

		Javelins = new List<SimJavelin>();
		Monsters = new List<SimMonsterCharacter>();
		Portals = new List<SimPortal>();
		Potions = new List<SimPotion>();
		Treasures = new List<SimTreasure>();
		Traps = new List<SimTrap>();
		Logger = new Logger();
		turnCount = 0;
//		PathDatabase = new HashSet<Pairing>();

		AssignCharacters(Characters);

		LevelString = levelString;
		//		BuildDB (this);
	}

	public SimMapNode[,] ParseBaseMap(string levelString){
		string[] lines = null;

		var platformId = Environment.OSVersion.Platform;

		switch (platformId) {
		case PlatformID.Unix:
			lines = levelString.Split ('\n');
			break;
		case PlatformID.MacOSX:
			lines = levelString.Split ('\n');
			break;
		default:
			lines = levelString.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);	
			break;
		}

		/*if(UnityEngine.Application.platform ++ UnityEngine.RuntimePlatform.WindowsEditor || UnityEngine.RuntimePlatform.WindowsPlayer){
			
		} else {
			
		}*/
		
		int width = lines[0].Length;
		int height = lines.Length;
		var result = new SimMapNode[width,height];
		for(int y = 0; y < lines.Length; y++){
			char[] row = lines[y].ToCharArray();
			for(int x = 0; x < row.Length; x++){
				if(row[x].Equals('X')){
					result[x,y] = new SimMapNode(new SimPoint(x,y), TileTypes.wall);
				} else {
					result[x,y] = new SimMapNode(new SimPoint(x,y), TileTypes.empty);
				}

			}
		}
		return result;
	}

	public SimGameCharacter[] ParseCharacterMap(string levelString){
		var values = new MagicNumberValues();

		string[] lines = levelString.Split('\n');

		var characterList = new List<SimGameCharacter>();
		var portalList = new List<SimPortal>();

		for(int y = 0; y < lines.Length; y++){
			char[] row = lines[y].ToCharArray();
			for(int x = 0; x < row.Length; x++){
				if(row[x].Equals('H')){
					var hero = new SimHero(true, new SimPoint(x,y), new SimPoint(x,y), values.HeroHealth, 0, 0, values.HeroDamage, 1, 0, 0,0,0);
					characterList.Add(hero);
				}
			}
		}

		for(int y = 0; y < lines.Length; y++){
			char[] row = lines[y].ToCharArray();
			for(int x = 0; x < row.Length; x++){
				SimGameCharacter newCharacter = null;
				switch(row[x]){
					case 'M':
					newCharacter = new SimMeleeMonster(true, new SimPoint(x,y), new SimPoint(x,y), values.MeleeMonsterHealth, values.MeleeMonsterDamage);
					break;
					case 'R':
					newCharacter = new SimRangedMonster(true, new SimPoint(x,y), new SimPoint(x,y), values.RangedMonsterHealth, values.RangedMonsterDamage, values.RangedMonsterAttackRange);
					break;
					case 'o':
					newCharacter = new SimOgreMonster(true, new SimPoint(x,y), new SimPoint(x,y), values.OgreMonsterHealth, values.OgreMonsterDamage);
					break;
					case 'b':
					newCharacter = new SimBlobMonster(true, new SimPoint(x,y), new SimPoint(x,y), values.BlobMonsterHealth, values.BlobMonsterDamage, 1);
					break;
					case 'm':
					newCharacter = new SimMinitaurMonster(true, new SimPoint(x,y), new SimPoint(x,y), values.MinitaurMonsterHealth, values.MinitaurMonsterDamage, 0, 0);
					break;
					case 'P':
					newCharacter = new SimPotion(true, new SimPoint(x,y), new SimPoint(x,y), values.PotionHealth, 0, values.PotionHealing);
					break;
					case 'p':
					newCharacter = new SimPortal(true, new SimPoint(x,y), new SimPoint(x,y), 1, 0);
					portalList.Add((SimPortal)newCharacter);
					break;
					case 'T':
					newCharacter = new SimTreasure(true, new SimPoint(x,y), new SimPoint(x,y), 1, 0);
					break;
					case 't':
					newCharacter = new SimTrap(true, new SimPoint(x,y), new SimPoint(x,y), values.TrapHealth, values.TrapDamage);
					break;
					case 'E':
					newCharacter = new SimEntrance(true, new SimPoint(x,y), new SimPoint(x,y), 1, 0);
                    startingEnteranceCount++;
					break;
					case 'e':
                    startingExitCount++;
					newCharacter = new SimExit(true, new SimPoint(x,y), new SimPoint(x,y), 1, 0);
					break;
				}
				if(newCharacter != null){
					characterList.Add(newCharacter);
				}
			}
		}

		//Link portals
		if(portalList.Count() > 0){
			if(portalList.Count() == 2){
				portalList[0].OtherPortalPoint = portalList[1].Point;
				portalList[1].OtherPortalPoint = portalList[0].Point;
			} else {
 				throw new WrongNumberOfPortalsException("Wrong number of portals: " + portalList.Count());
			}
		}

		return characterList.ToArray();		
	}

	public void AssignCharacters(SimGameCharacter[] characters){
		for(int character = 0; character < characters.Length; character++){
			switch(characters[character].CharacterType){
					case GameCharacterTypes.BlobMonster:
					Monsters.Add((SimBlobMonster)characters[character]);
					break;
					case GameCharacterTypes.Entrance:
					SimEntrance = (SimEntrance)characters[character];
					break;
					case GameCharacterTypes.Exit:
					SimExit = (SimExit)characters[character];
					break;
					case GameCharacterTypes.Hero:
					SimHero = (SimHero)characters[character];
					break;
					case GameCharacterTypes.Javelin:
					Javelins.Add((SimJavelin)characters[character]);
					break;
					case GameCharacterTypes.MeleeMonster:
					Monsters.Add((SimMonsterCharacter)characters[character]);
					break;
					case GameCharacterTypes.MinitaurMonster:
					Monsters.Add((SimMinitaurMonster)characters[character]);
					break;
					case GameCharacterTypes.OgreMonster:
					Monsters.Add((SimOgreMonster)characters[character]);
					break;
					case GameCharacterTypes.Portal:
					Portals.Add((SimPortal)characters[character]);
					break;
					case GameCharacterTypes.Potion:
					Potions.Add((SimPotion)characters[character]);
					break;
					case GameCharacterTypes.RangedMonster:
					Monsters.Add((SimRangedMonster)characters[character]);
					break;
					case GameCharacterTypes.Trap:
					Traps.Add((SimTrap)characters[character]);
					break;
					case GameCharacterTypes.Treasure:
					Treasures.Add((SimTreasure)characters[character]);
					break;
				}
		}
	}

	public SimLevel (SimLevel level){
		SimLevelState = level.SimLevelState;

		BaseMap = new SimMapNode[level.BaseMap.GetLength(0),level.BaseMap.GetLength(1)];
		for(int i = 0; i < level.BaseMap.GetLength(0); i++){
			for(int j = 0; j < level.BaseMap.GetLength(1); j++){
				BaseMap[i,j] = level.BaseMap[i,j].Clone();
			}
		}

		Characters = new SimGameCharacter[level.Characters.Length];
		
		Javelins = new List<SimJavelin>();
		Monsters = new List<SimMonsterCharacter>();
		Portals = new List<SimPortal>();
		Potions = new List<SimPotion>();
		Treasures = new List<SimTreasure>();
		Traps = new List<SimTrap>();
		LevelString = level.LevelString;
		Logger = new Logger(level.Logger);
		turnCount = level.turnCount;
//		PathDatabase = level.PathDatabase;

		int counter = 0;
		
		for(int character = 0; character < level.Characters.Length; character++){
			SimGameCharacter clone = level.Characters[character].Clone();
			
			Characters[counter] = clone;

			switch(clone.CharacterType){
				case GameCharacterTypes.BlobMonster:
				Monsters.Add((SimMonsterCharacter)clone);
				break;
				case GameCharacterTypes.Entrance:
				SimEntrance = (SimEntrance)clone;
				break;
				case GameCharacterTypes.Exit:
				SimExit = (SimExit)clone;
				break;
				case GameCharacterTypes.Hero:
				SimHero = (SimHero)clone;
				break;
				case GameCharacterTypes.Javelin:
				Javelins.Add((SimJavelin)clone);
				break;
				case GameCharacterTypes.MeleeMonster:
				Monsters.Add((SimMonsterCharacter)clone);
				break;
				case GameCharacterTypes.MinitaurMonster:
				Monsters.Add((SimMinitaurMonster)clone);
				break;
				case GameCharacterTypes.OgreMonster:
				Monsters.Add((SimOgreMonster)clone);
				break;
				case GameCharacterTypes.Portal:
				Portals.Add((SimPortal)clone);
				break;
				case GameCharacterTypes.Potion:
				Potions.Add((SimPotion)clone);
				break;
				case GameCharacterTypes.RangedMonster:
				Monsters.Add((SimRangedMonster)clone);
				break;
				case GameCharacterTypes.Trap:
				Traps.Add((SimTrap)clone);
				break;
				case GameCharacterTypes.Treasure:
				Treasures.Add((SimTreasure)clone);
				break;
			}
			counter++;
		}
	}

	public void SetActionCaching(bool cacheActions){
		for(int i = 0; i < Characters.Length; i++){
			Characters[i].SetActionCaching(cacheActions);
		}
	}

	public void RunTurn(SimHeroAction heroAction){
		SimHero.SetNextAction(heroAction);
		for(int character = 0; character < Characters.Length; character++){
			Characters[character].TakeTurn(this);
		}
		this.turnCount += 1;
	}

	public void LogMechanic(Mechanic mechanic)
    {
		this.Logger.LogMechanic(this.turnCount, mechanic);
    }

	public bool MoveIsLegal(SimGameCharacter character, SimHeroAction action){
		if(action.ActionType == HeroActionTypes.JavelinThrow){
			return LineOfSight(character.Point, action.DirectionOrTarget);
		}
		else if (action.ActionType == HeroActionTypes.None)
        {
			return true;
        }
		else {
			return MoveIsLegal(character, action.DirectionOrTarget);
		}
	}

	public bool MoveIsLegal(SimGameCharacter character, SimPoint move){
		if(move.X > 1 || move.Y > 1){
			return false;
		}
		if(move.X < -1 || move.Y < -1){
			return false;
		}

		SimPoint newPoint = character.Point + move;
		if(newPoint.X < 0 || newPoint.Y < 0){
			return false;
		}
		if(newPoint.X > BaseMap.GetLength(0) || newPoint.Y > BaseMap.GetLength(1)){
			return false;
		}
		if(BaseMap[newPoint.X,newPoint.Y].TileType == TileTypes.wall || BaseMap[newPoint.X,newPoint.Y].TileType == TileTypes.fillerWall){
			return false;	
		}
		return true;
	}

	public SimPoint[] AStar(SimPoint start, SimPoint end){
		try { 
		SimAStar<SimMapNode,object> simAStar = new SimAStar<SimMapNode,Object>(BaseMap);
		LinkedList<SimMapNode> path = simAStar.Search(start,end,null);
		SimPoint[] result = new SimPoint[path.Count];
		int count = 0;
		while(path.Count > 0){
			result[count] = path.First().Point;
			path.RemoveFirst();
			count++;
		}
		return result;
		}
		catch {
			return null;
        }
	}

	public bool LineOfSight(SimPoint start, SimPoint end){
		bool result = true;
		SimPoint[] rayTrace = RayTrace(start, end);
		for(int point = 0; point < rayTrace.Length; point++){
			if(BaseMap[rayTrace[point].X,rayTrace[point].Y].TileType == TileTypes.wall || BaseMap[rayTrace[point].X,rayTrace[point].Y].TileType == TileTypes.fillerWall){
				result = false;
			}
		}
		return result;
	}

	public List<SimPoint> GetPossibleJavelinTargets(){
		var possibleTargets = new List<SimPoint>();
		for(int monster = 0; monster < Monsters.Count; monster++){
			if(LineOfSight(SimHero.Point, Monsters[monster].Point)){
				possibleTargets.Add(Monsters[monster].Point);
			}
		}

		return possibleTargets;
	}

	public SimPoint[] RayTrace(SimPoint p1, SimPoint p2){
		int x0 = p1.X;
		int y0 = p1.Y;
		int x1 = p2.X;
		int y1 = p2.Y;

		List<SimPoint> trace = new List<SimPoint> ();

		int dx = Math.Abs (x1 - x0);
		int dy = Math.Abs (y1 - y0);
		int x = x0;
		int y = y0;
		int n = 1 + dx + dy;
		int x_inc = (x1 > x0) ? 1 : -1;
		int y_inc = (y1 > y0) ? 1 : -1;
		int error = dx - dy;
		dx *= 2;
		dy *= 2;

		for (int tiles = n; tiles > 0; tiles--) {
			trace.Add(new SimPoint(x,y));
			if(error > 0){
				x += x_inc;
				error -= dy;
			} else {
				y += y_inc;
				error += dx;
			}
		}

		SimPoint[] result = trace.ToArray();
		return result;
	}

	public List<SimHeroAction> GetPossibleHeroActions(){
        var possibleActions = new List<SimHeroAction>();
        
        if(SimHero.Javelins > 0){
	        List<SimPoint> javelinTargets = GetPossibleJavelinTargets();
	        for(int target = 0; target < javelinTargets.Count; target++){
	        	SimHeroAction javelinThrow = new SimHeroAction (HeroActionTypes.JavelinThrow, javelinTargets[target]);
	        	if(!possibleActions.Contains(javelinThrow)){
	        		possibleActions.Add(javelinThrow);
	        	}
	        }
	    }

	    for(int direction = 0; direction < _directions.Length; direction++){
	    	if(MoveIsLegal(SimHero, _directions[direction])){
	    		SimHeroAction move = new SimHeroAction(HeroActionTypes.Move, _directions[direction]);
	    		if(!possibleActions.Contains(move)){
	    			possibleActions.Add(move);
	    		}
	    	}
	    }

		return possibleActions; //TODO: Maybe optimize this method too?
    }

	public string ToAsciiMap(){
		string result = "";

		result += "Health: " + SimHero.Health + " " + SimHero.Point.ToString() + "\n";

		string[,] asciiMap = new string[BaseMap.GetLength(0),BaseMap.GetLength(1)];
		for(int x = 0; x < BaseMap.GetLength(0); x++){
			for(int y = 0; y < BaseMap.GetLength(1); y++){
				if(BaseMap[x,y].TileType == TileTypes.wall || BaseMap[x,y].TileType == TileTypes.fillerWall){
					asciiMap[x,y] = "X";	
				}
				if(BaseMap[x,y].TileType == TileTypes.empty){
					asciiMap[x,y] = " ";
				}
			}
		}
		foreach(SimGameCharacter character in Characters){
			string type = "~";
			if(character is SimHero){
				if(character.Alive){
					type = "H";
				} else {
					type = "☠";
				}
			} else {
				if(character is SimMeleeMonster){if(!character.Alive){type="∞";}else{type = "M";}}
				if(character is SimRangedMonster){if(!character.Alive){type="∞";}else{type = "R";}}
				if(character is SimOgreMonster){if(!character.Alive){type="∞";}else{type = "o";}}
				if(character is SimBlobMonster){if(!character.Alive){type="∞";}else{type = "b";}}
				if(character is SimMinitaurMonster){if(!character.Alive){type="∞";}else{type = "m";}}

				if(character is SimPotion){if(!character.Alive){type=" ";}else{type = "P";}}
				// THIS IS A KNOWN BUG, but since it was in the last version, we need it in there
				if(character is SimPortal){if(!character.Alive){type=" ";}else{type = "T";}}
				if(character is SimEntrance){if(!character.Alive){type=" ";}else{type = "E";}}
				if(character is SimExit){if(!character.Alive){type=" ";}else{type = "e";}}
				if(character is SimTreasure){if(!character.Alive){type=" ";}else{type = "T";}}
				if(character is SimTrap){if(!character.Alive){type=" ";}else{type = "t";}}
			}

			asciiMap[character.Point.X,character.Point.Y] = type;
		}

		for(int y = 0; y < asciiMap.GetLength(1); y++){
			for(int x = 0; x < asciiMap.GetLength(0); x++){
				result += asciiMap[x,y];
			}
			result += "\n";
		}
		
		return result;

	}


	//.............................................//
	public List<SimPoint> GetAllCharecters(){
		var allCharecters = new List<SimPoint>();

		//add monsters
		for(int monster = 0; monster < Monsters.Count; monster++){
				allCharecters.Add(Monsters[monster].Point);
		}

		//add treasures
		for(int treasures = 0; treasures < Treasures.Count; treasures++){
				allCharecters.Add(Treasures[treasures].Point);
		}

		//add potions
		for(int potions = 0; potions < Potions.Count; potions++){
				allCharecters.Add(Potions[potions].Point);
		}

		return allCharecters;
	}

	public int GetMonsterKilled()
	{
		SimLevel level = this;
		int monsterKills = 0;
		for (int i = 0; i < level.Characters.Length; i++)
		{
			if (level.Characters[i].CharacterType == GameCharacterTypes.MeleeMonster || level.Characters[i].CharacterType == GameCharacterTypes.RangedMonster || level.Characters[i].CharacterType == GameCharacterTypes.BlobMonster || level.Characters[i].CharacterType == GameCharacterTypes.OgreMonster)
			{
				if (!level.Characters[i].Alive)
				{
					monsterKills++;
				}
			}
		}
		return monsterKills;
	}

	//............................................//
		/*public SimGameCharacter SimGameCharacterFromCharacter(GameCharacter gameCharacter){
			if(gameCharacter is Hero){
				Hero hero = (Hero)gameCharacter;
				SimHero myHero = new SimHero(
						hero._alive,
						new SimPoint(hero._priorPoint.X,Math.Abs(hero._priorPoint.Y)),
						new SimPoint(hero._gridPoint.X,Math.Abs(hero._gridPoint.Y)),
						hero._health,
						hero._potionsDrunk,
						hero._damageTaken,
						hero._damage,
						hero._javelinsInInventory,
						hero._stepsTaken,
						hero._javelinThrows,
						hero._treasuresOpened,
						hero._potionsDrunk
					);
				SimHero = myHero;
				return myHero;
			}
			if(gameCharacter is Javelin){
				SimJavelin result = new SimJavelin(
						gameCharacter._alive,
						new SimPoint(gameCharacter._priorPoint.X,Math.Abs(gameCharacter._priorPoint.Y)),
						new SimPoint(gameCharacter._gridPoint.X,Math.Abs(gameCharacter._gridPoint.Y)),
						gameCharacter._health,
						gameCharacter._damage
					);
				Javelins.Add(result);
				return result;
			}
			if(gameCharacter is BlobMonster){
				BlobMonster blobMonster = (BlobMonster) gameCharacter;
				SimBlobMonster result = new SimBlobMonster(
						gameCharacter._alive,
						new SimPoint(gameCharacter._priorPoint.X,Math.Abs(gameCharacter._priorPoint.Y)),
						new SimPoint(gameCharacter._gridPoint.X,Math.Abs(gameCharacter._gridPoint.Y)),
						gameCharacter._health,
						gameCharacter._damage,
						blobMonster._damage
				);
				Monsters.Add(result);
				return result;
			}
			if(gameCharacter is MeleeMonster){
				SimMeleeMonster result = new SimMeleeMonster(
						gameCharacter._alive,
						new SimPoint(gameCharacter._priorPoint.X,Math.Abs(gameCharacter._priorPoint.Y)),
						new SimPoint(gameCharacter._gridPoint.X,Math.Abs(gameCharacter._gridPoint.Y)),
						gameCharacter._health,
						gameCharacter._damage
				);
				Monsters.Add(result);
				return result;
			}
			if(gameCharacter is MinitaurMonster){
				MinitaurMonster minitaurMonster = (MinitaurMonster) gameCharacter;
				SimMinitaurMonster result = new SimMinitaurMonster(
						gameCharacter._alive,
						new SimPoint(gameCharacter._priorPoint.X,Math.Abs(gameCharacter._priorPoint.Y)),
						new SimPoint(gameCharacter._gridPoint.X,Math.Abs(gameCharacter._gridPoint.Y)),
						gameCharacter._health,
						gameCharacter._damage,
						minitaurMonster.knockOutCounter,
						0 //TODO this is a hack, it should have a counter for how many times it has been knocked out.
				);
				Monsters.Add(result);
				return result;
			}
			if(gameCharacter is OgreMonster){
				SimOgreMonster result = new SimOgreMonster(
						gameCharacter._alive,
						new SimPoint(gameCharacter._priorPoint.X,Math.Abs(gameCharacter._priorPoint.Y)),
						new SimPoint(gameCharacter._gridPoint.X,Math.Abs(gameCharacter._gridPoint.Y)),
						gameCharacter._health,
						gameCharacter._damage
				);
				Monsters.Add(result);
				return result;
			}
			if(gameCharacter is RangedMonster){
				RangedMonster rangedMonster = (RangedMonster) gameCharacter;
				SimRangedMonster result = new SimRangedMonster(
						gameCharacter._alive,
						new SimPoint(gameCharacter._priorPoint.X,Math.Abs(gameCharacter._priorPoint.Y)),
						new SimPoint(gameCharacter._gridPoint.X,Math.Abs(gameCharacter._gridPoint.Y)),
						gameCharacter._health,
						gameCharacter._damage,
						rangedMonster.range
				);
				Monsters.Add(result);
				return result;
			}
			if(gameCharacter is Entrance){
				SimEntrance result = new SimEntrance(
						gameCharacter._alive,
						new SimPoint(gameCharacter._priorPoint.X,Math.Abs(gameCharacter._priorPoint.Y)),
						new SimPoint(gameCharacter._gridPoint.X,Math.Abs(gameCharacter._gridPoint.Y)),
						gameCharacter._health,
						gameCharacter._damage
				);
				SimEntrance = result;
				return result;
			}
			if(gameCharacter is Exit){
				SimExit result = new SimExit(
						gameCharacter._alive,
						new SimPoint(gameCharacter._priorPoint.X,Math.Abs(gameCharacter._priorPoint.Y)),
						new SimPoint(gameCharacter._gridPoint.X,Math.Abs(gameCharacter._gridPoint.Y)),
						gameCharacter._health,
						gameCharacter._damage
				);
				SimExit = result;
				return result;
			}
			if(gameCharacter is Portal){
				Portal portal = (Portal)gameCharacter;
				SimPortal result = new SimPortal(
						gameCharacter._alive,
						new SimPoint(gameCharacter._priorPoint.X,Math.Abs(gameCharacter._priorPoint.Y)),
						new SimPoint(gameCharacter._gridPoint.X,Math.Abs(gameCharacter._gridPoint.Y)),
						gameCharacter._health,
						gameCharacter._damage,
						new SimPoint(portal.otherPortal._gridPoint.X,Math.Abs(portal.otherPortal._gridPoint.Y))
				);
				Portals.Add(result);
				return result;

			}
			if(gameCharacter is Potion){
				Potion potion = (Potion) gameCharacter;
				SimPotion result = new SimPotion(
						gameCharacter._alive,
						new SimPoint(gameCharacter._priorPoint.X,Math.Abs(gameCharacter._priorPoint.Y)),
						new SimPoint(gameCharacter._gridPoint.X,Math.Abs(gameCharacter._gridPoint.Y)),
						gameCharacter._health,
						gameCharacter._damage,
						potion.healing
				);
				Potions.Add(result);
				return result;
			}
			if(gameCharacter is Trap){
				SimTrap result = new SimTrap(
						gameCharacter._alive,
						new SimPoint(gameCharacter._priorPoint.X,Math.Abs(gameCharacter._priorPoint.Y)),
						new SimPoint(gameCharacter._gridPoint.X,Math.Abs(gameCharacter._gridPoint.Y)),
						gameCharacter._health,
						gameCharacter._damage
				);
				Traps.Add(result);
				return result;
			}
			if(gameCharacter is Treasure){
				SimTreasure result =  new SimTreasure(
						gameCharacter._alive,
						new SimPoint(gameCharacter._priorPoint.X,Math.Abs(gameCharacter._priorPoint.Y)),
						new SimPoint(gameCharacter._gridPoint.X,Math.Abs(gameCharacter._gridPoint.Y)),
						gameCharacter._health,
						gameCharacter._damage
				);
				Treasures.Add(result);
				return result;
			}

			return null;
		}*/

}
