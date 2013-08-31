using UnityEngine;
using System.Collections;

enum BallStage {DoesNotExist, AwaitingRelease, InMotion};

public class GameControls : MonoBehaviour {
	Object ballPrefab;
	Ball ball;
	BallStage ballStage = BallStage.DoesNotExist;

	Vector3[] markers;
	Vector3 markersMid;

	void Awake() {
		ballPrefab = Resources.Load("Ball");

		// Find the two markers
		markers = new Vector3[2];
		markers[0] = GameObject.Find("BallMarkerLeft").transform.position;
		markers[1] = GameObject.Find("BallMarkerRight").transform.position;
		markersMid = markers[0] + (markers[1] - markers[0]) / 2;
	}

	void Start() {
		ResetBall();
	}

	void Update() {
		if (ballStage == BallStage.AwaitingRelease) {
			// Move ball left/right with touch
			if (Input.GetMouseButton(0)) {
				Vector3 t = ball.transform.position;
				t.x = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z)).x;
				if (t.x < markers[0].x)
					t.x = markers[0].x;
				if (t.x > markers[1].x)
					t.x = markers[1].x;
				ball.transform.position = t;
			}
		}
	}

	void ResetBall() {
		// Delete existing ball
		DestroyBall();

		// Create the new ball
		CreateBall(markersMid);
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
