using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {
	void Awake() {
		GameObject floorSprite = Resources.Load("Sprites/FloorSprite") as GameObject;
		GameObject floorLineSprite = Resources.Load("Sprites/FloorLineSprite") as GameObject;

		GameObject floorObj = new GameObject();
		floorObj.transform.position = Vector3.zero;
		floorObj.name = "Floor";

		int tilesX = 8;
		int tilesY = 16;
		float lineY = 4f;

		for (int j = 0; j < tilesY; j++) {
			for (int i = 0; i < tilesX; i++) {
				GameObject floorSpriteObj = GameObject.Instantiate(floorSprite, new Vector3(i * 2, j * 2, 0), Quaternion.identity) as GameObject;
				floorSpriteObj.name = "Floor";
				floorSpriteObj.transform.parent = floorObj.transform;
			}
		}

		for (int i = 0; i < tilesX; i++) {
			GameObject floorLineSpriteObj = GameObject.Instantiate(floorLineSprite, new Vector3(i * 2, lineY, -0.1f), Quaternion.identity) as GameObject;
			floorLineSpriteObj.name = "FloorLine";
			floorLineSpriteObj.transform.parent = floorObj.transform;
		}

		floorObj.transform.position = new Vector3(-7f, -12f, 1f);
	}
}
