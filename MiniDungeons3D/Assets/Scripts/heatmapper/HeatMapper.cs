using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;


public class SimHeatMapNode{

	public SimPoint Point{get;set;}
	public int VisitCount{get;set;}

	public SimHeatMapNode(SimPoint point, int visitCount){
		Point = point;
		VisitCount = visitCount;
	}
}

public class ActionMap{
	public int[,] _visitCounts;
	public int[,] _javelinCounts;

	public ActionMap(int width, int height){
		_visitCounts = new int[width,height];
		_javelinCounts = new int[width,height];
	}
}

public class GridMap{
	public string _heatMapFile;
	public string _mapFile;
	public string _visitMap;
	public string _levelMap;
	public string _mapName;
}

public enum HeatMapSource{
	GridCount,
	ActionSequence
}

public class HeatMapper : MonoBehaviour {

	public GameObject[] _heatMapSprites;

	public HeatMapSource _heatMapSource = HeatMapSource.GridCount;

	public float _displayYOffset;

	public GameObject[] _moveColors;
	public GameObject[] _javelinColors;

	public GameObject[,] _moveRender;
	public GameObject[,] _javelinRender;

	public LevelView _levelView;

	public ActionMap _actionMap;
	public string _actionString;
	List<SimHeroAction> _actions;

	SimPoint _heroStartPoint;

	public Dictionary<string,string> _gridHeatMapData;
	public string _gridDataRoot = "/Resources/HeatMapData/GridBased/";

	public Dictionary<string,string> _actionSequenceHeatMapData;
	public string _actionDataRoot = "/Resources/HeatMapData/ActionBased/";

	public float _tileWidth = -0.2f;
	public List<GameObject> _instantiatedHeatTiles;

	void Start(){
		_gridHeatMapData = new Dictionary<string, string>();
		_actionSequenceHeatMapData = new Dictionary<string, string>();

		_instantiatedHeatTiles = new List<GameObject>();

		List<string> fileNames = GetFilesRecursively(_gridDataRoot);
		//List<GridMap> gridMaps = BuildGridMaps(fileNames);
		
		//StartCoroutine(SaveHeatMapToDisk(gridMaps));
	}

	public static List<GridMap> BuildGridMaps(List<string> fileNames){
		var result = new List<GridMap>();

		foreach(string fileName in fileNames){
			if(fileName.Contains(".meta") || fileName.Contains(".png") || fileName.Contains(".DS_Store")) continue;

			int idx = fileName.ToLower().LastIndexOf("map",StringComparison.CurrentCulture);

			string mapName = fileName.Substring(idx);

			idx = fileName.IndexOf("Resources/",StringComparison.CurrentCulture);
			string fileLocation = fileName.Substring(idx + "Resources/".Length);

			var gridMap = new GridMap();
			gridMap._mapName = mapName;
			gridMap._heatMapFile = fileLocation.Replace(".txt", "");
			gridMap._mapFile = "AsciiMaps/" + gridMap._mapName.Replace(".txt", "");

			/*Debug.Log(	"Filename: " + fileName + "\n" + 
						"Map name: " + gridMap._mapName +"\n" +
						"Heat map file: " + gridMap._heatMapFile + "\n" +
						"Map file: " + gridMap._mapFile);*/

			gridMap._levelMap = Resources.Load<TextAsset>(gridMap._mapFile).text;
			gridMap._visitMap = Resources.Load<TextAsset>(gridMap._heatMapFile).text;

			result.Add(gridMap);
		}
		return result;
	}

	public void AddGridBasedHeatMap(GridMap gridMap){
		SimHeatMapNode[,] heatMap = ParseHeatMap(gridMap._visitMap);		
		LayoutHeatMapView(heatMap);
	}

	public void RemoveHeatMap(){
		foreach(GameObject heatTile in _instantiatedHeatTiles){
			GameObject.Destroy(heatTile);
		}
	}

	void LayoutHeatMapView(SimHeatMapNode[,] heatMap){
		for(int x = 0; x < heatMap.GetLength(0); x++){
			for(int y = 0; y < heatMap.GetLength(1); y++){
				//Debug.Log("x: " + x + " y:" + y + " count: " + heatMap[x,y].VisitCount);

				GameObject _tilePrefab = _heatMapSprites[heatMap[x,y].VisitCount];

				Vector3 newPosition = transform.position +  new Vector3(_tilePrefab.GetComponent<Renderer>().bounds.size.x * x, _tilePrefab.GetComponent<Renderer>().bounds.size.y * y * -1, 0);
				//Debug.Log(newPosition);
				GameObject newTile = (GameObject)Instantiate(_tilePrefab, newPosition, Quaternion.identity);
				_instantiatedHeatTiles.Add(newTile);

                //newTile.transform.parent = transform;
			}
		}
	}

	public SimHeatMapNode[,] ParseHeatMap(string levelString){
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

		//TODO: Protect against empty lines at the end.
		
		int width = lines[0].Length;
		int height = lines.Length;
		var result = new SimHeatMapNode[width,height];
		for(int y = 0; y < lines.Length; y++){
			char[] row = lines[y].ToCharArray();
			for(int x = 0; x < row.Length; x++){
				int visitValue = 0;
				int.TryParse(row[x].ToString(), out visitValue);
				result[x,y] = new SimHeatMapNode(new SimPoint(x,y), visitValue);
			}
		}
		return result;
	}

