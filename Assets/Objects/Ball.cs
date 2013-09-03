using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	GameObject holes;

	Hole targetHole;

	float velocityThreshold1 = 5f;
	float velocityThreshold2 = 7.5f;
	float velocityThreshold3 = 10f;

	public void Awake() {
		Transform[] children = gameObject.GetComponentsInChildren<Transform>();
		foreach (Transform child in children) {
			if (child.name == "Holes") {
				holes = child.gameObject;
			}
		}
	}

	public void FixedUpdate() {
		holes.transform.Rotate(((new Vector3(rigidbody.velocity.y, 0f, -rigidbody.velocity.x)).normalized) * (rigidbody.velocity.magnitude * 50f * Time.deltaTime));

		//Debug.Log(rigidbody.velocity.magnitude);

		float v = rigidbody.velocity.magnitude;
		if (v > velocityThreshold3) {
			rigidbody.drag = 2f;
		} else if (v > velocityThreshold2) {
			rigidbody.drag = 0.2f;
		} else if (v > velocityThreshold1) {
			rigidbody.drag = 0.1f;
		} else {
			rigidbody.drag = 0.05f;
		}

		if (targetHole != null) {
			// Apply high drag and pull the ball into the hole
			rigidbody.drag = 5f;
			Vector3 diff = transform.position - (targetHole.transform.position + new Vector3(0, -0.2f, 0));
			if (diff.magnitude > 0.05f)
				rigidbody.AddForce(diff.normalized * -0.1f * 1/Mathf.Max(diff.magnitude, 1f) * (Time.deltaTime * 40), ForceMode.VelocityChange);
		}
	}

	public void Roll(Vector2 v) {
		rigidbody.AddForce((Vector3)v, ForceMode.VelocityChange);
	}

	public void GoInHole(Hole hole) {
		targetHole = hole;
	}
}
