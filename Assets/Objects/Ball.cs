using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
	public void Roll(Vector2 v) {
		rigidbody.AddForce((Vector3)v, ForceMode.Impulse);
	}
}
