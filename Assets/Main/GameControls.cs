using UnityEngine;
using System.Collections;

public class GameControls : MonoBehaviour {
	Object ballPrefab;
	Ball ball;

	void Awake() {
		ballPrefab = Resources.Load("Ball");
	}

	void Start() {
		ball = (Instantiate(ballPrefab, Vector3.zero, Quaternion.identity) as GameObject).AddComponent<Ball>();
	}

/*
	Ball ball;

	void Start() {
		ball = (GameObject.Find("Ball") as GameObject).GetComponent<Ball>();
	}

	void Update() {
		for (int i = 0; i < Input.touchCount; i++) {
			if (Input.GetTouch(i).phase == TouchPhase.Began) {
				// Apply force to ball whenever user touches
				BallRoll(Input.GetTouch(i).position);
			}
		}

		if (Input.GetMouseButtonDown(0)) {
			BallRoll(Input.mousePosition);
		}
	}

	void BallRoll(Vector3 touch) {
		Vector2 touchPosWorld = Camera.main.ScreenToWorldPoint(new Vector3(touch.x, touch.y, -Camera.main.transform.position.z));

		Vector2 diff = touchPosWorld - (Vector2)ball.transform.position;
		ball.Roll(diff);
	}
*/
}
