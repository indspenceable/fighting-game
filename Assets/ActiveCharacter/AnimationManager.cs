using UnityEngine;
using System.Collections;

[RequireComponent (typeof(ActiveAnimator))]
public class AnimationManager : MonoBehaviour {
	ActiveAnimator animationBackend;
	public void Init() {
		animationBackend = GetComponent<ActiveAnimator>();
		animationBackend.EnableAllAnimations();
	}

	public float xv, yv;
	public bool grounded;
	public bool crouching;
	public bool attacking;
	/*
	public SealedFrame Next() {
		AnimationID id = AnimationID.IDLE;
		if (grounded) {
			if (attacking) {
				id = AnimationID.ATTACK;
			} else if (crouching) {
				id = AnimationID.CROUCH;
			} else if (xv != 0) {
				id = AnimationID.WALK;
			} else {
				id = AnimationID.IDLE;
			}
		} else {
			if (yv < 0) {
				id = AnimationID.FALL;
			} else {
				id = AnimationID.JUMP;
			}
		}


		if (xv > 0 && grounded) {
			animationBackend.SetFacing(Facing.RIGHT);
		} else if (xv < 0 && grounded) {
			animationBackend.SetFacing(Facing.LEFT);
		}
		return animationBackend.StartAnimation(id);
	}
	*/
}
