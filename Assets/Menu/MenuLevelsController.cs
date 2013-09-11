using UnityEngine;
using System.Collections;

public enum GameWorld {Lawn, Pinball, Sky, Space}

public class MenuLevelsController : MonoBehaviour {

	public GameObject prefabButtonlevel;

	GameWorld gameWorld = GameWorld.Lawn;

	void Awake() {
		Vector3 middle =  transform.position + new Vector3(-2f, -2f, 0f);
		for (int j = 0; j < 8; j++) {
			for (int i = 0; i < 3; i++) {
				GameObject.Instantiate(prefabButtonlevel, middle + new Vector3(i * 2f, j * 1.2f, 0f), Quaternion.identity);
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
