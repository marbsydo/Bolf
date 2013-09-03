using UnityEngine;
using System.Collections;

public class Hole : MonoBehaviour {

	const float fallInHoleVelocityThreshold = 8f;

	float timeLastChipBall = 0;

	void OnTriggerStay(Collider collider) {
		if (collider.CompareTag("Ball")) {
			//Debug.Log("Ball is near hole");

			// Apply a force to the bal
			Vector3 diff = collider.transform.position - transform.position;
			Vector3 forceVector;
			if (diff.magnitude > 0.3f && diff.magnitude < 0.7f) {
				forceVector = diff.normalized * -0.1f * 1/Mathf.Max(diff.magnitude, 1) * (Time.deltaTime * 40);
				collider.rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
			}
			if (diff.magnitude < 0.7f) {
				Ball ball = collider.GetComponent<Ball>();
				if (ball.rigidbody.velocity.magnitude < fallInHoleVelocityThreshold) {
					ball.GoInHole(this);
				} else {
					// Too fast, so chip off some speed and knock the ball a bit
					if (Time.timeSinceLevelLoad > timeLastChipBall + 1f) {
						timeLastChipBall = Time.timeSinceLevelLoad;

						float chipAngle = 20 + Random.value * 20 * (Random.value < 0.5f ? -1 : 1);
						ball.rigidbody.velocity = Quaternion.AngleAxis(chipAngle, Vector3.forward) * ball.rigidbody.velocity;
					}
				}
			}
		}
	}

	Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle) {
		return angle * (point - pivot) + pivot;
	}
}
