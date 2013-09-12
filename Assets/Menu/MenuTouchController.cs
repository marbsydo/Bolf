using UnityEngine;
using System.Collections;

public class MenuTouchController : MonoBehaviour {

	Camera cam;

	void Awake() {
		cam = (GameObject.Find("MenuCamera") as GameObject).GetComponent<Camera>() as Camera;
	}

	void Update() {
		PerformTouchRaycast();
	}

	void PerformTouchRaycast() {
		// Raycast and see which collider is hit
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction * 20, Color.yellow);

		bool touchOverButton = false;

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			if (hit.collider != null) {
				// A collider is being hit
				TellColliderTheyAreHit(hit.collider);
			}
		}
	}

	void TellColliderTheyAreHit(Collider collider) {
		MenuButton menuButton = collider.gameObject.GetComponent<MenuButton>() as MenuButton;
		if (menuButton != null) {
			menuButton.HitThisFrame();
		}
	}
}
