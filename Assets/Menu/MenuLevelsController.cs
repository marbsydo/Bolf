using UnityEngine;
using System.Collections;

public enum GameWorld {Lawn, Pinball, Sky, Space}

public class MenuLevelsController : MonoBehaviour {

	public GameObject prefabButtonlevel;

	GameWorld gameWorld = GameWorld.Lawn;

	void Awake() {
		Vector3 middle =  transform.position + new Vector3(3f, 4f, 0f);
		int level = 1;

		// rows of 4 3 4 3 4
		for (int row = 0; row < 5; row++) {
			int colSize = 3 + (1 - (row % 2));
			for (int col = 0; col < colSize; col++) {
				GameObject obj = GameObject.Instantiate(prefabButtonlevel, middle + new Vector3(col * -2f + (colSize == 3 ? -1f : 0f), row * -1.5f, 0f), Quaternion.Euler(180f, 0f, 0f)) as GameObject;

				// Change the object's name and assign it its level number
				obj.name = "Level" + level;
				obj.GetComponent<MenuButton>().SetLevelNumber(level);
				level++;
			}
		}
	}

	public void SetGameWorld(GameWorld gameWorld) {
		this.gameWorld = gameWorld;
	}

	public Color GetDesiredBackgroundColor() {
		Color c;
		switch (this.gameWorld) {
		case GameWorld.Lawn:
			c = new Color(0.33203f, 0.71875f, 0.18750f);
			break;
		case GameWorld.Pinball:
			c = new Color(0.11764f, 0.45490f, 0.61176f);
			break;
		case GameWorld.Sky:
			c = new Color(0.50588f, 1.00000f, 0.92941f);
			break;
		case GameWorld.Space:
			c = new Color(0.75294f, 1.00000f, 0.04705f);
			break;
		default:
			Debug.LogWarning("Unknown gameWorld " + gameWorld);
			goto case GameWorld.Lawn;
		}
		return c;
	}
}
