using UnityEngine;
using System.Collections;

public class HoseBulge : MonoBehaviour {

	public bool colliding;

	float lastHitTime;

	void OnTriggerStay(Collider collider) {
		if (collider.CompareTag("DynamicObject") || collider.CompareTag("Ball") || collider.name == "HoseBulgeCollider") {
			colliding = true;
			lastHitTime = Time.timeSinceLevelLoad;
		}
	}

	void LateUpdate() {
		if (colliding) {
			// If hit nothing for 0.5 seconds, we are no longer colliding
			if (lastHitTime < Time.timeSinceLevelLoad - 0.5f) {
				colliding = false;
			}
		}
	}

	public bool GetColliding() {
		return colliding;
	}
}
