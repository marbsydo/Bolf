using UnityEngine;
using System.Collections;

public enum ButtonAction {Menu, Retry}

public class GUIButton : MonoBehaviour {

	public ButtonAction buttonAction = ButtonAction.Menu;

	float buttonRadius = 1f;
	bool pressed = false;

	Camera cam;

	void Start() {
		cam = ((GameObject.Find("GUICamera") as GameObject).GetComponent<Camera>() as Camera);
	}

	void Update() {

		Vector3 mPos, mPosWorld; // mPos = mouse coords in screen space; mPosWorld = mouse coords in world space

		mPos = Input.mousePosition;
		mPosWorld = cam.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -cam.transform.position.z));

		if (Input.GetMouseButtonDown(0)) {
			if (PosNearButton(mPosWorld, buttonRadius)) {
				pressed = true;
			}
		}

		if (Input.GetMouseButtonUp(0)) {
			if (pressed) {
				// Do action
				switch (buttonAction) {
				case ButtonAction.Menu:
					// Do menu stuff
					//TODO: This should bring up a pause menu, but for now it can just go to the menu
					Application.LoadLevel("menu");
					break;
				case ButtonAction.Retry:
					// Retry level
					//((GameObject.Find("GameControls") as GameObject).GetComponent<GameControls>() as GameControls).ResetAll();
					Application.LoadLevel(Application.loadedLevelName);
					break;
				}
				pressed = false;
			}
		}

		if (pressed) {
			if (!PosNearButton(mPosWorld, buttonRadius)) {
				pressed = false;
			}
		}

		if (pressed) {
			transform.localScale = Vector3.one * 1.1f;
		} else {
			transform.localScale = Vector3.one;
		}
	}

	bool PosNearButton(Vector3 p, float d) {
		//Debug.DrawLine((Vector2) transform.position, (Vector2) p);
		return ((((Vector2) transform.position) - ((Vector2) p)).magnitude < d);
	}
}
