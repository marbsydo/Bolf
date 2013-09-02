using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {
	void Awake() {
		GameObject floorSprite = Resources.Load("Sprites/FloorSprite") as GameObject;

		GameObject floorObj = new GameObject();
		floorObj.transform.position = Vector3.zero;
		floorObj.name = "Floor";

		int tilesX = 10;
		int tilesY = 18;

		for (int j = 0; j < tilesY; j++) {
			for (int i = 0; i < tilesX; i++) {
				GameObject floorSpriteObj = GameObject.Instantiate(floorSprite, new Vector3(i * 2, j * 2, 0), Quaternion.identity) as GameObject;
				floorSpriteObj.transform.parent = floorObj.transform;
			}
		}

		floorObj.transform.position = new Vector3(-7f, -11f, 0.5f);
	}
}
