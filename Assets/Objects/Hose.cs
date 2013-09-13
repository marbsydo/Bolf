using UnityEngine;
using System.Collections;

public class Hose : MonoBehaviour {
	float startAngle;

	void Awake() {
		startAngle = transform.eulerAngles.z;
	}

	void Update() {
		
	}
}
