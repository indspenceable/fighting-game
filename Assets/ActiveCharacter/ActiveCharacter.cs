using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(AnimationManager))]
[RequireComponent (typeof(CharacterController))]
public class ActiveCharacter : MonoBehaviour {
	private AnimationManager animator;
	private CharacterController controls;
	public MatchManager match;
	
	void Start() {
		animator = gameObject.GetComponent<AnimationManager>();
		animator.Init();
		currentFrame = animator.Next();

		controls = GetComponent<CharacterController>();
	}

	Vector2 velocity = new Vector2(0, 0);
	bool grounded;
	bool crouching;
	bool attacking;
	[SerializeField] SealedFrame currentFrame;
	List<Hitbox> hitsThisFrame = new List<Hitbox>();

	void NormalizeVelocity() {
		if (grounded) {
			velocity.x = velocity.x * 0.4f; 
			if (Mathf.Abs(velocity.x) < 0.1f) {
				velocity.x = 0f;
			}
		} else {
			velocity.x = velocity.x * 0.8f; 
			if (Mathf.Abs(velocity.x) < 0.1f) {
				velocity.x = 0f;
			}
		}
	}

	public void NextFrame() {
		animator.xv = velocity.x;
		animator.yv = velocity.y;
		animator.grounded = grounded;
		animator.crouching = crouching;
		animator.attacking = attacking;
		currentFrame = animator.Next();
	}
	public void ApplyHits() {
		if (hitsThisFrame.Count > 0) {
			Hitbox best = hitsThisFrame[0];
			foreach (Hitbox hb in hitsThisFrame) {
				if (hb.pct > best.pct) {
					best = hb;
				}
			}
			// Do the best one!
			velocity += best.GetDirection();
			hitsThisFrame.Clear();
		}
	}

	public void DeliverHits() {
		int i = 0;
		foreach(ActiveCharacter otherCharacter in match.characters) {
			if (otherCharacter == this)
				continue;
			foreach(Hitbox hitbox in currentFrame.hitboxes) {
				foreach(Hurtbox hurtbox in otherCharacter.currentFrame.hurtboxes) {
					if (hitbox.myCollider.bounds.Intersects(hurtbox.myCollider.bounds)) {
						otherCharacter.hitsThisFrame.Add(hitbox);
					}
				}
			}
		}
	}

	void Update() {
		SetGroundedAndSnapToSurface();
		attacking = false;

		// Oh man, check if we can actually move!
		velocity.x += controls.Horizontal() * HorizontalVelocity();

		if (!animator.attacking) {
			crouching = controls.Crouching();
			if (crouching) {
				velocity.x = 0;
			}

			if (controls.Jumping() && grounded) {
				velocity.y = 6;
			}

			if (grounded && controls.Attacking()) {
				attacking = true;
			}
		}

		NormalizeVelocity();
		transform.Translate(velocity*Time.deltaTime);
	}

	float HorizontalVelocity() {
		if (grounded) {
			return 5f;
		} else {
			return 1f;
		}
	}

	void SetGroundedAndSnapToSurface() {
		if (transform.position.y <= 0f) {
			grounded = true;
			transform.position = new Vector3(transform.position.x, 0f);
			if (velocity.y < 0) {
				velocity.y = 0;
			}
		} else {
			grounded = false;
			velocity.y -= 0.3f;
		}
	}

}
