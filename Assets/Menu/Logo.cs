using UnityEngine;
using System.Collections;

public class Logo : MonoBehaviour {

	public float angleOffset = 0f;

	void Update() {
		transform.eulerAngles = new Vector3(1f, angleOffset + Mathf.Sin(Time.timeSinceLevelLoad * 1.5f) * 5f, 1f);
	}
}
