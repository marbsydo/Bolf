using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	public float desiredRoomWidth = 14f;

	void Update() {
		camera.orthographicSize = (desiredRoomWidth / 2) / Screen.width * Screen.height;
	}
}
