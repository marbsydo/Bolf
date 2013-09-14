using UnityEngine;
using System.Collections;

public class Hose : MonoBehaviour {

	// You can change these three:
	const int numBulges = 10; // How many bulges to make
	const float spacing = 1.5f; // How far apart they are spread
	const float speed = 3f;   // Speed the bulges move at

	// Do not set these
	// These are set from reading transform.eulerAngles.z
	float angle;
	float angleCos;
	float angleSin;
	Vector3 spacingVector;
	Vector3 spacingVectorNormalized;

	HoseBulge[] bulges;

	float timeOfLastBulge = 0f;

	public float GetTimeOfLastBulge() {
		return timeOfLastBulge;
	}

	void Awake() {
		Object bulgePrefab = Resources.Load("Secondary/HoseBulge");
		bulges = new HoseBulge[numBulges];

		angle = transform.eulerAngles.z * Mathf.Deg2Rad;
		angleCos = Mathf.Cos(angle);
		angleSin = Mathf.Sin(angle);

		spacingVector = new Vector3(spacing * angleCos, spacing * angleSin, 0f);
		spacingVectorNormalized = spacingVector.normalized;

		Quaternion rot = Quaternion.Euler(0f, 0f, transform.eulerAngles.z);

		// Create all the bulges in a line leading out from the hose
		for (int i = 0; i < bulges.Length; i++) {
			float offset = i * spacing;
			bulges[i] = (GameObject.Instantiate(bulgePrefab, transform.position + spacingVector * i, rot) as GameObject).GetComponent<HoseBulge>() as HoseBulge;
		}
	}

	void Update() {

		// Overview:
		// Move all of the bulges towards the hose
		// If a bulge has moved too far, move it back to the end of the line
		// The end of the line is found by finding the Furthest() away bulge

		float step = speed * -Time.deltaTime;
		Vector3 bulgeStep = new Vector3(step * angleCos, step * angleSin, 0f);

		GameObject furthest = Furthest();

		for (int i = 0; i < bulges.Length; i++) {
			// Check the bulge is not hitting something
			if (!bulges[i].GetColliding())
				bulges[i].transform.position += bulgeStep;

			Vector3 vectorToHoseNormalized = (transform.position - bulges[i].transform.position).normalized;

			// This function essentially checks whether the bulge has travelled beyond the hose
			// The logic is as follows:
			// If the direction FROM the hose TO the bulge is the same as the direction the bulge is travelling in, it has gone too far
			// Comparing the direction is then done in a hacky way by normalizing the vectors and checking if they are similar. It's good enough.
			if (ApproximatelyEqual(vectorToHoseNormalized, spacingVectorNormalized)) {
				// Move the bulge back to the end of the line
				bulges[i].transform.position = furthest.transform.position + spacingVector;

				// Record the time at which this occurs
				timeOfLastBulge = Time.timeSinceLevelLoad;
			}
		}
	}

	// Compares if two vector3s are approximately equal
	// NOTE: DOES NOT COMPARE THE Z! (this is intended, just fyi)
	bool ApproximatelyEqual(Vector3 a, Vector3 b) {
		return (ApproximatelyEqual(a.x, b.x, 0.01f) && ApproximatelyEqual(a.y, b.y, 0.01f));
	}

	bool ApproximatelyEqual(float a, float b, float tolerance) {
		return (Mathf.Abs(a - b) < tolerance);
	}

	GameObject Furthest() {
		float dis = -1f;
		GameObject furthest = bulges[0].gameObject;
		for (int i = 0; i < bulges.Length; i++) {
			float disTemp = (transform.position - bulges[i].transform.position).sqrMagnitude;
			if (disTemp > dis) {
				dis = disTemp;
				furthest = bulges[i].gameObject;
			}
		}
		return furthest;
	}

}
