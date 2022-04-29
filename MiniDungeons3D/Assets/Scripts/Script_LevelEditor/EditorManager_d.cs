using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class EditorManager_d : MonoBehaviour {

	public DisplayManager_d displayManager; 
	FileEditor _fileEditor;
	public GameObject Content,plane,backBtn,playBtn,genBtn;
	public GameObject heroIcon, exitIcon, wallIcon, wallIcon1, blobIcon, chestIcon, meleeMonsterIcon, miniatureIcon, ogreIcon,portalIcon, potionIcon, rangedMonsterIcon, trapIcon, deleteIcon,emptyIcon;
	private bool entranceUsed, exitUsed,portalInUse,portalUsed,changeWall;
	private GameObject selectedIcon,displayObj,button1,button2,button3,button4,button5,button6,button7,button8,button9,button10,button11,button12,button13;
	private string iconName,file,pathStreamingAssets;
	private char activeASCII;

	void OnEnable()
	{
		// get the file where the map will be stored
		file = Application.persistentDataPath + "/" + displayManager.GetFile ()+".txt";

		// clear the screen
		string[] tagsToDelete = {"Icon","Portal"};
		foreach (string tag in tagsToDelete) {
			GameObject[] objects = GameObject.FindGameObjectsWithTag (tag);
			for (var i = 0; i < objects.Length; i++) {
				DestroyImmediate (objects [i]);
			}
		}

		//get the file editor
		_fileEditor = GetComponent<FileEditor>();

		// set up the upper boundary walls
		string[] upWalls = {"Button (0,1)","Button (0,2)","Button (0,3)","Button (0,4)","Button (0,5)","Button (0,6)","Button (0,7)","Button (0,8)" };
		foreach (string pos in upWalls) {
			GameObject obj =  GameObject.Find (pos);
			GameObject element = Instantiate (wallIcon, obj.transform) as GameObject;
			// scale the instane to fit in screen properly
			element.transform.localScale -= new Vector3 (0.1f, 0.3f, 0);
		}

		//instanciate the buttons in scrollbar
		button1 = Instantiate(heroIcon, Content.transform) as GameObject;
		button2 = Instantiate(exitIcon, Content.transform) as GameObject;
		button3 = Instantiate(wallIcon, Content.transform) as GameObject;
		// resize the wall icon
		button3.transform.localScale -= new Vector3 (0.1f, 0.3f, 0);
		button4 = Instantiate(blobIcon, Content.transform) as GameObject;
		button5 = Instantiate(rangedMonsterIcon, Content.transform) as GameObject;
		button6 = Instantiate(meleeMonsterIcon, Content.transform) as GameObject;
		button7 = Instantiate(miniatureIcon, Content.transform) as GameObject;
		button8 = Instantiate(ogreIcon, Content.transform) as GameObject;
		button9 = Instantiate(chestIcon, Content.transform) as GameObject;
		button10 = Instantiate(potionIcon, Content.transform) as GameObject;
		button11= Instantiate(portalIcon, Content.transform) as GameObject;
		button12= Instantiate(trapIcon, Content.transform) as GameObject;
		button13= Instantiate(deleteIcon, Content.transform) as GameObject;

		//add action listener to the buttons
		button1.GetComponent<Button>().onClick.AddListener(() => IconClick());
		button2.GetComponent<Button>().onClick.AddListener(() => IconClick());
		button3.GetComponent<Button>().onClick.AddListener(() => IconClick());
		button4.GetComponent<Button>().onClick.AddListener(() => IconClick());
		button5.GetComponent<Button>().onClick.AddListener(() => IconClick());
		button6.GetComponent<Button>().onClick.AddListener(() => IconClick());
		button7.GetComponent<Button>().onClick.AddListener(() => IconClick());
		button8.GetComponent<Button>().onClick.AddListener(() => IconClick());
		button9.GetComponent<Button>().onClick.AddListener(() => IconClick());
		button10.GetComponent<Button>().onClick.AddListener(() => IconClick());
		button11.GetComponent<Button>().onClick.AddListener(() => IconClick());
		button12.GetComponent<Button>().onClick.AddListener(() => IconClick());
		button13.GetComponent<Button>().onClick.AddListener(() => IconClick());

		// if editing an existing map
		if(displayManager.IsEdit()){
			// set up the existing map in editor
			SetUpEditor();
		}
	}


	// listener for grid buttons 
	public void OnGridButtonClick()
	{
		//reset test data
		Variables.ResetVariables ();
		displayManager.SetPlayTestData ();
		displayManager.SetAutoTestData (null);

		int row, column;
		changeWall = false;
		// get the selected grid button
		GameObject selectedGrid = EventSystem.current.currentSelectedGameObject;
		//if any icon is selected in scrollbar

		if (selectedIcon != null) {
			// if an element is exists in the selected place
			if (selectedGrid.name.Contains ("Clone")) {
				//if existing entrance or exit or portal or wall is deleted 
				if (selectedGrid.name.Contains ("Hero")) {
					Activate (button1);
				} else if (selectedGrid.name.Contains ("Exit")) {
					Activate (button2);
				} else if (selectedGrid.name.Contains ("Wall")) {
					changeWall = true;
				} else if (selectedGrid.name.Contains ("Portal")) {
					DeletePortal (selectedGrid);
					Activate (button11);
					portalUsed = false;
				}

				// get the grid button 
				selectedGrid = selectedGrid.transform.parent.gameObject;
			}

			//get the selected object name
			string gridName = selectedGrid.name;

			// get the position from the name (names are in "Button(row,column)" format)
			gridName = gridName.Replace ("Button", "").Replace ("(", "").Replace (")", "");
			string[] pos = gridName.Split (new char[]{ ',' }, 2);

			// change the row and column no to integer
			row = System.Int32.Parse (pos [0]);
			column = System.Int32.Parse (pos [1]);

			//if a wall tile is deleted
			if (changeWall) {
				ArrangeWall (selectedGrid, row, column,"deleteWall");
			}

			// if selected icon is a wall set the wall arrangements
			if (iconName == "WallIcon") {
				ArrangeWall (selectedGrid, row, column,"addWall");
			}

			//instantiate the selcted gameobject in the selected position
			GameObject element = PlaceElement (displayObj, selectedGrid,row,column);
			// save the placed element in file
			_fileEditor.ChangeLine (file, file, row, column, activeASCII);

			// if entrance or exit icon is selected empty the iconName variable,so that this icons cannot be placed more than one
			if (iconName == "HeroIcon" || iconName == "ExitIcon") {
				selectedIcon = null;
			} // if a solid wall is placed,change wall to regular one
			else if (displayObj == wallIcon1) {
				displayObj = wallIcon;
			}// if portal icon is selected and one portal is placed on map then set portalUsed flag
			else if (iconName == "PortalIcon" && portalInUse == false) {
				portalInUse = true;
				Deactivate (backBtn);
				Deactivate (playBtn);
				Deactivate (genBtn);

			} else if (iconName == "PortalIcon" && portalInUse == true) {
				portalInUse = false;
				portalUsed = true;
				selectedIcon = null;
				Activate (backBtn);
				Activate (playBtn);
				Activate (genBtn);
			}

			//if entrance or exit or portal icon is used deactivate the respective button
			if (entranceUsed) {
				Deactivate (button1);
			} else if (exitUsed) {
				Deactivate (button2);
			} else if (portalUsed) {
				Deactivate (button11);
			}
		}
	}

	// action listener for scrollbar icon buttons
	void IconClick ()
	{
		// if portal icon is not in use (portal in use indicates one portal is placed and another is not placed yet,so user need to place the other portal then can select other icon)
		if(!portalInUse)
		{	
			entranceUsed = false;
			exitUsed = false;
			// get the selected item and get the icon name
			selectedIcon = EventSystem.current.currentSelectedGameObject;
			iconName = selectedIcon.name;
			iconName = iconName.Replace("(Clone)",""); 

			// when entrance or exit icon is selected change the respective flags and set the ascii code for each selection
			if(iconName == "HeroIcon")
			{
				displayObj = heroIcon;
				entranceUsed = true;
				activeASCII = 'H';
			}
			else if(iconName == "ExitIcon")
			{
				displayObj = exitIcon;
				exitUsed = true;
				activeASCII = 'e';
			}
			else if(iconName == "BlobIcon")
			{
				displayObj = blobIcon;
				activeASCII = 'b';
			}
			else if(iconName == "ChestIcon")
			{
				displayObj = chestIcon;
				activeASCII = 'T';
			}
			else if(iconName == "MeleeMonsterIcon")
			{
				displayObj = meleeMonsterIcon;
				activeASCII = 'M';
			}else if(iconName == "MiniatureIcon")
			{
				displayObj = miniatureIcon;
				activeASCII = 'm';
			}else if(iconName == "OgreIcon")
			{
				displayObj = ogreIcon;
				activeASCII = 'o';
			}else if(iconName == "PortalIcon")
			{
				displayObj = portalIcon;
				activeASCII = 'p';
			}else if(iconName == "PotionIcon")
			{
				displayObj = potionIcon;
				activeASCII = 'P';
			}else if(iconName == "RangedMonsterIcon")
			{
				displayObj = rangedMonsterIcon;
				activeASCII = 'R';
			}else if(iconName == "TrapIcon")
			{
				displayObj = trapIcon;
				activeASCII = 't';
			}else if(iconName == "WallIcon")
			{
				displayObj = wallIcon;
				activeASCII = 'X';
			}else if(iconName == "DeleteIcon")
			{
				displayObj = emptyIcon;
				activeASCII = ' ';
			}
		}
	}


	// deactivate the given button
	void Deactivate (GameObject btn)
	{
		btn.GetComponent<Button>().interactable = false;
	}


	// activate the given button
	void Activate (GameObject btn)
	{
		btn.GetComponent<Button>().interactable = true;
	}


	// wall arrangements according to the upper and lower wall
	void ArrangeWall(GameObject grid,int row, int column,string change){
		int rowAbove,rowBelow ;
		//get the rows above and below of the selected grid position
		rowAbove = row - 1;
		rowBelow = row + 1;

		// get the up and down object by name
		GameObject upObject = GameObject.Find ("Button (" + rowAbove + "," + column+ ")");
		GameObject downObject = GameObject.Find ("Button (" + rowBelow + "," + column + ")");

		// if there is a regular wall above the selected position while addind a wall below
		if( upObject.transform.Find("WallIcon(Clone)") != null && change == "addWall"){
			// delete the existing regular wall
			Destroy (upObject.transform.GetChild(1).gameObject);
			//create a solid wall in that place
			PlaceElement (wallIcon1, upObject,rowAbove,column);
		}

		// if there is a wall below the selected position while adding a wall above
		if( (downObject.transform.Find("WallIcon(Clone)") != null || downObject.transform.Find("WallIcon1(Clone)") != null) && change == "addWall"){
			// change the wall to solid wall
			displayObj = wallIcon1;
		}

		// if there is a solid wall above the selected position while deleting the below wall
		if( upObject.transform.Find("WallIcon1(Clone)") != null && change == "deleteWall"){
			// delete the existing wall
			Destroy (upObject.transform.GetChild(1).gameObject);
			//create a regular wall in that place
			PlaceElement (wallIcon, upObject,rowAbove,column);
		}
	}


	// place given element in given position
	GameObject PlaceElement(GameObject objectToPlace, GameObject pos,int x,int y){
		// if the button already have an element, delete that
		if(pos.transform.childCount > 1){
			DestroyImmediate (pos.transform.GetChild (1).gameObject);
		}
		//instantiate gameobject in that position
		GameObject element = Instantiate (objectToPlace, pos.transform) as GameObject;
		// scale the instane to fit in screen properly
		element.transform.localScale -= new Vector3 (0.1f, 0.3f, 0);

		if (x != 0 && x != 19  &&  y != 0 && y != 9) {
			// add action listener to the newly instantiated object
			element.GetComponent<Button> ().onClick.AddListener (() => OnGridButtonClick ());
		}

	 if(objectToPlace == portalIcon){
			element.tag = "Portal";
		}
		return element;
	}


	// read elements from existing map and set them in editor screen
	void SetUpEditor(){
		//read file using stream reader
		StreamReader streamReader = new StreamReader(file);
		string fileData = null;
		while(!streamReader.EndOfStream)
		{
			//store the data in a string
			fileData += streamReader.ReadLine( ) + '\n';
		}
		streamReader.Close( ); 
		// split the string into array of strings
		string[] lines = fileData.Split ('\n');
		//loop through all the lines
		for(int row = 1; row <19 ; row++){
			// get the line in current row index
			string currentLine = lines[row];

			// loop through all characters of the selected line
			for (int col = 1; col < 9; col++) {
				// get character in current column index
				char  ASCII =currentLine[col];

				//if there is a valid character 
				if (ASCII != null && ASCII != ' ') {
					// find the position where the element will be placed
					GameObject parent = GameObject.Find ("Button (" + row + "," + col + ")");

					//set display object according to the ASCII 
					if (ASCII == 'H') {
						displayObj = heroIcon;
						Deactivate (button1);
					} else if (ASCII == 'e') {
						displayObj = exitIcon;
						Deactivate (button2);
					} else if (ASCII == 'b') {
						displayObj = blobIcon;
					} else if (ASCII == 'T') {
						displayObj = chestIcon;
					} else if (ASCII == 'M') {
						displayObj = meleeMonsterIcon;
					} else if (ASCII == 'm') {
						displayObj = miniatureIcon;
					} else if (ASCII == 'o') {
						displayObj = ogreIcon;
					} else if (ASCII == 'p') {
						displayObj = portalIcon;
						Deactivate (button11);
					} else if (ASCII == 'P') {
						displayObj = potionIcon;
					} else if (ASCII == 'R') {
						displayObj = rangedMonsterIcon;
					} else if (ASCII == 't') {
						displayObj = trapIcon;
					} else if (ASCII == 'X') {
						displayObj = wallIcon;
						ArrangeWall (parent, row , col,"addWall");
					}

					//instantiate gameobject in that position
					PlaceElement (displayObj, parent,row,col);
				}
			}
		}
	}

	// find the portal which is paired with the selected one and delete it
	void DeletePortal(GameObject obj){
		GameObject pairedPortal;
		//get the parent of the selected object
		GameObject parentObj = obj.transform.parent.gameObject;
		// find the portals 
		GameObject[] portals = GameObject.FindGameObjectsWithTag ("Portal");

		//check if the portal has same parent as the selected one has,if not remove it
		if (portals[0].transform.parent.gameObject != parentObj) {
			pairedPortal = portals [0];
		}
		else {
			pairedPortal = portals [1];
		}

		//get the selected object name
		string grid = pairedPortal.transform.parent.gameObject.name;

		// get the position from the name (names are in "Button(row,column)" format)
		grid = grid.Replace ("Button", "").Replace ("(", "").Replace (")", "");
		string[] pos = grid.Split (new char[]{ ',' }, 2);

		// change the row and column no to integer
		int row = System.Int32.Parse (pos [0]);
		int column = System.Int32.Parse (pos [1]);

		Destroy (pairedPortal);
		// remove the portal from the ASCII file
		_fileEditor.ChangeLine (file, file, row, column, ' ');
	}
}