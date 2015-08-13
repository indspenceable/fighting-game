using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(ActiveAnimator))]
[RequireComponent (typeof(CharacterController))]
public class ActiveCharacter : MonoBehaviour
{
	private ActiveAnimator animator;
	private CharacterController controls;
	public MatchManager match;
	
	public void Boot (MatchManager matchManager)
	{
		gameObject.SetActive (true);
		this.match = matchManager;
		animator = gameObject.GetComponent<ActiveAnimator> ();
		animator.EnableAllAnimations ();
		currentFrame = animator.StartAnimationOrNext (AnimationID.IDLE);

		controls = GetComponent<CharacterController> ();
	}
	
	Vector2 velocity = new Vector2 (0, 0);
	bool grounded;
	bool crouching;
	bool attacking;
	int hitlag;
	Facing direction;
	[SerializeField]
	SealedFrame currentFrame;
	List<Hitbox> hitsThisFrame = new List<Hitbox> ();
	float traction = 0.8f;

	void ApplyTraction (AnimationID cid)
	{
		if (grounded &&
			((cid != AnimationID.DASH && cid != AnimationID.RUN) || 
			(velocity.x > 0 && controls.Horizontal () <= 0 ||
			velocity.x < 0 && controls.Horizontal () >= 0))) {
			velocity.x = velocity.x * traction;
		}
		if (Mathf.Abs (velocity.x) < 0.1f) {
			velocity.x = 0f;
		}
	}

	float GROUNDED_VELOCITY_MAX = 8f;

	float LimitToMaxGroundedVelocity (float currentVelocity)
	{
		if (currentVelocity > GROUNDED_VELOCITY_MAX) {
			return GROUNDED_VELOCITY_MAX;
		} else if (currentVelocity < -GROUNDED_VELOCITY_MAX) {
			return -GROUNDED_VELOCITY_MAX;
		}
		return currentVelocity;
	}

	float AIR_VELOCITY_MAX = 4f;

	float LimitToMaxAirVelocity (float currentVelocity)
	{
		if (currentVelocity > AIR_VELOCITY_MAX) {
			return AIR_VELOCITY_MAX;
		} else if (currentVelocity < -AIR_VELOCITY_MAX) {
			return -AIR_VELOCITY_MAX;
		}
		return currentVelocity;
	}

	// * Deal with Damage! *//
	[SerializeField]
	float pct = 0f;

	public void ApplyHits ()
	{
		if (hitsThisFrame.Count > 0) {
			Hitbox best = hitsThisFrame [0];
			foreach (Hitbox hb in hitsThisFrame) {
				if (hb.pct > best.pct) {
					best = hb;
				}
			}
			// Do the best one!
			velocity = best.GetDirection () * (1f + pct);
			pct += best.pct / 100f;
			best.ApplyHitlag ();
			hitsThisFrame.Clear ();
		}
	}

	public void AddHitlag (int frames)
	{
		hitlag += frames;
	}

	public void TakeHit(Hitbox hitBox) {
		if (InHitlag()) {
			return;
		}
		hitsThisFrame.Add(hitBox);
	}

	public void DeliverHits ()
	{
		if (InHitlag()) {
			return;
		}

		int i = 0;
		foreach (ActiveCharacter otherCharacter in match.characters) {
			if (otherCharacter == this)
				continue;
			foreach (Hitbox hitbox in currentFrame.hitboxes) {
				foreach (Hurtbox hurtbox in otherCharacter.currentFrame.hurtboxes) {
					if (hitbox.myCollider.bounds.Intersects (hurtbox.myCollider.bounds)) {
						otherCharacter.TakeHit(hitbox);
					}
				}
			}
		}
	}

	bool CanFastFall ()
	{
		return false;
	}

	bool CanAttack (AnimationID cid)
	{
		return grounded;
	}

	bool CanMove (AnimationID cid)
	{
		return (cid != AnimationID.ATTACK) && (cid != AnimationID.CROUCH);
	}

	bool CanJump (AnimationID cid)
	{
		if (grounded) {
			return (cid != AnimationID.ATTACK);
		} else {
			// No air jumps for now
			return false;
		}
	}

	bool IsLanding (AnimationID cid)
	{
		return ((cid == AnimationID.FALL) || (cid == AnimationID.JUMP));
	}
		
	void ApplyLandingLagAndTransitionAnimation (AnimationID cid)
	{
		animator.StartAnimationOrNext (AnimationID.IDLE);
	}

	bool MidAttack (AnimationID cid)
	{
		return cid == AnimationID.ATTACK;
	}

