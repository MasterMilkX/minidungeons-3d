using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearPanelScript : MonoBehaviour {

	public void Disappear(GameObject me)
	{
		me.SetActive(false);
	}

	public void Appear(GameObject me)
	{
		me.SetActive(true);
	}
}
