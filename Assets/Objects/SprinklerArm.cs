using UnityEngine;
using System.Collections;

public class SprinklerArm : MonoBehaviour {

	float maxDistance = 8f;
	float sprinkleForce = 30f;

	public int angleOffset;

	GameObject waterJet;
	GameObject waterSplash;

	void Awake() {
		waterJet = GameObject.Instantiate(Resources.Load("Sprites/WaterJetSprite") as Object, transform.position, Quaternion.identity) as GameObject;
		waterSplash = GameObject.Instantiate(Resources.Load("Sprites/WaterSplashSprite") as Object, transform.position, Quaternion.identity) as GameObject;
	}

	void Update() {
		float angle = angleOffset + transform.parent.eulerAngles.z;
		Vector3 sprinkleVector = (new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f)).normalized;
		// sprinkleVector is a normalized vector pointing out from the end of the sprinkler's arm
		//Debug.DrawLine(transform.position, transform.position + sprinkleVector, Color.red);

		Ray ray = new Ray(transform.position, sprinkleVector);
		
		float rayDistance = maxDistance;

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, maxDistance)) {
			rayDistance = hit.distance;
			if (hit.collider != null) {
				if (hit.collider.gameObject.rigidbody != null) {
					// Apply a force to what we hit
					hit.collider.gameObject.rigidbody.AddForce(sprinkleVector * sprinkleForce, ForceMode.Acceleration);
				}
			}
		}

		Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

		// Update the jet sprite
		Vector3 midPoint = (ray.origin + (ray.direction * rayDistance / 2));
		waterJet.transform.position = midPoint + new Vector3(0f, 0f, 0.1f);
		waterJet.transform.eulerAngles = new Vector3(0f, 0f, angle);
		waterJet.transform.localScale = new Vector3(rayDistance * 2f, 1f, 1f);

		// Update the splash sprite
		Vector3 endPoint = (ray.origin + ray.direction * rayDistance);
		waterSplash.transform.position = endPoint + new Vector3(0f, 0f, 0.05f);
		waterSplash.transform.eulerAngles = new Vector3(0f, 0f, Time.timeSinceLevelLoad * 400f);
	}
}
