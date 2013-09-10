using UnityEngine;
using System.Collections;

enum BallStage {DoesNotExist, AwaitingRelease, InMotion};
enum ReleaseStage {Positioning, PulledBack, Released};

public class GameControls : MonoBehaviour {

	GUIScore guiScore;
		
	Object ballPrefab;
	Ball ball;
	BallStage ballStage = BallStage.DoesNotExist;

	ReleaseStage releaseStage = ReleaseStage.Positioning;

	SwipeSensor swipeSensor = new SwipeSensor();
	TouchInput touchInput;

	Vector3 ballStartPosition;

	void Awake() {
		touchInput = new TouchInput(GameObject.Find("GUICamera"));
		guiScore = (GameObject.Find("GUIScore") as GameObject).GetComponent<GUIScore>();

		ballPrefab = Resources.Load("Ball");

		GameObject temp = GameObject.Find("Ball") as GameObject;
		if (temp != null) {
			ballStartPosition = temp.transform.position;
		} else {
			Debug.LogWarning("No ball in scene");
			ballStartPosition = Vector3.zero;
		}
		Destroy(temp);
	}

	void Start() {
		ResetBall();
	}

	void Update() {
		if (ballStage == BallStage.AwaitingRelease) {

			// Get the current swipe
			Vector2TimeArray swipe = swipeSensor.GetLongestStraightishSwipe();
			bool lineExists = (swipe.GetLength() >= 2); // Must be at least 2 points for a line to exist in the swipe
			Vector2 lineStart = Vector2.zero;
			Vector2 lineVector = Vector2.zero;
			float lineTime = 0f;
			if (lineExists) {
				lineStart = swipe.GetPosition(swipe.GetLength() - 1);
				lineVector = swipe.GetPosition(0) - lineStart;
				lineTime = swipe.GetTime(swipe.GetLength() - 1) - swipe.GetTime(0);
			}

			// Draw debug swipe (black = raw; white = inferred line)
			if (Debug.isDebugBuild) {
				if (lineExists) {
					for (int i = 0; i < swipe.GetLength() - 1; i++) {
						Debug.DrawLine((Vector3) swipe.GetPosition(i), (Vector3) swipe.GetPosition(i + 1), Color.black);
					}
					Debug.DrawLine((Vector3) lineStart, (Vector3) (lineStart + lineVector), Color.white);
				}
			}

			Vector3 mPos, mPosWorld; // mPos = mouse coords in screen space; mPosWorld = mouse coords in world space

			mPos = Input.mousePosition;
			mPosWorld = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -Camera.main.transform.position.z));

/*
			// If the ball is in the hole, touch to reset
			if (touchInput.Down()) {
				if (ball.IsInHole()) {
					ResetAll();
				}
			}
*/

			// If touching, add point to swipeSensor
			if (touchInput.Stay()) {
				swipeSensor.AddPoint((Vector2) mPosWorld);
			}

			// If released touch, fire if necessary
			if (touchInput.Up()) {
				if (lineExists) {
					if (!ball.IsInHole()) {
						Vector3 forceVector = ((Vector3) lineVector.normalized) * (lineVector.magnitude / Mathf.Max(0.1f, lineTime));
						ball.rigidbody.AddForce(forceVector, ForceMode.Impulse);
						guiScore.IncStrokes();
					}
				}
				swipeSensor.ResetPoints();
			}
		}
	}

	public void ResetAll() {
		ResetBall();
		swipeSensor.ResetPoints();
		guiScore.ResetStrokes();
	}

	void ResetBall() {
		DestroyBall();
		CreateBall(ballStartPosition);
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

public class TouchInput {

	GameObject guiCamera;
	Vector2 halfWidth = Vector2.zero;

	// yes this is awful code and it should actually read the height from the object
	const float heightOfBlackBar = 2f;	// height of the black bar at the top of the GUI

	public TouchInput(GameObject guiCamera) {
		Init(guiCamera);
	}

	private void Init(GameObject guiCamera) {
		this.guiCamera = guiCamera;
		halfWidth.x = ((float) guiCamera.camera.orthographicSize / Screen.height) * Screen.width;
		halfWidth.y = ((float) guiCamera.camera.orthographicSize);
	}

	public bool TouchWithinArea() {
		Vector3 mPos, mPosWorld, mPosWGUI; // mPos = mouse coords in screen space; mPosWorld = mouse coords in world space, mPosWGUI = mouse coords in world space of the GUI camera

		mPos = Input.mousePosition;
		mPosWorld = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -Camera.main.transform.position.z));
		mPosWGUI = guiCamera.camera.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -guiCamera.transform.position.z));

		return (mPosWGUI.y < guiCamera.transform.position.y + halfWidth.y - heightOfBlackBar);
	}

	public bool Down() {
		return (TouchWithinArea() && Input.GetMouseButtonDown(0));
	}

	public bool Stay() {
		return (TouchWithinArea() && Input.GetMouseButton(0));
	}

	public bool Up() {
		return (TouchWithinArea() && Input.GetMouseButtonUp(0));
	}
}

