using UnityEngine;
using System.Collections;

public class SprinklerMain : MonoBehaviour {

	public float maxVelocity = 1f;
	public float torque = -100f;

	void Start() {
		rigidbody.maxAngularVelocity = maxVelocity;
	}

	void FixedUpdate() {
		rigidbody.AddTorque(new Vector3(0f, 0f, torque), ForceMode.Acceleration);
	}
}
