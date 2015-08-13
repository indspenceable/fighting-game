using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class MatchManager : MonoBehaviour
{
	[SerializeField]
	CharacterDefinition playerCharacterToSpawn;
	[SerializeField]
	CharacterDefinition aiCharacterToSpawn;
	public List<ActiveCharacter> characters;

	void Start ()
	{
		QualitySettings.vSyncCount = 0;  // VSync must be disabled
		Application.targetFrameRate = -1;
		
		characters = new List<ActiveCharacter> ();
		if (playerCharacterToSpawn != null) {
			ActiveCharacter ac = SpawnCharacter<CharacterController.PlayerCharacterController> (this, playerCharacterToSpawn);
			ac.transform.Translate (-2, 0, 0);
			characters.Add (ac);
		}
		if (aiCharacterToSpawn != null) {
			ActiveCharacter ac = SpawnCharacter<CharacterController.DoNothingCharacterController> (this, aiCharacterToSpawn);
			ac.transform.Translate (2, 0, 0);
			characters.Add (ac);
		}
	}


	Queue<float> Deltas = new Queue<float>();
	void OnDrawGizmos() {
		float x = 0f;

		foreach(float f in Deltas) {
			Camera camera = Camera.current;
			if (f > TICK) {
				Gizmos.color = Color.red;
			} else {
				Gizmos.color = Color.green;
			}
			Vector3 start = camera.ViewportToWorldPoint(new Vector3(x, 0f, camera.nearClipPlane));
			Vector3 ending = camera.ViewportToWorldPoint(new Vector3(x, f, camera.nearClipPlane));
			Gizmos.DrawLine(start, ending);
			x+=0.001f;
		}
	}


	[SerializeField] float TICK = 1f/20f;
	float dt = 0f;
	float LAST_DT;
	void Update()
	{
		if (Deltas.Count > 100) {
			Deltas.Dequeue();
		}
		dt += Time.deltaTime;
		if (dt > TICK) {
			Deltas.Enqueue(Time.time - LAST_DT);
			LAST_DT = Time.time;

			dt -= TICK;
			foreach (ActiveCharacter ac in characters) {
				ac.NextFrame(Time.fixedDeltaTime);
			}
			foreach (ActiveCharacter ac in characters) {
				ac.DeliverHits();
			}
			foreach (ActiveCharacter ac in characters) {
				ac.ApplyHits();
			}

			foreach (ActiveCharacter ac in characters) {
				ac.ReduceHitlag();
			}
		}
	}



	// * * * * * * * * * * * * //
	// Character Spawning Code //
	// * * * * * * * * * * * * //

	private static ActiveCharacter SpawnCharacter<T> (MatchManager match, CharacterDefinition origin)
		where T : CharacterController
	{
		GameObject go = Instantiate (origin.gameObject) as GameObject;
		go.AddComponent<T> ();
		CharacterDefinition characterDefinition = go.GetComponent<CharacterDefinition> ();
		ActiveCharacter ac = go.AddComponent<ActiveCharacter> ();
		foreach (AnimationID id in Enum.GetValues(typeof(AnimationID)).Cast<AnimationID>()) {
			CharacterAnimation animation = characterDefinition.GetAnimation (id);
			SealedAnimation sa = AnimationFromStatic (animation);
			sa.SetFrames(animation.GetFrames ().Select (f => FrameToStatic (sa, f)).ToArray ());
			Destroy (animation);
			ac.GetComponent<ActiveAnimator> ().Add (id, sa);
		}
		Destroy (characterDefinition);
		ac.Boot(match);
		return ac;
	}

	public static SealedAnimation AnimationFromStatic (CharacterAnimation characterAnimation)
	{
		SealedAnimation sealedAnimation = characterAnimation.gameObject.AddComponent<SealedAnimation> ();
		sealedAnimation.loopBackFrame = characterAnimation.loopBackFrame;
		sealedAnimation.id = characterAnimation.id;
		sealedAnimation.followingAnimation = characterAnimation.followingAnimation;
		sealedAnimation.ticksPerFrame = characterAnimation.ticksPerFrame;
		return sealedAnimation;
	}

	private static SealedFrame FrameToStatic (SealedAnimation animation, Frame f)
	{
		SealedFrame sf = f.gameObject.AddComponent<SealedFrame> ();
		sf.data = f.data;
		sf.hitboxes = f.GetHitboxes ().ToArray ();
		sf.hurtboxes = f.GetHurtboxes ().ToArray ();
		sf.sprites = f.GetSprites ().ToArray ();
		sf.animation = animation;
		foreach (Transform t in sf.GetComponentsInChildren<Transform>(true)) {
			t.gameObject.SetActive (true);
		}
		sf.gameObject.SetActive (false);
		foreach (Hitbox hb in sf.hitboxes) {
			hb.GetComponent<Collider2D> ().enabled = true;
			hb.myCollider.isTrigger = true;
		}
		foreach (Hurtbox hb in sf.hurtboxes) {
			hb.GetComponent<Collider2D> ().enabled = true;
			hb.myCollider.isTrigger = true;
		}
		Destroy (f);
		return sf;
	}
}
