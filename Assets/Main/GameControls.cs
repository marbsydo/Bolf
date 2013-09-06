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

		float[] t = {0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 100f};
		Debug.Log(swipeSensor.CalculateVariance(t));
		Debug.Log(swipeSensor.PolarToCartesian(22.6f * Mathf.Deg2Rad) * 13);
	}

	void Update() {
		if (ballStage == BallStage.AwaitingRelease) {

			Vector3 mPos, mPosWorld; // mPos = mouse coords in screen space; mPosWorld = mouse coords in world space

			mPos = Input.mousePosition;
			mPosWorld = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -Camera.main.transform.position.z));

			swipeSensor.AddPoint((Vector2) mPosWorld);

			Vector2[] swipe = swipeSensor.GetLastStraightSwipe(50);
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
1b) Make the script able to tell for how many X seconds the mouse/touch input has not changed direction by more than e.g. 90 degrees
2) Make the ball following the mouse/touch like normal in the Input.GetMouseButton(0) code below
3) If the finger crosses the line, take the speed and direction from the last "straight line" as decided in 1b) and fire the ball accordingly
*/

public class SwipeSensor {

	private const int swipeHistoryLengthMax = 100;
	private int swipeHistoryLength;
	private Vector2[] swipeHistory = new Vector2[swipeHistoryLengthMax];
	private float swipeHistoryTimeInterval = 0.1f;
	private float swipeHistoryTimeLast;

	// This function should be called constantly to inform SwipeSensor of the current swipe position
	public void AddPoint(Vector2 point) {
		// Only add another point if there has been a sufficient time interval
		if (swipeHistoryTimeLast == null || Time.timeSinceLevelLoad > swipeHistoryTimeLast + swipeHistoryTimeInterval) {
			swipeHistoryTimeLast = Time.timeSinceLevelLoad;

			//for (int i = 0; i < swipeHistoryLengthMax - 1; i++) {
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

	public Vector2[] GetSwipeHistory(int numPoints) {
		int numberOfPointsRequested = Mathf.Min(swipeHistoryLength, numPoints);

		Vector2[] requestedHistory = new Vector2[numberOfPointsRequested];
		for (int i = 0; i < numberOfPointsRequested; i++) {
			requestedHistory[i] = swipeHistory[i];
		}

		return requestedHistory;
	}

	public int CalculateNumPointsOfLastStraightSwipe(float tolerance) {
		tolerance = Mathf.Abs(tolerance);

		Vector2 swipeSum = Vector2.zero;
		Vector2 swipeMean;

		int numPoints = 0;
		int currentLength;
		Vector2 swipeDiffSquaredFromMeanTotal;

		for (int i = 0; i < swipeHistoryLength; i++) {
			currentLength = (i + 1);
			swipeSum += swipeHistory[i];
			swipeMean = swipeSum / currentLength;

			// Calculate the variance of the last i number of points
			swipeDiffSquaredFromMeanTotal = Vector2.zero;
			for (int j = 0; j < i; j++) {
				swipeDiffSquaredFromMeanTotal += Sqr(swipeHistory[j] - swipeMean);
			}
			Vector2 variance = swipeDiffSquaredFromMeanTotal / currentLength;

			if (variance.x > tolerance || variance.y > tolerance) {
				// We have found where the points get too variant, so the ideal result is the previous i
				numPoints = Mathf.Max(0, i - 1);
				break;
			}
		}

		return numPoints;
	}

	public bool AreLastXPointsStraight(int numPoints, float tolerance) {
		return false; // TODO
	}

	// Calculates the angle between p0 & p1, p1 & p2, p2 & p3, etc...
	public float[] CalculateAnglesBetweenLastXPoints(int numPoints) {
		float[] f = {0f};
		return f; //TODO
	}

	public float CalculateMeanfOfAngles(float[] angles) {
		return 0f; //TODO
	}

	public Vector2 PolarToCartesian(float p) {
		Vector2 c;
		c.x = Mathf.Cos(p);
		c.y = Mathf.Sin(p);
		return c;
	}

	public float CalculateAngleBetweenPoints(Vector2 a, Vector2 b) {
		return Mathf.Atan2(b.y - a.y, b.x - a.x);
	}

	// Calculates the variance of an array of floats
	public float CalculateVariance(float[] f) {
		float fSum = 0;
		for (int i = 0; i < f.Length; i++) {
			fSum += f[i];
		}
		float fMean = fSum / f.Length;

		float preVariance = 0;
		for (int i = 0; i < f.Length; i++) {
			preVariance += (f[i] - fMean) * (f[i] - fMean);
		}
		return preVariance / f.Length;
	}

	private Vector2 Sqr(Vector2 a) {
		return new Vector2(a.x * a.x, a.y * a.y);
	}

	public Vector2[] GetLastStraightSwipe(float tolerance) {
		return GetSwipeHistory(CalculateNumPointsOfLastStraightSwipe(tolerance));
	}
}