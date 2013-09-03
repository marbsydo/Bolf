using UnityEngine;
using System.Collections;

public class Hole : MonoBehaviour {

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
			if (diff.magnitude < 0.5f) {
				Ball ball = collider.GetComponent<Ball>();
				if (ball.rigidbody.velocity.magnitude < 5f) {
					ball.GoInHole(this);
				} else {
					// Too fast, so chip off some speed and knock the ball a bit
					if (Time.timeSinceLevelLoad > timeLastChipBall + 1f) {
						timeLastChipBall = Time.timeSinceLevelLoad;
						//ball.rigidbody.AddForce(new Vector3(Random.value, Random.value, 0) * 10000000f, ForceMode.VelocityChange);
						//ball.rigidbody.velocity = ball.rigidbody.velocity + new Vector3(Random.value, Random.value, 0f) * 10f;
						//ball.rigidbody.velocity = RotateAroundPoint(ball.rigidbody.velocity, Vector3.zero, Quaternion.Euler(0, 90, 0));
						//ball.rigidbody.velocity *= 0.8f;
						//ball.rigidbody.MoveRotation(Quaternion.Euler(0, 0, 45));

						float t = ball.rigidbody.velocity.magnitude;
						//ball.rigidbody.AddRelativeForce(new Vector3(Random.value, Random.value, 0f) * -10f, ForceMode.VelocityChange);
						float chipAngle;
						chipAngle = 45 + Random.value * 45;
						if (Random.value < 0.5)
							chipAngle *= -1;

						ball.rigidbody.velocity = Quaternion.AngleAxis(chipAngle, Vector3.forward) * ball.rigidbody.velocity;
						if (ball.rigidbody.velocity.magnitude > t) {
							ball.rigidbody.velocity = ball.rigidbody.velocity.normalized * t;
						}
						Debug.Log("CHIP! " + Random.value);
					}
				}
			}
		}
	}

	Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle) {
		return angle * (point - pivot) + pivot;
	}
}
