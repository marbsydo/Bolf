using UnityEngine;
using System.Collections;

enum BallStage {DoesNotExist, AwaitingRelease, InMotion};
//enum ReleaseStage {NoInput, MovingBallSideways, SwipingTowardsBall, WaitingToSwipeAway, SwipingAwayFromBall, Released}
enum ReleaseStage {NoInput, Swiping, Released};

public class GameControls : MonoBehaviour {
	Object ballPrefab;
	Ball ball;
	BallStage ballStage = BallStage.DoesNotExist;
	ReleaseStage releaseStage = ReleaseStage.NoInput;

	Vector3[] markers;
	Vector3 markersMid;
	float markersLine;

	Vector3 ballPositionAtFirstTouch;
	Vector3[] mPosWorldPrev = new Vector3[2];
	public bool swipingUpwards = false;
	public bool swipingUpwardsFast = false;
	const float swipingSpeedThreshold = 0.5f;

	float swipeTimeStart;
	Vector3 swipeFrom;
	Vector3 swipeTo;

	const float distTouchBall = 2f;

	// Constants used to modify the ball's pre-roll appearance
	const float distBallCanBeHeldBack = 1.5f;
	const float ballScalingModifier = 0.5f; // This is how much bigger the ball gets e.g. 1.5 = 150%
	const float ballSinModifier = 2f;

	void Awake() {
		ballPrefab = Resources.Load("Ball");

		// Find the two markers
		markers = new Vector3[2];
		markers[0] = GameObject.Find("BallMarkerLeft").transform.position;
		markers[1] = GameObject.Find("BallMarkerRight").transform.position;
		markersMid = markers[0] + (markers[1] - markers[0]) / 2;
		markersLine = markersMid.y + 1.62f;
	}

	void Start() {
		ResetBall();
	}

	void Update() {
		if (ballStage == BallStage.AwaitingRelease) {

			/*
			TODO: Rip out everything except the section in Input.GetMouseButton(0) which does the 3D stuff
			Instead, make it so that the ball it just below the line at the start.
			The user can then slide the ball left and right.
			When their X position is chosen, they swipe straight down and the ball locks into its X position while moving in the Y.
			The user can then swipe forward (or diagonally forward).

			//EDIT: Script the above. Do the following:
			1) Write a generic script to detect the speed and direction of mouse/touch input over the last X seconds
			1b) Make the script able to tell for how many X seconds the mouse/touch input has not changed direction by more than e.g. 90 degrees
			2) Make the ball following the mouse/touch like normal in the Input.GetMouseButton(0) code below
			3) If the finger crosses the line, take the speed and direction from the last "straight line" as decided in 1b) and fire the ball accordingly
			*/

			// Player touches beneath the line, then moves their finger
			// If the touch goes horizontal, move the ball to where the finger is
			// If the touch goes upwards, return the ball to the position it was first at when touched, and fire upon release

			Vector3 mPos, mPosWorld; // mPos = mouse coords in screen space; mPosWorld = mouse coords in world space

			mPos = Input.mousePosition;
			mPosWorld = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -Camera.main.transform.position.z));

			if (Input.GetMouseButtonDown(0)) {
				releaseStage = ReleaseStage.Swiping;

				// Set our history of the last two touches to mPosWorld
				mPosWorldPrev[0] = mPosWorldPrev[1] = mPosWorld;
			}

