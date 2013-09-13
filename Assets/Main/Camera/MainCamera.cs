using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	public float desiredRoomWidth = 14f;

	float roomTop;
	float roomBottom;

	float cameraY = 0;
	float cameraZ = -10f;

	GameObject target;

	void Awake() {
		roomTop = (GameObject.Find("WallTop") as GameObject).transform.position.y;
		roomBottom = (GameObject.Find("WallBottom") as GameObject).transform.position.y;
	}

	void Update() {
		camera.orthographicSize = (desiredRoomWidth / 2) / Screen.width * Screen.height;

		target = GameObject.Find("Ball") as GameObject;

		if (target != null) {
			cameraY = target.transform.position.y;
		} else {
			cameraY = 0f;
		}

		cameraY = Mathf.Max(cameraY, roomBottom + camera.orthographicSize + 0.5f);
		cameraY = Mathf.Min(cameraY, roomTop - camera.orthographicSize - 0.5f);

		camera.transform.position = new Vector3(0, cameraY, cameraZ);
	}
}
