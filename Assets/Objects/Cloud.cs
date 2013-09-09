using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour {
	Vector3 startPos;

	void Awake() {
		startPos = transform.position;
	}

	void Update() {
		transform.position = startPos + new Vector3(Mathf.Sin(Time.timeSinceLevelLoad * 1.2f) * 0.2f, Mathf.Cos(Time.timeSinceLevelLoad * 1.9f) * 0.05f, 0f);
	}
}
