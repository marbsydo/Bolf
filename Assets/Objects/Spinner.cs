using UnityEngine;
using System.Collections;

public enum SpinnerDirection {Clockwise, AntiClockwise};

public class Spinner : MonoBehaviour {
	public float maxAngularVelocity = 7f;
	public float torque = 10f;
	public SpinnerDirection spinnerDirection = SpinnerDirection.AntiClockwise;

	void Start() {
		rigidbody.maxAngularVelocity = maxAngularVelocity;
	}

	void FixedUpdate() {
		rigidbody.AddTorque(new Vector3(0, 0, torque * (spinnerDirection == SpinnerDirection.Clockwise ? -1 : 1)), ForceMode.VelocityChange);
	}
}
