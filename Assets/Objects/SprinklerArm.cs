using UnityEngine;
using System.Collections;

public class SprinklerArm : MonoBehaviour {

	public int angleOffset;

	WaterJet waterJet = new WaterJet(8f, 30f, false);

	void Awake() {
		waterJet.Init();
	}

	void Update() {
		float angle = angleOffset + transform.parent.eulerAngles.z;
		Vector3 jetOrigin = transform.position;
		Vector3 jetVector = waterJet.EulerAngleToNormalizedVector(angle);
		waterJet.Spray(jetOrigin, jetVector);
	}
}

public class WaterJet {

	float maxDistance; // How far the jet reaches
	float jetForce;    // The force the jet applies
	bool increasing;   // If true, the force increases linearly closer to the source of the jet

	GameObject waterJet;
	GameObject waterSplash;

	public WaterJet(float maxDistance, float jetForce, bool increasing) {
		this.maxDistance = maxDistance;
		this.jetForce = jetForce;
		this.increasing = increasing;
	}

	public void SetMaxDistance(float maxDistance) {
		this.maxDistance = maxDistance;
	}

	public void SetMaxForce(float jetForce) {
		this.jetForce = jetForce;
	}

	public void Init() {
		waterJet = GameObject.Instantiate(Resources.Load("Sprites/WaterJetSprite") as Object, Vector3.zero, Quaternion.identity) as GameObject;
		waterSplash = GameObject.Instantiate(Resources.Load("Sprites/WaterSplashSprite") as Object, Vector3.zero, Quaternion.identity) as GameObject;
	}

	public Vector3 EulerAngleToNormalizedVector(float angle) {
		return (new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f)).normalized;
	}

	public void Spray(Vector3 jetOrigin, Vector3 jetVector) {
		Ray ray = new Ray(jetOrigin, jetVector);
		
		float rayDistance = maxDistance;

		RaycastHit hit;
		int layerMask = ~(1 << 11);	// Hit all layers except layer 11 (layer 11 is the NoJetCollider layer)
		if (Physics.Raycast(ray, out hit, maxDistance, layerMask)) {
			rayDistance = hit.distance;
			if (hit.collider != null) {
				if (hit.collider.gameObject.rigidbody != null) {
					// Check if it is the sort of rigidbody we should hit
					if (hit.collider.gameObject.rigidbody.CompareTag("DynamicObject") || hit.collider.gameObject.rigidbody.CompareTag("Ball")) {
						// Apply a force to what we hit
						float appliedForce = jetForce;
						if (increasing) {
							// Scale linearly, so it is more powerful when the distance is shorter
							appliedForce = jetForce - (jetForce / maxDistance * rayDistance);
						}
						hit.collider.gameObject.rigidbody.AddForce(jetVector * appliedForce, ForceMode.Acceleration);
					}
				}
			}
		}

		Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

		// Update the jet sprite
		if (waterJet != null) {
			Vector3 midPoint = (ray.origin + (ray.direction * rayDistance / 2));
			waterJet.transform.position = midPoint + new Vector3(0f, 0f, 0.1f);
			waterJet.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(jetVector.y, jetVector.x) * Mathf.Rad2Deg);
			waterJet.transform.localScale = new Vector3(rayDistance * 2f, 1f, 1f);
		}

		// Update the splash sprite
		if (waterSplash != null) {
			Vector3 endPoint = (ray.origin + ray.direction * rayDistance);
			waterSplash.transform.position = endPoint + new Vector3(0f, 0f, 0.05f);
			waterSplash.transform.eulerAngles = new Vector3(0f, 0f, Time.timeSinceLevelLoad * 400f);
		}
	}
}