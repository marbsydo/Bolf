using UnityEngine;
using System.Collections;

public class Hole : MonoBehaviour {
	void OnTriggerEnter(Collider collider) {
		if (collider.CompareTag("Ball")) {
			Debug.Log("Ball is near hole");
		}
	}
}
