using UnityEngine;
using System.Collections;

public enum MenuScreen {Main, Worlds, Levels, __Length}

public class MenuCamera : MonoBehaviour {
	MenuScreen menuScreen = MenuScreen.Main;

	void Update() {
		int targetRotation;
		switch (menuScreen) {
		case MenuScreen.Main:
			targetRotation = 0;
			break;
		case MenuScreen.Worlds:
			targetRotation = 90;
			break;
		case MenuScreen.Levels:
			targetRotation = 190;
			break;
		default:
			Debug.LogWarning("Unknown menu screen" + menuScreen);
			goto case MenuScreen.Main;
		}

		Vector3 temp = transform.eulerAngles;
		temp.y = Mathf.Lerp(temp.y, targetRotation, Time.deltaTime * 7f);
		transform.eulerAngles = temp;

		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			menuScreen--;
			if ((int) menuScreen < 0)
				menuScreen = (MenuScreen) 0;
		}
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			menuScreen++;
			if ((int) menuScreen >= (int) MenuScreen.__Length)
				menuScreen = (MenuScreen) MenuScreen.__Length - 1;
		}
	}
}
