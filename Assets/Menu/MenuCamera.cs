using UnityEngine;
using System.Collections;

public enum MenuScreen {Main, Worlds, Levels, Settings, Black, __Length}

public enum BlackAction {None, PlayNextLevel, PlaySpecificLevel, Quit, __Length};

public class MenuCamera : MonoBehaviour {
	MenuScreen menuScreen = MenuScreen.Main;
	BlackAction blackAction = BlackAction.None;

	MenuLevelsController menuLevelsController;
	MainController mainController;

	SpecificLevel specificLevel;

	Color colorStart;
	Color colorBlack;
	Color colorWorlds;

	private const float blackAngle = 80; // The angle the camera rotates down in the X direction when it fades to black

	void Awake() {
		colorStart = camera.backgroundColor;
		colorBlack = Color.black;
		colorWorlds = new Color(0.46875f, 0f, 0.734375f);

		menuLevelsController = (GameObject.Find("MenuLevelsController") as GameObject).GetComponent<MenuLevelsController>();
		mainController = (GameObject.Find("MainController") as GameObject).GetComponent<MainController>();
	}

	void Update() {
		float targetRotationY;
		float targetRotationX;
		Color targetColor;
		switch (menuScreen) {
		case MenuScreen.Main:
			targetRotationY = 0;
			targetRotationX = 0;
			targetColor = colorStart;
			break;
		case MenuScreen.Worlds:
			targetRotationY = 90;
			targetRotationX = 0;
			targetColor = colorWorlds;
			break;
		case MenuScreen.Levels:
			targetRotationY = 180;
			targetRotationX = 0;
			//targetColor = colorStart;
			targetColor = menuLevelsController.GetDesiredBackgroundColor();
			break;
		case MenuScreen.Settings:
			targetRotationY = 270;
			targetRotationX = 0;
			targetColor = colorStart;
			break;
		case MenuScreen.Black:
			targetRotationY = 0;
			targetRotationX = blackAngle;
			targetColor = colorBlack;
			break;
		default:
			Debug.LogWarning("Unknown menu screen" + menuScreen);
			goto case MenuScreen.Main;
		}

		// Move camera
		Vector3 temp = transform.eulerAngles;
		temp.y = Mathf.Lerp(temp.y, targetRotationY, Time.deltaTime * 7f);
		temp.x = Mathf.Lerp(temp.x, targetRotationX, Time.deltaTime * 7f);
		transform.eulerAngles = temp;

		// Change background color
		camera.backgroundColor = Color.Lerp(camera.backgroundColor, targetColor, Time.deltaTime * 7f);

		// Perform black action if necessary
		if (menuScreen == MenuScreen.Black) {
			Debug.Log(transform.eulerAngles.x + "|" + blackAngle);
			//if (transform.eulerAngles.x == blackAngle) {
			if (IsApproximately(transform.eulerAngles.x, blackAngle, 0.1f)) {
				PerformBlackAction();
			}
		}

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

	bool IsApproximately(float a, float b, float tolerance) {
		return (Mathf.Abs(a - b) < tolerance);
	}

	void PerformBlackAction() {
		switch (blackAction) {
		case BlackAction.None:
			Debug.LogWarning("No action defined!");
			SetMenuScreen(MenuScreen.Main);
			break;
		case BlackAction.PlayNextLevel:
			// Play next level
			Application.LoadLevel("level_1");
			goto case BlackAction.None;
			break;
		case BlackAction.Quit:
			Debug.Log("Game has ended.");
			Application.Quit();
			break;
		case BlackAction.PlaySpecificLevel:
			// play the specific level
			mainController.PlayLevel(this.specificLevel);
			break;
		}
	}

	public void SetSpecificLevel(SpecificLevel specificLevel) {
		this.specificLevel = specificLevel;
	}

	public void SetMenuScreen(MenuScreen menuScreen) {
		this.menuScreen = menuScreen;
	}

	public void SetBlackAction(BlackAction blackAction) {
		this.blackAction = blackAction;
	}
}