			if (releaseStage == ReleaseStage.Swiping) {
				if (Input.GetMouseButton(0)) {

					// Make a target variable for the ball's position
					Vector3 ballTargetPos = ball.transform.position;

					// Set the target X position, clamped between two points
					ballTargetPos.x = mPosWorld.x;
					ballTargetPos.x = Mathf.Max(markers[0].x + 0.3f, ballTargetPos.x);
					ballTargetPos.x = Mathf.Min(markers[1].x - 0.3f, ballTargetPos.x);

					// Set the target Y position
					float offsetY = Mathf.Max(0f, markersLine - mPosWorld.y);
					offsetY = Mathf.Sin(Mathf.Min(offsetY / ballSinModifier, Mathf.PI/2f)) * distBallCanBeHeldBack; // Use Min to limit below 2pi to avoid looping back upon itself
					////offsetY = Mathf.Min(offsetY, distBallCanBeHeldBack); // Same as above line, but linear instead of circular movement. Circular looks better.
					ballTargetPos.y = markersLine - offsetY;
					
					// Lerp the target position each axis individually so that lerping can be performed at different timescales
					Vector3 ballTargetPosLerped;
					ballTargetPosLerped.z = ballTargetPos.z; // Move instantly, because Z is always locked
					float yModifier = 5f;//40f;
					float xModifier = 8f;
					//if (ballTargetPos.y > ball.transform.position.y) {yModifier = 5f;}
					ballTargetPosLerped.y = Mathf.Lerp(ball.transform.position.y, ballTargetPos.y, Time.deltaTime * yModifier); // Move in y quite fast
					ballTargetPosLerped.x = Mathf.Lerp(ball.transform.position.x, ballTargetPos.x, Time.deltaTime * xModifier); // Move in x quite slow

					// Finally move the ball to its new target position, after lerping
					ball.transform.position = ballTargetPosLerped;

					// If the user is wiping upwards, actually just leave the ball where it was when the swipe began
					if (swipingUpwardsFast) {
						ball.transform.position = swipeFrom;
					}

					// Also update the ball's scale using Sin again to give the appearance of height
					ball.SetScale(CalculateBallScale());//Vector3.one * (1 + Mathf.Sin(Mathf.Max(0f, markersLine - ball.transform.position.y) / ballSinModifier) * ballScalingModifier);

					// If the mouse is moving up, when previously it was moving down, record this as the start of the swipe
					if (swipingUpwards == false && mPosWorld.y > mPosWorldPrev[0].y && mPosWorldPrev[0].y <= mPosWorldPrev[1].y && mPosWorld.y < markersLine - 1f) {
						Debug.Log("STARTED MOVING" + Random.value);
						swipingUpwards = true;
						swipeFrom = ball.transform.position;
					}

					// If swiping upwards, measure how fast the swipe is. If it is fast, enabled swipingUpwardsFast. This locks us into the swipe procedure.
					if (swipingUpwards) {
						if ((mPosWorld - mPosWorldPrev[1]).magnitude > swipingSpeedThreshold && (Mathf.Abs(mPosWorld.x - mPosWorldPrev[1].x) < 0.6f)) {
							swipingUpwardsFast = true;
						}
					}

					// If the mouse is moving down, when previously it was ALSO moving down, record this as resetting the swipe
					if (swipingUpwards == true && mPosWorld.y < mPosWorldPrev[0].y && mPosWorldPrev[0].y < mPosWorldPrev[1].y) {
						Debug.Log("RESETTING" + Random.value);
						swipingUpwards = false;
						swipingUpwardsFast = false;
					}

					// If the mouse has been in the same place for a while, record this as resetting the swipe

					if (mPosWorldPrev[0] != mPosWorldPrev[1])
						mPosWorldPrev[1] = mPosWorldPrev[0];
					if (mPosWorld != mPosWorldPrev[0])
						mPosWorldPrev[0] = mPosWorld;


				} else {
					releaseStage = ReleaseStage.NoInput;
				}
			}

			if (Input.GetMouseButtonUp(0)) {
				if (swipingUpwardsFast) {
					swipingUpwards = false;
					swipingUpwardsFast = false;
					// If released while moving fast, fire the ball!
					//if (mPosWorld.y > mPosWorldPrev[0].y && mPosWorldPrev[0].y > mPosWorldPrev[1].y) {
					if ((mPosWorld - mPosWorldPrev[1]).magnitude > swipingSpeedThreshold) {
						Debug.LogWarning("FIRING BALL!" + Random.value);
					} else { // Else if release while not moving, reset the ball
						Debug.Log("RESETTING_F" + Random.value);
					}
				}
			}

			if (releaseStage == ReleaseStage.NoInput) {
				Vector3 t = ball.transform.position;
				t.y = Mathf.Lerp(t.y, markersLine, Time.deltaTime * 5f);
				ball.transform.position = t;

				ball.SetScale(CalculateBallScale());//Vector3.one * (1 + Mathf.Sin(Mathf.Max(0f, markersLine - ball.transform.position.y) / ballSinModifier) * ballScalingModifier);
			}


			/*
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
						float swipeSpeed = (swipeDist / swipeTime) / 15f;

						Vector3 swipeVector = swipeMean.normalized * swipeSpeed;

						ball.Roll((Vector2)swipeVector);
						//BallRoll(swipeVector);

						releaseStage = ReleaseStage.Released;
					}
				} else if (releaseStage == ReleaseStage.SwipingTowardsBall || releaseStage == ReleaseStage.MovingBallSideways || releaseStage == ReleaseStage.WaitingToSwipeAway) {
					releaseStage = ReleaseStage.NoInput;
				}
			}
		*/
		}
	}

	Vector3 CalculateBallScale() {
		return Vector3.one * (1 + Mathf.Sin(Mathf.Max(0f, markersLine - ball.transform.position.y) / ballSinModifier) * ballScalingModifier);
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
			ball.name = "Ball";
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
