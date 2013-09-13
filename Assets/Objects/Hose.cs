using UnityEngine;
using System.Collections;

public class Hose : MonoBehaviour {
	float startAngle;

	bool gotWater = true;

	WaterJet waterJet = new WaterJet(8f, 50f);

	void Awake() {
		waterJet.Init();
		startAngle = transform.eulerAngles.z;
	}

	void Update() {
		GameObject ball = GameObject.Find("Ball") as GameObject;

		if (ball != null) {
			// Rotate towards the ball
			float angle = Mathf.Atan2(ball.transform.position.y - transform.position.y, ball.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0f, 0f, angle);

			// Spray the ball
			if (gotWater) {
				Vector3 jetOrigin;
				Vector3 jetVector = waterJet.EulerAngleToNormalizedVector(angle);
				jetOrigin = transform.position + jetVector * 0.5f;
				waterJet.Spray(jetOrigin, jetVector);
			}
		}
	}
}
