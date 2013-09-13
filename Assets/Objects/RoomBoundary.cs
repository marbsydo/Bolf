using UnityEngine;
using System.Collections;

public class RoomBoundary : MonoBehaviour {
	void Awake() {
		MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in meshRenderers) {
			Destroy(meshRenderer);
		}
	}
}
