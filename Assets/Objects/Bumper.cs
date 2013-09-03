using UnityEngine;
using System.Collections;

public class Bumper : MonoBehaviour {
	void OnCollisionEnter(Collision collision) {
		if (collision.collider.CompareTag("Ball")) {
			// The ball hit us

			// Do the bumper animation
			Expand();

			// Apply a force to the ball
			Vector3 forceVector = (collision.transform.position - transform.position).normalized * 5f;
			collision.rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
		}
	}

	void Expand() {
		transform.localScale = Vector3.one * 1.3f;
	}

	void Update() {
		transform.localScale = Vector3.Slerp(transform.localScale, Vector3.one, Time.deltaTime * 8);
	}
}
