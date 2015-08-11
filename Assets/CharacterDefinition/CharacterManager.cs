using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {
	public Material spriteMaterial;

	private static CharacterManager instance;
	public static CharacterManager Instance() {
		if (instance == null) {
			instance = (CharacterManager)FindObjectOfType(typeof(CharacterManager));
			if (instance == null && !Application.isPlaying) {
				instance = new GameObject("Character Manager").AddComponent<CharacterManager>();
			}
		}
		return instance;
	}

	void Start() {
		foreach(Transform t in GetComponentsInChildren<Transform>()) {
			t.gameObject.SetActive(false);
		}
		gameObject.SetActive(true);
	}

	// TODO
	public List<CharacterDefinition> GetCharacters() { 
		return new List<CharacterDefinition>(GetComponentsInChildren<CharacterDefinition>());
	}

	public void BuildEmptyCharacter() {
		CharacterDefinition newbie = new GameObject("Character").AddComponent<CharacterDefinition>();
		newbie.transform.parent = transform;
		newbie.CharacterName = "New Character";
	}
}
