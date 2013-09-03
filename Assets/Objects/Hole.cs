using UnityEngine;
using System.Collections;

public class Hole : MonoBehaviour {
	void OnTriggerStay(Collider collider) {
		if (collider.CompareTag("Ball")) {
			//Debug.Log("Ball is near hole");

			// Apply a force to the bal
			Vector3 diff = collider.transform.position - transform.position;
			Vector3 forceVector;
			if (diff.magnitude > 0.5f) {
				forceVector = diff.normalized * -0.3f * 1/Mathf.Max(diff.magnitude, 1);
				collider.rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
			} else {
				forceVector = diff.normalized * -1.5f * 1/Mathf.Max(diff.magnitude, 1);
				collider.rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
			}
		}
	}
}
