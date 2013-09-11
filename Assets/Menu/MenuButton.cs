using UnityEngine;
using System.Collections;

public enum MenuButtonAction {MenuMain, MenuWorlds, MenuLevels, MenuSettings, WorldLawn, WorldPinball, WorldSky, WorldSpace, ActionPlay, __Length}

public class MenuButton : MonoBehaviour {

	public MenuButtonAction buttonAction = MenuButtonAction.MenuMain;

	Camera cam;
	MenuCamera menuCamera;
	bool pressed;

	void Awake() {
		cam = (GameObject.Find("MenuCamera") as GameObject).GetComponent<Camera>() as Camera;
		menuCamera = cam.gameObject.GetComponent<MenuCamera>();
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			if (TouchOverButton()) {
				// Player has pressed down on the button
				pressed = true;
			}
		}

		if (pressed) {
			if (!TouchOverButton()) {
				// Touch has moved off button
				pressed = false;
			}
		}

		if (Input.GetMouseButtonUp(0)) {
			if (pressed) {
				pressed = false;
				// Touch was released while over the button so perform the action
				Action();
			}
		}

		// Update the sprite
		if (pressed) {
			transform.localScale = Vector3.one * 1.1f;
		} else {
			transform.localScale = Vector3.one;
		}
	}

	void Action() {
		Debug.Log("Doing action for " + buttonAction);
		// Perform the button action
		switch (buttonAction) {
		case MenuButtonAction.MenuWorlds:
			menuCamera.SetMenuScreen(MenuScreen.Worlds);
			break;
		case MenuButtonAction.MenuMain:
			menuCamera.SetMenuScreen(MenuScreen.Main);
			break;
		case MenuButtonAction.MenuLevels:
			menuCamera.SetMenuScreen(MenuScreen.Levels);
			break;
		case MenuButtonAction.MenuSettings:
			//TODO: Settings page does not exist yet
			//menuCamera.SetMenuScreen(MenuScreen.Settings);
			break;
		}
	}

	// TODO: this function is awful because every single button in the scene calls it!!!
	// It should ideally just be called once and a message passed on to the relevant button that is has been clicked
	bool TouchOverButton() {
		// Raycast and see if the mouse is over the button
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction * 20, Color.yellow);

		bool touchOverButton = false;

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			if (hit.collider != null) {
				// really terrible code
				touchOverButton = (hit.collider.name == gameObject.name);
			}
		}

		return touchOverButton;
	}
}
