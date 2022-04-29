using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

	public Dropdown _ddWall, _ddChest,_ddPotion,_ddTrap,_ddOgre,_ddMiniature,_ddRangedMonster,_ddMeleeMonster,_ddBlob;

	private int wall,chest,potion,trap,ogre,minitaur,rangedMonster,meleeMonster,blob;
	FileEditor _fileEditor;
	DisplayManager_d _displayManager;
	void Start(){
		//add action listener for the dropdowns
		_ddWall.onValueChanged.AddListener(delegate{SetInputValue(_ddWall);});
		_ddChest.onValueChanged.AddListener(delegate{SetInputValue(_ddChest);});
		_ddPotion.onValueChanged.AddListener(delegate{SetInputValue(_ddPotion);});
		_ddTrap.onValueChanged.AddListener(delegate{SetInputValue(_ddTrap);});
		_ddOgre.onValueChanged.AddListener(delegate{SetInputValue(_ddOgre);});
		_ddMiniature.onValueChanged.AddListener(delegate{SetInputValue(_ddMiniature);});
		_ddRangedMonster.onValueChanged.AddListener(delegate{SetInputValue(_ddRangedMonster);});
		_ddMeleeMonster.onValueChanged.AddListener(delegate{SetInputValue(_ddMeleeMonster);});
		_ddBlob.onValueChanged.AddListener(delegate{SetInputValue(_ddBlob);});

		_fileEditor = GetComponent<FileEditor>();
		_displayManager = GameObject.FindWithTag("GameController").GetComponent<DisplayManager_d>();
	}

	// set value of input variables from respective dropdown
	 void SetInputValue(Dropdown dp){
		// substract 1 from dropdown value as the first option of the dropdown is for selecting random nunber.
		//After substraction if the value is >= 0 take the number as input if the value is -1 consider it as selection of random number.

		// if the dropdown is for number of walls
		if(dp == _ddWall){
			if (dp.value == 0) {
				wall = dp.value;
			} else {
				wall = dp.value + 55;
			}
		}// if the dropdown is for number of chests
		else if(dp == _ddChest){
			chest = dp.value;
		}// if the dropdown is for number of potions
		else if(dp == _ddPotion){
			potion = dp.value;
		}// if the dropdown is for number of traps
		else if(dp == _ddTrap){
			trap = dp.value;
		}// if the dropdown is for number of ogres
		else if(dp == _ddOgre){
			ogre = dp.value;
		}// if the dropdown is for number of miniatures
		else if(dp == _ddMiniature){
			minitaur = dp.value;
		}// if the dropdown is for number of ranged monsters
		else if(dp == _ddRangedMonster){
			rangedMonster = dp.value;
		}// if the dropdown is for number of melee monsters
		else if(dp == _ddMeleeMonster){
			meleeMonster = dp.value;
		}// if the dropdown is for number of blobs
		else if(dp == _ddBlob){
			blob = dp.value;
		}
	}
	//listener for generate btn
	public void OnGen(){
		// substract 1 from dropdown value as the first option of the dropdown is for selecting random nunber.
		//After substraction if the value is >= 0 take the number as input if the value is -1 consider it as selection of random number.
		wall -= 1;
		chest -= 1;
		potion -= 1;
		trap -= 1;
		ogre -= 1;
		minitaur -= 1;
		rangedMonster -= 1;
		meleeMonster -= 1;
		blob -= 1;

		// Digger Creator
		DiggerGenerator dg = new DiggerGenerator();
		dg.GenerateMap(wall);

		// Agent Furnisher
		AgentFurnisher f = new AgentFurnisher();
		f.GenerateMap(dg.GetMap(), minitaur, meleeMonster, rangedMonster, ogre, blob, chest, potion, trap);


		_fileEditor.ChangeLine(Application.persistentDataPath + "/"+ DisplayManager_d.currentLevel + ".txt",
							   f.GetMap());
		// reset input variables
		ResetInput ();
		// refresh the map underneath
		_displayManager.edit = true;
		_displayManager.ShowScene(_displayManager._editorScn);
		_displayManager._inputPanel.SetActive(false);
		//reset all test data
		Variables.ResetVariables ();
		_displayManager.SetPlayTestData ();
		_displayManager.SetAutoTestData (null);
	}

	// reset input variables
	public void ResetInput(){
		//reset all dropdowns
		_ddWall.value = 0;
		_ddChest.value = 0;
		_ddPotion.value = 0;
		_ddTrap.value = 0;
		_ddOgre.value = 0;
		_ddMiniature.value = 0;
		_ddRangedMonster.value = 0;
		_ddMeleeMonster.value = 0;
		_ddBlob.value = 0;
		//reset all variables
		wall = 0;
		chest = 0;
		potion = 0;
		trap = 0;
		ogre = 0;
		minitaur = 0;
		rangedMonster = 0;
		meleeMonster = 0;
		blob = 0;
	}
}
