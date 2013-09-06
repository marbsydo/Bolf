using UnityEngine;
using System.Collections;

enum BallStage {DoesNotExist, AwaitingRelease, InMotion};
enum ReleaseStage {NoInput, Swiping, Released};

public class GameControls : MonoBehaviour {
	Object ballPrefab;
	Ball ball;
	BallStage ballStage = BallStage.DoesNotExist;

	ReleaseStage releaseStage = ReleaseStage.NoInput;

	Vector3[] markers;
	Vector3 markersMid;
	float markersLine;

	const float ballScalingModifier = 0.5f; // This is how much bigger the ball gets e.g. 1.5 = 150%
	const float ballSinModifier = 2f;

	SwipeSensor swipeSensor = new SwipeSensor();

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

			Vector3 mPos, mPosWorld; // mPos = mouse coords in screen space; mPosWorld = mouse coords in world space

			mPos = Input.mousePosition;
			mPosWorld = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -Camera.main.transform.position.z));

			swipeSensor.AddPoint((Vector2) mPosWorld);

			Vector2[] swipe = swipeSensor.GetLongestStraightishSwipe();
			//Debug.Log(swipe.Length + " " + Random.value);
			//Vector2[] swipe = swipeSensor.GetSwipeHistory(1f);
			if (swipe.Length >= 2) {
				for (int i = 0; i < swipe.Length - 2; i++) {
					Debug.DrawLine((Vector3) swipe[i], (Vector3) swipe[i + 1]);
				}
			}
		}
	}

	Vector3 CalculateBallScale() {
		return Vector3.one * (1 + Mathf.Sin(Mathf.Max(0f, markersLine - ball.transform.position.y) / ballSinModifier) * ballScalingModifier);
	}

	void ResetBall() {
		DestroyBall();
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
}

/*
1) Write a generic script to detect the speed and direction of mouse/touch input over the last X seconds
1b) Make the script able to tell for how many X seconds the mouse/touch input has not changed direction by more than e.g. 20 degrees
2) Make the ball following the mouse/touch like normal in the Input.GetMouseButton(0) code below
3) If the finger crosses the line, take the speed and direction from the last "straight line" as decided in 1b) and fire the ball accordingly
*/

public class SwipeSensor {

	// swipeHistoryLengthMax * swipeHistoryTimeInterval = how many seconds of history are recorded
	private const int swipeHistoryLengthMax = 40;
	private int swipeHistoryLength;
	private const int shortestAllowedSwipe = 5;
	private Vector2[] swipeHistory = new Vector2[swipeHistoryLengthMax];
	private float swipeHistoryTimeInterval = 0.05f;
	private float swipeHistoryTimeLast;

	// This function should be called constantly to inform SwipeSensor of the current swipe position
	public void AddPoint(Vector2 point) {
		// Only add another point if there has been a sufficient time interval
		if (swipeHistoryTimeLast == null || Time.timeSinceLevelLoad > swipeHistoryTimeLast + swipeHistoryTimeInterval) {
			swipeHistoryTimeLast = Time.timeSinceLevelLoad;

			for (int i = swipeHistoryLengthMax - 2; i >= 0; i--) {
				swipeHistory[i + 1] = swipeHistory[i];
			}
			swipeHistory[0] = point;
			swipeHistoryLength++;
			if (swipeHistoryLength > swipeHistoryLengthMax)
				swipeHistoryLength = swipeHistoryLengthMax;
		}
	}

	public int GetNumPoints() {
		return swipeHistoryLength;
	}

	public void ResetPoints() {
		swipeHistory = new Vector2[swipeHistoryLengthMax];
		swipeHistoryLength = 0;
	}

	public Vector2[] GetLongestStraightishSwipe() {
		Vector2[] ps = new Vector2[0];
		bool found = false;
		for (int i = swipeHistoryLength; i >= shortestAllowedSwipe; i--) {
			ps = FirstXPoints(i);
			if (ArePointsStraightish(ps, 0.2f)) {
				found = true;
				break;
			}
		}
		if (!found)
			ps = new Vector2[0];
		return ps;
	}

	public Vector2[] FirstXPoints(int numPoints) {

		// Returns the first X number of points from the swipeHistory
		Vector2[] xPoints = new Vector2[Mathf.Min(numPoints, swipeHistoryLength)];
		for (int i = 0; i < xPoints.Length; i++) {
			xPoints[i] = swipeHistory[i];
		}

		return xPoints;
	}

	public bool ArePointsStraightish(Vector2[] ps, float threshold) {
		return AreNormalsSimilar(ConvertPointsToNormals(ps), threshold);
	}

	public Vector2[] ConvertPointsToNormals(Vector2[] ps) {

		// Subtracts the first element from all other elements in the array, then normalizses the result
		// The returned array is 1 item shorter than the inputted array because the first element is dropped
		Vector2[] ns = new Vector2[Mathf.Max(0, ps.Length - 1)];
		for (int i = 1; i < ns.Length; i++) {
			ns[i] = (ps[i] - ps[0]).normalized;
		}

		return ns;
	}

	public bool AreNormalsSimilar(Vector2[] ns, float threshold) {

		// Calculate mean
		Vector2 nsMean = Vector2.zero;
		for (int i = 0; i < ns.Length; i++) {
			nsMean += ns[i];
		}
		nsMean /= ns.Length;
		nsMean.Normalize();

		// Compare all points to the mean
		bool similar = true;
		for (int i = 1; i < ns.Length; i++) {
			if ((ns[i] - nsMean).magnitude > threshold) {
				similar = false;
				break;
			}
		}

		return similar;
	}
}