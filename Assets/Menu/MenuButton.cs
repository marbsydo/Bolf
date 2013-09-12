using UnityEngine;
using System.Collections;

public enum MenuButtonAction {MenuMain, MenuWorlds, MenuLevels, MenuSettings, MenuBlack, WorldLawn, WorldPinball, WorldSky, WorldSpace, ActionPlay, ActionQuit, ActionLevel, __Length}

public class MenuButton : MonoBehaviour {

	public MenuButtonAction buttonAction = MenuButtonAction.MenuMain;
	public int levelNumber;

	Camera cam;
	MenuCamera menuCamera;
	bool pressed;
	MenuLevelsController menuLevelsController;
	MenuTouchController menuTouchController;

	bool hitThisFrame = false;

	void Awake() {
		cam = (GameObject.Find("MenuCamera") as GameObject).GetComponent<Camera>() as Camera;
		menuCamera = cam.gameObject.GetComponent<MenuCamera>();

		menuLevelsController = (GameObject.Find("MenuLevelsController") as GameObject).GetComponent<MenuLevelsController>();
		menuTouchController = (GameObject.Find("MenuTouchController") as GameObject).GetComponent<MenuTouchController>();
	}

	public void SetLevelNumber(int levelNumber) {
		if (buttonAction == MenuButtonAction.ActionLevel) {
			this.levelNumber = levelNumber;
		} else {
			Debug.LogWarning("Tried to assign a levelNumber to a button that has nothing to do with levelNumber");
		}
	}

	void Update() {

		bool touchOverButton = TouchOverButton();

		if (Input.GetMouseButtonDown(0)) {
			if (touchOverButton) {
				// Player has pressed down on the button
				pressed = true;
			}
		}

		if (pressed) {
			if (!touchOverButton) {
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
		case MenuButtonAction.ActionPlay:
			menuCamera.SetMenuScreen(MenuScreen.Black);
			menuCamera.SetBlackAction(BlackAction.Play);
			break;
		case MenuButtonAction.ActionQuit:
			menuCamera.SetMenuScreen(MenuScreen.Black);
			menuCamera.SetBlackAction(BlackAction.Quit);
			break;
		case MenuButtonAction.WorldLawn:
			menuLevelsController.SetGameWorld(GameWorld.Lawn);
			menuCamera.SetMenuScreen(MenuScreen.Levels);
			break;
		case MenuButtonAction.WorldPinball:
			menuLevelsController.SetGameWorld(GameWorld.Pinball);
			menuCamera.SetMenuScreen(MenuScreen.Levels);
			break;
		case MenuButtonAction.WorldSky:
			menuLevelsController.SetGameWorld(GameWorld.Sky);
			menuCamera.SetMenuScreen(MenuScreen.Levels);
			break;
		case MenuButtonAction.WorldSpace:
			menuLevelsController.SetGameWorld(GameWorld.Space);
			menuCamera.SetMenuScreen(MenuScreen.Levels);
			break;
		}
	}

	public void HitThisFrame() {
		hitThisFrame = true;
	}

	void ResetHitThisFrame() {
		hitThisFrame = false;
	}

	bool WasHitThisFrame() {
		bool wasHit = hitThisFrame;
		hitThisFrame = false;
		return wasHit;
	}

	bool TouchOverButton() {
		bool touchOverButton = false;
		if (menuTouchController.TouchExists()) {
			Collider collider = menuTouchController.TouchCollider();
			// Am I the one being hit?
			if (collider.name == gameObject.name) {
				touchOverButton = true;
			}
		}
		return touchOverButton;
	}
}
