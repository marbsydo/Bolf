using UnityEngine;
using System.Collections;

public enum GUIAnchorX {Left, Middle, Right}
public enum GUIAnchorY {Top, Middle, Bottom}
public enum GUILayer {LayerTop, Layer2, Layer3, LayerMiddle, Layer5, Layer6, LayerBottom};

public class GUIAnchor : MonoBehaviour {

	public GUIAnchorX anchorX = GUIAnchorX.Middle;
	public GUIAnchorY anchorY = GUIAnchorY.Middle;
	public Vector2 offset = Vector2.zero;
	public GUILayer layer = GUILayer.LayerMiddle;

	GameObject guiCamera;

	void Awake() {
		guiCamera = GameObject.Find("GUICamera");
		UpdatePosition();
	}

	void Update() {
		UpdatePosition();
	}

	void UpdatePosition() {
		Vector2 halfWidth = Vector2.zero;
		halfWidth.x = ((float) guiCamera.camera.orthographicSize / Screen.height) * Screen.width;
		halfWidth.y = ((float) guiCamera.camera.orthographicSize);

		Vector2 anchorOffset = Vector2.zero;
		switch (anchorX) {
		case GUIAnchorX.Left:		anchorOffset.x = -halfWidth.x;		break;
		case GUIAnchorX.Middle:		anchorOffset.x = 0f;				break;
		case GUIAnchorX.Right:		anchorOffset.x = halfWidth.x;		break;
		}
		switch (anchorY) {
		case GUIAnchorY.Top:		anchorOffset.y = halfWidth.y;		break;
		case GUIAnchorY.Middle:		anchorOffset.y = 0f;				break;
		case GUIAnchorY.Bottom:		anchorOffset.y = -halfWidth.y;		break;	
		}

		transform.position = guiCamera.transform.position + new Vector3(anchorOffset.x + offset.x, anchorOffset.y + offset.y, 10f + ((int) layer));
	}
}