	public IEnumerator SaveHeatMapToDisk(List<GridMap> gridMaps){
		//GridMap gridMap = gridMaps[0];

		//Debug.Log("Heatmap file: " + gridMap._heatMapFile);

		foreach(GridMap gridMap in gridMaps){
			//Debug.Log("Level map: " + gridMap._levelMap);
			var level = new SimLevel(gridMap._levelMap);
			_levelView.Initialize(level);
			_levelView.RefreshView(level);
			_levelView.DepthSortGameCharacters();

			AddGridBasedHeatMap(gridMap);

			yield return new WaitForEndOfFrame();

			var tex = new Texture2D(Screen.width, Screen.height);
			tex.ReadPixels(new Rect(0,0,Screen.width,Screen.height),0,0);
			tex.Apply();

			byte[] bytes = tex.EncodeToPNG();

			string outputDirectory = Application.dataPath + "/GeneratedHeatMaps/";
			if(!Directory.Exists(outputDirectory)){
				Directory.CreateDirectory(outputDirectory);
			}

			string heatMapFileName = gridMap._heatMapFile.Substring(gridMap._heatMapFile.LastIndexOf("/", StringComparison.CurrentCulture));
			File.WriteAllBytes(outputDirectory + heatMapFileName + ".png", bytes);
			Debug.Log("Output-file: " + heatMapFileName + ".png");
			RemoveHeatMap();
			_levelView.ClearView();
		}
	}

	public List<string> GetFilesRecursively(string rootDirectory){
		var result = new List<string>();
		string[] files = Directory.GetFiles(Application.dataPath + rootDirectory);
		foreach(string file in files){
			result.Add(file);
		}
		string[] directories = Directory.GetDirectories(Application.dataPath + rootDirectory);
		foreach(string directory in directories){
			//Debug.Log(directory);
			result.AddRange(GetFilesRecursively(directory.Replace(Application.dataPath, "")));
		}

		return result;
	}

	public void ClearHeatMap(){
		for(int x = 0; x < _moveRender.GetLength(0); x++){
			for(int y = 0; y < _moveRender.GetLength(1); y++){
				if(_moveRender[x,y] != null){
					Destroy(_moveRender[x,y]);
				}
				if(_javelinRender[x,y] != null){
					Destroy(_javelinRender[x,y]);
				}
			}
		}
	}

	public void InitializeAndRender(LevelView levelView){
		_levelView = levelView;
		_levelView.Animating = false;
		_actions = ActionTraceUtils.ReadActionString(_actionString);
		_actionMap = BuildActionMap(_levelView._level, _actions, levelView);
		RenderActionMap(_actionMap);
		levelView.RefreshView(levelView._level);
		levelView.DepthSortGameCharacters();
	}

	public void RenderActionMap(ActionMap _actionMap){
		_moveRender = new GameObject[_actionMap._visitCounts.GetLength(0),_actionMap._visitCounts.GetLength(1)];
		_javelinRender = new GameObject[_actionMap._visitCounts.GetLength(0),_actionMap._visitCounts.GetLength(1)];

		for(int x = 0; x < _moveRender.GetLength(0); x++){
			for(int y = 0; y < _moveRender.GetLength(1); y++){
				GameObject moveOverlay = null;
				GameObject javelinOverlay = null;
				if(_actionMap._visitCounts[x,y] == 1){
					moveOverlay = Instantiate(_moveColors[0]);
				}
				if(_actionMap._visitCounts[x,y] == 2){
					moveOverlay = Instantiate(_moveColors[1]);
				}
				if(_actionMap._visitCounts[x,y] >= 3){
					moveOverlay = Instantiate(_moveColors[2]);
				}
				if(_actionMap._javelinCounts[x,y] == 1){
					javelinOverlay = Instantiate(_javelinColors[0]);
				}
				if(_actionMap._javelinCounts[x,y] == 2){
					javelinOverlay = Instantiate(_javelinColors[1]);	
				}
				if(_actionMap._javelinCounts[x,y] >= 3){
					javelinOverlay = Instantiate(_javelinColors[2]);
				}
				if(moveOverlay != null){
					moveOverlay.transform.position = new Vector3(_levelView._map[x,y].transform.position.x,_levelView._map[x,y].transform.position.y + _displayYOffset,_levelView._map[x,y].transform.position.z);
					_moveRender[x,y] = moveOverlay;
				}
				if(javelinOverlay != null){
					javelinOverlay.transform.position = new Vector3(_levelView._map[x,y].transform.position.x,_levelView._map[x,y].transform.position.y+_displayYOffset,_levelView._map[x,y].transform.position.z);
					_javelinRender[x,y] = javelinOverlay;
				}
			}
		}
	}

	ActionMap BuildActionMap(SimLevel level, List<SimHeroAction> actions, LevelView levelView){
		var result = new ActionMap(level.BaseMap.GetLength(0), level.BaseMap.GetLength(1));
		//SimPoint heroPoint = level.SimHero.Point;
		foreach(SimHeroAction action in actions){
			level.RunTurn(action);
			levelView.RefreshView(level);
			result._visitCounts[level.SimHero.Point.X,level.SimHero.Point.Y]++;
			if(action.ActionType == HeroActionTypes.JavelinThrow){result._javelinCounts[action.DirectionOrTarget.X,action.DirectionOrTarget.Y]++;}
		}
		return result;
	}
}
