using UnityEngine;
using System.Collections;

enum BallStage {DoesNotExist, AwaitingRelease, InMotion};

public class GameControls : MonoBehaviour {
	Object ballPrefab;
	Ball ball;
	BallStage ballStage = BallStage.DoesNotExist;

	void Awake() {
		ballPrefab = Resources.Load("Ball");
	}

	void Start() {
		ResetBall();
	}

	void Update() {
		if (ballStage == BallStage.AwaitingRelease) {
			// Move ball left/right with touch
		}
	}

	void ResetBall() {
		// Delete existing ball
		DestroyBall();

		// Find the two markers
		Vector3[] markers = new Vector3[2];
		markers[0] = GameObject.Find("BallMarkerLeft").transform.position;
		markers[1] = GameObject.Find("BallMarkerRight").transform.position;
		Vector3 midPoint = markers[0] + (markers[1] - markers[0]) / 2;

		// Create the new ball
		CreateBall(midPoint);
	}

	void CreateBall(Vector3 p) {
		if (ballStage == BallStage.DoesNotExist) {
			ball = (Instantiate(ballPrefab, p, Quaternion.identity) as GameObject).AddComponent<Ball>();
			ballStage = BallStage.AwaitingRelease;
		} else {
			Debug.LogError("Tried to create a new ball when one already exists");
		}
	}

	void DestroyBall() {
		if (ball != null) {
			Destroy(ball.transform);
			ballStage = BallStage.DoesNotExist;
		}
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
