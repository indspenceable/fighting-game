using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class MatchManager : MonoBehaviour {
	[SerializeField] CharacterDefinition playerCharacterToSpawn;
	[SerializeField] CharacterDefinition aiCharacterToSpawn;

	public List<ActiveCharacter> characters;

	void Start() {
		characters = new List<ActiveCharacter>();
		if (playerCharacterToSpawn != null) {
			ActiveCharacter ac = SpawnCharacter<CharacterController.PlayerCharacterController>(this, playerCharacterToSpawn);
			ac.transform.Translate(-2, 0, 0);
			characters.Add(ac);
		}
		if (aiCharacterToSpawn != null) {
			ActiveCharacter ac = SpawnCharacter<CharacterController.DoNothingCharacterController>(this, aiCharacterToSpawn);
			ac.transform.Translate(2, 0, 0);
			characters.Add(ac);
		}
	}

	float dt;
	float TICK = 1f/50f;
	void FixedUpdate() {
//		dt += Time.deltaTime;
//		if (dt > TICK) {
			dt = 0f;


			foreach(ActiveCharacter ac in characters) {
				ac.NextFrame();
			}
			foreach(ActiveCharacter ac in characters) {
				ac.DeliverHits();
			}
			foreach(ActiveCharacter ac in characters) {
				ac.ApplyHits();
			}
//		}
	}



	// * * * * * * * * * * * * //
	// Character Spawning Code //
	// * * * * * * * * * * * * //

	private static ActiveCharacter SpawnCharacter<T>(MatchManager match, CharacterDefinition origin)
		where T : CharacterController {
		GameObject go = Instantiate(origin.gameObject) as GameObject;
		go.AddComponent<T>();
		CharacterDefinition characterDefinition = go.GetComponent<CharacterDefinition>();
		ActiveCharacter ac = go.AddComponent<ActiveCharacter>();
		foreach (AnimationID id in Enum.GetValues(typeof(AnimationID)).Cast<AnimationID>()) {
			CharacterAnimation animation = characterDefinition.GetAnimation(id);
			SealedAnimation sa = AnimationFromStatic(animation);
			sa.frames = animation.GetFrames().Select(f => FrameToStatic(sa, f)).ToArray();
			Destroy(animation);
			ac.GetComponent<ActiveAnimator>().Add(id, sa);
		}
		Destroy(characterDefinition);
		ac.gameObject.SetActive(true);
		ac.match = match;
		return ac;
	}

	public static SealedAnimation AnimationFromStatic(CharacterAnimation characterAnimation) {
		SealedAnimation sealedAnimation = characterAnimation.gameObject.AddComponent<SealedAnimation>();
		sealedAnimation.loopBackFrame = characterAnimation.loopBackFrame;
		sealedAnimation.id = characterAnimation.id;
		sealedAnimation.followingAnimation = characterAnimation.followingAnimation;
		return sealedAnimation;
	}


	private static SealedFrame FrameToStatic(SealedAnimation animation, Frame f) {
		SealedFrame sf = f.gameObject.AddComponent<SealedFrame>();
		sf.data = f.data;
		sf.hitboxes = f.GetHitboxes().ToArray();
		sf.hurtboxes = f.GetHurtboxes().ToArray();
		sf.sprites = f.GetSprites().ToArray();
		sf.animation = animation;
		foreach(Transform t in sf.GetComponentsInChildren<Transform>(true)) {
			t.gameObject.SetActive(true);
		}
		sf.gameObject.SetActive(false);
		foreach(Hitbox hb in sf.hitboxes) {
			hb.GetComponent<Collider2D>().enabled = true;
			hb.myCollider.isTrigger = true;
		}
		foreach(Hurtbox hb in sf.hurtboxes) {
			hb.GetComponent<Collider2D>().enabled = true;
			hb.myCollider.isTrigger = true;
		}
		Destroy(f);
		return sf;
	}
}
