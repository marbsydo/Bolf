using UnityEngine;
using System.Collections;

public enum GameWorld {Lawn, Pinball, Sky, Space}

public class SpecificLevel {

	public GameWorld gameWorld;
	public int levelNumber;

	public SpecificLevel(GameWorld gameWorld, int levelNumber) {
		this.gameWorld = gameWorld;
		this.levelNumber = levelNumber;
	}
}

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

		/*
		for (int j = 0; j < tilesY; j++) {
			for (int i = 0; i < tilesX; i++) {
				GameObject floorSpriteObj = GameObject.Instantiate(floorSprite, new Vector3(i * 2, j * 2, 0), Quaternion.identity) as GameObject;
				floorSpriteObj.name = "Floor";
				floorSpriteObj.transform.parent = floorObj.transform;
			}
		}
		*/
		/*
		for (int i = 0; i < tilesX; i++) {
			GameObject floorLineSpriteObj = GameObject.Instantiate(floorLineSprite, new Vector3(i * 2, lineY, -0.1f), Quaternion.identity) as GameObject;
			floorLineSpriteObj.name = "FloorLine";
			floorLineSpriteObj.transform.parent = floorObj.transform;
		}

		floorObj.transform.position = new Vector3(-7f, -12f, 1f);
		*/
	}

	// Returns true on success
	public bool PlayLevel(SpecificLevel specificLevel) {
		bool success = true;
		// Check number is valid
		if (IsValidLevelNumber(specificLevel.levelNumber)) {
			//TODO: Might want to verify that the world is valid
			Application.LoadLevel("level_" + GameWorldToString(specificLevel.gameWorld) + "_" + specificLevel.levelNumber);

			//NOTE: This method of catching errors shouldn't be relied upon in production
			// however there is no proper way of checking whether a level exists before loading it
			// thus it is impossible to avoid an error when attempting to load a level that does not exist
			if (!Application.isLoadingLevel) {
				Debug.LogWarning("Level does not appear to exist!");
				success = false;
			}
		} else {
			Debug.LogWarning("Could not load level. Number is invalid: " + specificLevel.levelNumber);
			success = false;
		}

		return success;
	}

	public string GameWorldToString(GameWorld gameWorld) {
		string s = "";
		switch (gameWorld) {
			case GameWorld.Lawn:	s = "lawn";		break;
			case GameWorld.Pinball:	s = "pinball";	break;
			case GameWorld.Sky:		s = "sky";		break;
			case GameWorld.Space:	s = "space";	break;
		}
		return s;
	}

	public bool IsValidLevelNumber(int levelNumber) {
		return (levelNumber >= 1 && levelNumber <= 18);
	}
}