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

	bool hitThisFrame = false;

	void Awake() {
		cam = (GameObject.Find("MenuCamera") as GameObject).GetComponent<Camera>() as Camera;
		menuCamera = cam.gameObject.GetComponent<MenuCamera>();

		menuLevelsController = (GameObject.Find("MenuLevelsController") as GameObject).GetComponent<MenuLevelsController>();
	}

	public void SetLevelNumber(int levelNumber) {
		if (buttonAction == MenuButtonAction.ActionLevel) {
			this.levelNumber = levelNumber;
		} else {
			Debug.LogWarning("Tried to assign a levelNumber to a button that has nothing to do with levelNumber");
		}
	}

	void Update() {

		bool wasHitThisFrame = WasHitThisFrame();

		if (Input.GetMouseButtonDown(0)) {
			if (wasHitThisFrame) {
				// Player has pressed down on the button
				pressed = true;
			}
		}

		if (pressed) {
			if (!wasHitThisFrame) {
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

	/*
	bool TouchOverButton() {
		return hitThisFrame;
	}
	*/

	/*
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
	*/
}