public class SwipeSensor {
	// if timeLimited is true, then
	//   swipeHistoryLengthMax * swipeHistoryTimeInterval = how many seconds of history are recorded

	private bool timeLimited = false;
	private const int swipeHistoryLengthMax = 40;
	private int swipeHistoryLength;
	private const int minAllowedPoints = 4;
	private Vector2TimeArray swipeHistory = new Vector2TimeArray(swipeHistoryLengthMax);
	private float swipeHistoryTimeInterval = 0.05f;
	private float swipeHistoryTimeLast = -999;
	private float swipeHistoryPosInterval = 0.5f;
	private Vector2 swipeHistoryPosLast = Vector2.one * -999;
	private float straightLineSensitivity = 0.5f; // The greater the value, the more leniant we are upon the definition of a "straight" line

	// This function should be called constantly to inform SwipeSensor of the current swipe position
	public void AddPoint(Vector2 point) {
		bool sufficientTimeInterval = Time.timeSinceLevelLoad > swipeHistoryTimeLast + swipeHistoryTimeInterval;
		bool sufficientPosInterval = (swipeHistoryPosLast - point).magnitude > swipeHistoryPosInterval;

		if ((timeLimited && sufficientTimeInterval) || (!timeLimited && sufficientPosInterval)) {
			swipeHistoryTimeLast = Time.timeSinceLevelLoad;
			swipeHistoryPosLast = point;

			for (int i = swipeHistoryLengthMax - 2; i >= 0; i--) {
				swipeHistory.CopyElement(i, i + 1);
			}
			swipeHistory.SetElement(0, point, Time.timeSinceLevelLoad);

			swipeHistoryLength++;
			if (swipeHistoryLength > swipeHistoryLengthMax)
				swipeHistoryLength = swipeHistoryLengthMax;
		}
	}

	public int GetNumPoints() {
		return swipeHistoryLength;
	}

	public void ResetPoints() {
		swipeHistory = new Vector2TimeArray(swipeHistoryLengthMax);
		swipeHistoryLength = 0;
	}

	public Vector2TimeArray GetLongestStraightishSwipe() {
		Vector2TimeArray ps = new Vector2TimeArray(0);
		bool found = false;
		for (int i = swipeHistoryLength; i >= minAllowedPoints; i--) {
			ps = FirstXPoints(i);
			if (ArePointsStraightish(ps.GetPositionArray(), straightLineSensitivity)) {
				found = true;
				break;
			}
		}
		if (!found)
			ps = new Vector2TimeArray(0);
		return ps;
	}

	public Vector2TimeArray FirstXPoints(int numPoints) {

		// Returns the first X number of points from the swipeHistory
		Vector2TimeArray xPoints = new Vector2TimeArray(Mathf.Min(numPoints, swipeHistoryLength));
		for (int i = 0; i < xPoints.GetLength(); i++) {
			xPoints.SetElement(i, swipeHistory.GetPosition(i), swipeHistory.GetTime(i));
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

public class Vector2TimeArray {
	private Vector2[] position;
	private float[] time;
	private int length;

	public Vector2TimeArray(int length) {
		this.position = new Vector2[length];
		this.time = new float[length];
		this.length = length;
	}

	public Vector2 GetPosition(int i) {
		return position[i];
	}

	public Vector2[] GetPositionArray() {
		return position;
	}

	public float GetTime(int i) {
		return time[i];
	}

	public float[] GetTimeArray() {
		return time;
	}

	public int GetLength() {
		return this.length;
	}

	public void SetElement(int i, Vector2 position, float time) {
		this.position[i] = position;
		this.time[i] = time;
	}

	public void CopyElement(int from, int to) {
		this.position[to] = this.position[from];
		this.time[to] = this.time[from];
	}
}