using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	GameObject holes;

	public void Awake() {
		Transform[] children = gameObject.GetComponentsInChildren<Transform>();
		foreach (Transform child in children) {
			if (child.name == "Holes") {
				holes = child.gameObject;
			}
		}
	}

	public void Update() {
		holes.transform.Rotate(((new Vector3(rigidbody.velocity.y, 0f, -rigidbody.velocity.x)).normalized) * (rigidbody.velocity.magnitude * 50f * Time.deltaTime));
	}

	public void Roll(Vector2 v) {
		rigidbody.AddForce((Vector3)v, ForceMode.VelocityChange);
	}
}
