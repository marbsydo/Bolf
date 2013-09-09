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

			// Get the current swipe
			Vector2[] swipe = swipeSensor.GetLongestStraightishSwipe();
			bool lineExists = (swipe.Length >= 2); // Must be at least 2 points for a line to exist in the swipe
			Vector2 lineStart = Vector2.zero;
			Vector2 lineVector = Vector2.zero;
			float lineTime = 1f; //TODO: Calculate time it took to draw the line
			if (lineExists) {
				lineStart = swipe[swipe.Length - 1];
				lineVector = swipe[0] - lineStart;
			}

			// Draw debug swipe (black = raw; white = inferred line)
			if (Debug.isDebugBuild) {
				if (lineExists) {
					for (int i = 0; i < swipe.Length - 1; i++) {
						Debug.DrawLine((Vector3) swipe[i], (Vector3) swipe[i + 1], Color.black);
					}
					Debug.DrawLine((Vector3) lineStart, (Vector3) (lineStart + lineVector), Color.white);
				}
			}

			Vector3 mPos, mPosWorld; // mPos = mouse coords in screen space; mPosWorld = mouse coords in world space

			mPos = Input.mousePosition;
			mPosWorld = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -Camera.main.transform.position.z));

			if (Input.GetMouseButtonDown(0)) {
				ResetBall();
			}

			if (Input.GetMouseButton(0)) {
				swipeSensor.AddPoint((Vector2) mPosWorld);
			}

			if (Input.GetMouseButtonUp(0)) {
				if (lineExists) {
					if (((Vector2) ball.transform.position - lineStart).magnitude < 2f) {
						Vector3 forceVector = ((Vector3) lineVector.normalized) * (lineVector.magnitude / lineTime);
						ball.rigidbody.AddForce(forceVector, ForceMode.Impulse);
					}
				}
				swipeSensor.ResetPoints();
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

	// if timeLimited is true, then
	//   swipeHistoryLengthMax * swipeHistoryTimeInterval = how many seconds of history are recorded

	private bool timeLimited = false;
	private const int swipeHistoryLengthMax = 40;
	private int swipeHistoryLength;
	private const int minAllowedPoints = 4;
	private Vector2[] swipeHistory = new Vector2[swipeHistoryLengthMax];
	private float swipeHistoryTimeInterval = 0.05f;
	private float swipeHistoryTimeLast = -999;
	private float swipeHistoryPosInterval = 0.5f;
	private Vector2 swipeHistoryPosLast = Vector2.one * -999;

	// This function should be called constantly to inform SwipeSensor of the current swipe position
	public void AddPoint(Vector2 point) {
		bool sufficientTimeInterval = Time.timeSinceLevelLoad > swipeHistoryTimeLast + swipeHistoryTimeInterval;
		bool sufficientPosInterval = (swipeHistoryPosLast - point).magnitude > swipeHistoryPosInterval;

		if ((timeLimited && sufficientTimeInterval) || (!timeLimited && sufficientPosInterval)) {
			swipeHistoryTimeLast = Time.timeSinceLevelLoad;
			swipeHistoryPosLast = point;

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
		for (int i = swipeHistoryLength; i >= minAllowedPoints; i--) {
			ps = FirstXPoints(i);
			if (ArePointsStraightish(ps, 0.4f)) {
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

	// This function checks whether a given series of Vector2 points resemble a straight line
	// First, the function ConvertPointsToNormals converts the series of points to where is
	// referred to as normals. Essentially, this is the normalized difference between adjacent
	// vector. As a result, all values will have a magnitude of 1 (with the exception of vectors
	// that were equal to Vector2.zero will have a magnitude of 0). Secondly, the function
	// AreNormalsSimilar will check whether all these normals are similar, i.e. the mean is taken,
	// and true is returned if all normals are within the given threshold of the mean. Logically
	// this means that all the vectors are pointing in the same direction, so the original line
	// must have been straight-ish!
	public bool ArePointsStraightish(Vector2[] ps, float threshold) {
		return AreNormalsSimilar(ConvertPointsToNormals(ps), threshold, true);
	}

	public Vector2[] ConvertPointsToNormals(Vector2[] ps) {

		// Subtracts the first element from all other elements in the array, then normalizses the result
		// The returned array is 1 element shorter than the inputted array
		Vector2[] ns = new Vector2[Mathf.Max(0, ps.Length - 1)];
		for (int i = 1; i < ns.Length; i++) {
			ns[i] = (ps[i] - ps[i - 1]).normalized;
		}

		return ns;
	}

	public bool AreNormalsSimilar(Vector2[] ns, float threshold, bool ignoreZeroMagnitudes) {

		// ns = Array of Vector2 normals to be compared
		// threshold = If any value in ns deviates from the mean by amount `threshold` or more, false is returned
		// ignoreZeroMagnitudes = If true, any value in ns equal to Vector2.zero will not cause false to be returned

		// Calculate mean
		Vector2 nsMean = Vector2.zero;
		for (int i = 0; i < ns.Length; i++) {
			nsMean += ns[i];
		}
		nsMean /= ns.Length;
		nsMean.Normalize();

		// Compare all points to the mean
		bool similar = true;
		for (int i = 0; i < ns.Length; i++) {
			if ((ns[i] - nsMean).magnitude > threshold && (ns[i] != Vector2.zero ^ !ignoreZeroMagnitudes)) {
				similar = false;
				break;
			}
		}

		return similar;
	}
}