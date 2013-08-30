using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {
	void Update() {
		float desiredRoomWidth = 11f;
		camera.orthographicSize = (desiredRoomWidth / 2) / Screen.width * Screen.height;
	}
}
