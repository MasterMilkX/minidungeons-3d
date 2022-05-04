using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour {

	public Toggle[] hearts;

	public void UpdateHealth(int health){
		if (health < 0 || health > MagicNumberManager.instance.GetValues().HeroMaxHealth) {
			Debug.LogWarning("HealthBar received hero health value that cannot be represented: " + health);
		}
		for (int i = 0; i < hearts.Length; i++) {
			hearts[i].isOn = false;
		}
		for (int j = 0; j < health; j++) {
			hearts[j].isOn = true;
		}
	}
}
