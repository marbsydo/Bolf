using UnityEngine;
using System.Collections;

public class MenuTouchController : MonoBehaviour {

	Camera cam;

	bool touchExists;
	Collider touchCollider;

	void Awake() {
		cam = (GameObject.Find("MenuCamera") as GameObject).GetComponent<Camera>() as Camera;
	}

	void Update() {
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) {
			PerformTouchRaycast();
		}
	}

	void PerformTouchRaycast() {

		touchExists = false;

		// Raycast and see which collider is hit
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction * 20, Color.yellow);

		bool touchOverButton = false;

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			if (hit.collider != null) {
				touchExists = true;
				touchCollider = hit.collider;
			}
		}
	}

	public bool TouchExists() {
		return touchExists;
	}

	public Collider TouchCollider() {
		return touchCollider;
	}
}
