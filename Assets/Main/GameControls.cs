using UnityEngine;
using System.Collections;

enum BallStage {DoesNotExist, AwaitingRelease, InMotion};
enum ReleaseStage {NoInput, MovingBallSideways, SwipingTowardsBall, WaitingToSwipeAway, SwipingAwayFromBall, Released}

public class GameControls : MonoBehaviour {
	Object ballPrefab;
	Ball ball;
	BallStage ballStage = BallStage.DoesNotExist;
	ReleaseStage releaseStage = ReleaseStage.NoInput;

	Vector3[] markers;
	Vector3 markersMid;

	float swipeTimeStart;
	Vector3 swipeFrom;
	Vector3 swipeTo;

	const float distTouchBall = 2f;

	void Awake() {
		ballPrefab = Resources.Load("Ball");

		// Find the two markers
		markers = new Vector3[2];
		markers[0] = GameObject.Find("BallMarkerLeft").transform.position;
		markers[1] = GameObject.Find("BallMarkerRight").transform.position;
		markersMid = markers[0] + (markers[1] - markers[0]) / 2;
		//markersLine = markersMid.y + 1.5f;
	}

	void Start() {
		ResetBall();
	}

	void Update() {
		if (ballStage == BallStage.AwaitingRelease) {

			Vector3 m, ms;

			if (Input.GetMouseButtonDown(0)) {
				m = Input.mousePosition;
				ms = Camera.main.ScreenToWorldPoint(new Vector3(m.x, m.y, -Camera.main.transform.position.z));

				if (releaseStage == ReleaseStage.Released) {
					// DEBUGGING: reset ball when touch after released
					ResetBall();
					releaseStage = ReleaseStage.NoInput;
				}
				if (releaseStage == ReleaseStage.NoInput) {
					if ((ms - ball.transform.position).magnitude < distTouchBall) {
						releaseStage = ReleaseStage.MovingBallSideways;
					}
				}
			}

			if (Input.GetMouseButton(0)) {
				m = Input.mousePosition;
				ms = Camera.main.ScreenToWorldPoint(new Vector3(m.x, m.y, -Camera.main.transform.position.z));

				if (releaseStage == ReleaseStage.MovingBallSideways) {
					Vector3 t = ball.transform.position;
					t.x = ms.x;
					if (t.x < markers[0].x)
						t.x = markers[0].x;
					if (t.x > markers[1].x)
						t.x = markers[1].x;
					ball.transform.position = t;
				}

				if (releaseStage == ReleaseStage.NoInput) {
					// Begin swiping towards ball
					releaseStage = ReleaseStage.SwipingTowardsBall;
					swipeFrom = ms;
				} else if (releaseStage == ReleaseStage.SwipingTowardsBall) {
					if (ms.y < ball.transform.position.y + 0.5f) {

						// Power is based upon time it takes to swipe from the ball back out
						
						releaseStage = ReleaseStage.WaitingToSwipeAway;
					}
				} else if (releaseStage == ReleaseStage.WaitingToSwipeAway) {
					if (ms.y > ball.transform.position.y + 1f) {
						swipeTimeStart = Time.timeSinceLevelLoad;
						releaseStage = ReleaseStage.SwipingAwayFromBall;
					}
				}
			}

			if (Input.GetMouseButtonUp(0)) {
				m = Input.mousePosition;
				ms = Camera.main.ScreenToWorldPoint(new Vector3(m.x, m.y, -Camera.main.transform.position.z));

				if (releaseStage == ReleaseStage.SwipingAwayFromBall) {
					if ((ms - ball.transform.position).magnitude < distTouchBall) {
						// Release too close to the ball, so do not allow the launch
						releaseStage = ReleaseStage.NoInput;
					} else {
						// Release ball
						swipeTo = ms;

						// aim towards the average of swipeFrom and swipeTo
						//TODO: Make less accurate if swipeFrom and swipeTo vary a lot
						Vector3 swipeMean = swipeTo - ball.transform.position;//((swipeFrom + swipeTo) / 2) - ball.transform.position;

						// Power of the ball is based upon the time it took the roll it
						// A faster roller (less time) equals a faster ball (more power)
						//const float maxPower = 2.5f;
						//float swipePower = maxPower / ((Time.timeSinceLevelLoad - swipeTimeStart) + 0.1f);
						//Vector3 swipeVector = swipeMean.normalized * swipePower;

						float swipeTime = Time.timeSinceLevelLoad - swipeTimeStart;
						float swipeDist = swipeMean.magnitude;
						float swipeSpeed = (swipeDist / swipeTime) / 25f;

						Vector3 swipeVector = swipeMean.normalized * swipeSpeed;

						ball.Roll((Vector2)swipeVector);
						//BallRoll(swipeVector);

						releaseStage = ReleaseStage.Released;
					}
				} else if (releaseStage == ReleaseStage.SwipingTowardsBall || releaseStage == ReleaseStage.MovingBallSideways || releaseStage == ReleaseStage.WaitingToSwipeAway) {
					releaseStage = ReleaseStage.NoInput;
				}
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
			ball = (Instantiate(ballPrefab, p, Quaternion.identity) as GameObject).GetComponent<Ball>();
			ballStage = BallStage.AwaitingRelease;
		} else {
			Debug.LogError("Tried to create a new ball when one already exists");
		}
	}

	void DestroyBall() {
		if (ball != null) {
			Destroy(ball.gameObject);
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
*/
/*
	void BallRoll(Vector3 pos) {
		ball.Roll((Vector2)pos - (Vector2)ball.transform.position);
	}
*/
}
