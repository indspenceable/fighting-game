using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterDefinition : MonoBehaviour {
	public string CharacterName;

	public List<CharacterAnimation> GetAllAnimations() {
		return new List<CharacterAnimation>(GetComponentsInChildren<CharacterAnimation>(true));
	}

	private CharacterAnimation _GetAnimation(AnimationID id) {
		foreach (CharacterAnimation anim in GetComponentsInChildren<CharacterAnimation>(true)) {
			if (anim.id == id) {
				return anim;
			}
		}
		return null;
	}

	public CharacterAnimation GetAnimation(AnimationID id) {
		CharacterAnimation anim = _GetAnimation(id);
		if (anim == null) {
			anim = new GameObject(id.ToString()).AddComponent<CharacterAnimation>();
			anim.transform.parent = transform;
			anim.id = id;
			anim.followingAnimation = id;
		}
		return anim;
	}
}
