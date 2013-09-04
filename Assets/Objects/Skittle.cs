using UnityEngine;
using System.Collections;

public class Skittle : MonoBehaviour {
	void FixedUpdate() {
		rigidbody.AddForce(new Vector3(0f, 0f, 9.81f), ForceMode.Force);
	}
}
