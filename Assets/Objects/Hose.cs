using UnityEngine;
using System.Collections;

public class Hose : MonoBehaviour {

	// You can change these three:
	const int numBulges = 10; // How many bulges to make
	const float spacing = 2f; // How far apart they are spread
	const float speed = 3f;   // Speed the bulges move at

	// Do not set these
	// These are set from reading transform.eulerAngles.z
	float angle;
	float angleCos;
	float angleSin;
	Vector3 spacingVector;
	Vector3 spacingVectorNormalized;

	GameObject[] bulges;

	void Awake() {
		Object bulgePrefab = Resources.Load("Sprites/HoseBulgeSprite");
		bulges = new GameObject[numBulges];

		angle = transform.eulerAngles.z * Mathf.Deg2Rad;
		angleCos = Mathf.Cos(angle);
		angleSin = Mathf.Sin(angle);

		spacingVector = new Vector3(spacing * angleCos, spacing * angleSin, 0f);
		spacingVectorNormalized = spacingVector.normalized;

		Quaternion rot = Quaternion.Euler(0f, 0f, transform.eulerAngles.z);

		for (int i = 0; i < bulges.Length; i++) {
			float offset = i * spacing;
			bulges[i] = GameObject.Instantiate(bulgePrefab, transform.position + spacingVector * i, rot) as GameObject;
		}
	}

	void Update() {
		float step = speed * -Time.deltaTime;
		Vector3 bulgeStep = new Vector3(step * angleCos, step * angleSin, 0f);

		GameObject furthest = Furthest();

		for (int i = 0; i < bulges.Length; i++) {
			bulges[i].transform.position += bulgeStep;

			Vector3 vectorToHoseNormalized = (transform.position - bulges[i].transform.position).normalized;

			if (ApproximatelyEqual(vectorToHoseNormalized, spacingVectorNormalized)) {
				bulges[i].transform.position = furthest.transform.position + spacingVector;
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
		GameObject furthest = bulges[0];
		for (int i = 0; i < bulges.Length; i++) {
			float disTemp = (transform.position - bulges[i].transform.position).sqrMagnitude;
			if (disTemp > dis) {
				dis = disTemp;
				furthest = bulges[i];
			}
		}
		return furthest;
	}

}
