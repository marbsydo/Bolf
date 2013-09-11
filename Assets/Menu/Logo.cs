using UnityEngine;
using System.Collections;

public class Logo : MonoBehaviour {
	void Update() {
		transform.eulerAngles = new Vector3(1f, Mathf.Sin(Time.timeSinceLevelLoad * 1.5f) * 5f, 1f);
	}
}
