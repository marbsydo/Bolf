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
		//holes.transform.Rotate((new Vector3(rigidbody.velocity.y, -rigidbody.velocity.x, 0f)).normalized * (rigidbody.velocity.magnitude / 30));
		Vector3 a = (new Vector3(rigidbody.velocity.y, -rigidbody.velocity.x, 0f)).normalized;
		float b = (rigidbody.velocity.magnitude / 30);
		holes.transform.Rotate(a * b);
	}

	public void Roll(Vector2 v) {
		rigidbody.AddForce((Vector3)v, ForceMode.VelocityChange);
	}
}