	SealedFrame AnimateToNextFrame (float dt)
	{
		AnimationID cid = currentFrame.animation.id;
		if (grounded && IsLanding (cid)) {
			ApplyLandingLagAndTransitionAnimation (cid);
		} else if (grounded) {
			// When we're grounded, only apply movement in cases where we should
			if (MidAttack (cid)) {
				//do nothing!
			} else if (controls.Attacking () && CanAttack (cid)) {
				// Do Ground Attack
				return animator.StartAnimationOrNext (AnimationID.ATTACK);
			} else if (controls.Jumping () && CanJump (cid)) {
				velocity.y += 5f;
				return animator.StartAnimationOrNext (AnimationID.JUMP);
			} else if (controls.Crouching ()) {
				return animator.StartAnimationOrNext (AnimationID.CROUCH);
			} else if (cid == AnimationID.CROUCH) {
				return animator.StartAnimationOrNext (AnimationID.UNCROUCH);
			} else if (cid == AnimationID.PIVOT) {
				// Woop
			} else {
				if (controls.Horizontal () > 0) {
					if (animator.GetFacing () == Facing.RIGHT || velocity.x >= 0 || cid == AnimationID.DASH) {
						if (cid == AnimationID.DASH && animator.GetFacing () == Facing.LEFT) {
							// Hackity hack - jump to idle so we're forced to restart the dash animation.
							animator.StartAnimationOrNext (AnimationID.IDLE);
						}
						animator.SetFacing (Facing.RIGHT);
						velocity.x = LimitToMaxGroundedVelocity (velocity.x + controls.Horizontal () * GroundVelocity (dt));
						return animator.StartAnimationOrNext (AnimationID.DASH, AnimationID.RUN);
					} else {
						animator.SetFacing (Facing.RIGHT);
						return animator.StartAnimationOrNext (AnimationID.PIVOT);
					}
				} else if (controls.Horizontal () < 0) {
					if (animator.GetFacing () == Facing.LEFT || velocity.x <= 0 || cid == AnimationID.DASH) {
						if (cid == AnimationID.DASH && animator.GetFacing () == Facing.RIGHT) {
							// Hackity hack - jump to idle so we're forced to restart the dash animation.
							animator.StartAnimationOrNext (AnimationID.IDLE);
						}
						animator.SetFacing (Facing.LEFT);
						velocity.x = LimitToMaxGroundedVelocity (velocity.x + controls.Horizontal () * GroundVelocity (dt));
						return animator.StartAnimationOrNext (AnimationID.DASH, AnimationID.RUN);
					} else {
						animator.SetFacing (Facing.LEFT);
						return animator.StartAnimationOrNext (AnimationID.PIVOT);
					}
				} else {
					if (cid == AnimationID.RUN || cid == AnimationID.SKID) {
						return animator.StartAnimationOrNext (AnimationID.SKID);
					}
				}
			}
		} else { // Airborne
			// In the air, can always move.
			velocity.x = LimitToMaxAirVelocity (velocity.x + controls.Horizontal () * AirVelocity (dt));

			
			if (controls.Attacking () && CanAttack (cid)) {
				// Do Air Attack
			} else if (controls.FastFall () && CanFastFall ()) {
				// FastFall
			} else {
				// Air Jump
				if (controls.Jumping () && CanJump (cid)) {
					velocity.y += 5f;
				}
				if (velocity.y > 0) {
					return animator.StartAnimationOrNext (AnimationID.FALL);
				} else {
					return animator.StartAnimationOrNext (AnimationID.JUMP);
				}
			}
		}
		return animator.NextFrame ();
	}

	public bool InHitlag() {
		return (hitlag > 0);
	}

	public void NextFrame (float dt)
	{
		if (InHitlag()) {
			return;
		}
		SetGroundedAndSnapToSurface ();
		ApplyGravity ();
		currentFrame = AnimateToNextFrame (dt);
		ApplyTraction (currentFrame.animation.id);
		PhysicsUpdate ();
	}

	public void PhysicsUpdate ()
	{
		transform.Translate (velocity * Time.deltaTime);
		SetGroundedAndSnapToSurface ();
	}
			
	float AirVelocity (float dt)
	{
		return dt * 30f;
	}

	float GroundVelocity (float dt)
	{
		return dt * 9999;
	}

	void SetGroundedAndSnapToSurface ()
	{
		if (transform.position.y <= 0f) {
			grounded = true;
			transform.position = new Vector3 (transform.position.x, 0f);
			if (velocity.y < 0) {
				velocity.y = 0;
			}
		} else {
			grounded = false;
		}
	}

	void ApplyGravity ()
	{
		velocity.y -= 0.3f;
	}

	public void ReduceHitlag() {
		if (InHitlag()) {
			hitlag -= 1;
		}
	}
}
