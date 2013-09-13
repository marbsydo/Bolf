using UnityEngine;
using System.Collections;

public class GUIScore : MonoBehaviour {

	public GUIText t;

	int strokes = 0;
	int par;

	void Awake() {
		par = GetPar(Application.loadedLevelName);
	}

	public void IncStrokes() {
		strokes++;
	}

	public void ResetStrokes() {
		strokes = 0;
	}

	void Update() {
		t.text = "Par: " + par + "\nStrokes: " + strokes;
	}

	int GetPar(string levelName) {
		int par = 1;
		if (levelName == "level_lawn_1") {
			par = 1;
		} else if (levelName == "level_lawn_2") {
			par = 3;
		} else {
			Debug.LogWarning("Unknown level: " + levelName);
		}
		return par;
	}
}
