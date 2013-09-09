using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	GameObject holes;

	Hole targetHole;
	bool isInHole = false;

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
			
			// Work out the vector between us (the ball) and the bottom of the hole (which is slightly lower than the center to give a 3D effect)
			Vector3 diff = transform.position - (targetHole.transform.position + new Vector3(0, -0.25f, 0));

			// Make the ball shrink slightly as it goes in the hole
			transform.localScale = Vector3.Slerp(transform.localScale, Vector3.one * 0.9f, Time.deltaTime * 2f);

			// Apply a high amount of drag to make the ball come to a reset when it reaches the middle
			rigidbody.drag = 5f;

			if (diff.magnitude > 0.05f) {
				// Pull ball into hole until it comes to rest
				rigidbody.AddForce(diff.normalized * -0.6f * 1/Mathf.Max(diff.magnitude, 1f) * (Time.deltaTime * 40), ForceMode.VelocityChange);
			}
		}
	}

	public void Roll(Vector2 v) {
		rigidbody.AddForce((Vector3)v, ForceMode.VelocityChange);
	}

	public void GoInHole(Hole hole) {
		targetHole = hole;
		isInHole = true;
	}

	public bool IsInHole() {
		return isInHole;
	}

	public void SetScale(Vector3 s) {
		transform.localScale = s;
	}
}
