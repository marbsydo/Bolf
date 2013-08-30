using UnityEngine;
using System.Collections;

public class GameControls : MonoBehaviour {

	Ball ball;

	void Start() {
		ball = (GameObject.Find("Ball") as GameObject).GetComponent<Ball>();
	}

	void Update() {
		for (int i = 0; i < Input.touchCount; i++) {
			if (Input.GetTouch(i).phase == TouchPhase.Began) {
				// Apply force to ball whenever user touches
				Vector2 touchPos = Input.GetTouch(i).position;
				Vector2 touchPosWorld = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, -Camera.main.transform.position.z));

				Vector2 diff = touchPosWorld - (Vector2)ball.transform.position;
				ball.Roll(diff);
			}
		}
	}
}
