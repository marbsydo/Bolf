using UnityEngine;
using System.Collections;

public class Hole : MonoBehaviour {
	void OnTriggerStay(Collider collider) {
		if (collider.CompareTag("Ball")) {
			//Debug.Log("Ball is near hole");

			// Apply a force to the bal
			Vector3 diff = collider.transform.position - transform.position;
			Vector3 forceVector;
			if (diff.magnitude > 0.3f && diff.magnitude < 0.7f) {
				forceVector = diff.normalized * -0.1f * 1/Mathf.Max(diff.magnitude, 1);
				collider.rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
			}
			if (diff.magnitude < 0.5f) {
				Ball ball = collider.GetComponent<Ball>();
				if (ball.rigidbody.velocity.magnitude < 5f) {
					ball.GoInHole(this);
				}
			}
		}
	}
}
