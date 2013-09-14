using UnityEngine;
using System.Collections;

public class Nozzle : MonoBehaviour {

	public Hose hose;

	const float maxDistance = 8f;
	const float maxForce = 200f;
	float currentDistance = maxDistance;
	float currentForce = maxForce;

	//float startAngle;
	WaterJet waterJet;

	void Awake() {
		waterJet = new WaterJet(currentDistance, currentForce, true);
		waterJet.Init();
		//startAngle = transform.eulerAngles.z;
	}

	void Update() {
		GameObject ball = GameObject.Find("Ball") as GameObject;

		float lerpTime = Time.deltaTime * 2f; // Less time = slower

		if (hose.GetTimeOfLastBulge() > Time.timeSinceLevelLoad - 0.5f) {
			// Got water recently
			currentDistance = Mathf.Lerp(currentDistance, maxDistance, lerpTime);
			currentForce = Mathf.Lerp(currentForce, maxForce, lerpTime);
		}  else {
			currentDistance = Mathf.Lerp(currentDistance, 1f, lerpTime);
			currentForce = Mathf.Lerp(currentForce, 5f, lerpTime);
			//currentDistance -= Time.deltaTime * 1f;
			//currentForce -= Time.deltaTime * 2f;
		}

		waterJet.SetMaxDistance(currentDistance);
		waterJet.SetMaxForce(currentForce);

		if (ball != null) {
			// Rotate towards the ball
			float angle = Mathf.Atan2(ball.transform.position.y - transform.position.y, ball.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0f, 0f, angle);

			// Spray the ball
			Vector3 jetOrigin;
			Vector3 jetVector = waterJet.EulerAngleToNormalizedVector(angle);
			jetOrigin = transform.position + jetVector * 0.5f;
			waterJet.Spray(jetOrigin, jetVector);
		}
	}
}
