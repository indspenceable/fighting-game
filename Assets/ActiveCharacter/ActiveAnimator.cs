using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActiveAnimator : MonoBehaviour {
	bool flipped = true;
	void Flip(){
		// Switch the way the player is labelled as facing
		flipped = !flipped;
		
		// Multiply the player's x local scale by -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	public bool IsFlipped() {
		return !flipped;
	}
	public Facing GetFacing() {
		return direction;
	}

	private Dictionary<AnimationID, SealedAnimation> animationIDToAnimationObject;
	public void Add(AnimationID id, SealedAnimation sa) {
		if (animationIDToAnimationObject == null) {
			animationIDToAnimationObject = new Dictionary<AnimationID, SealedAnimation>();
		}
		animationIDToAnimationObject.Add(id, sa);
	}

	[SerializeField] AnimationID currentAnimation;
	[SerializeField] int currentFrame;
	public SealedFrame StartAnimationOrNext(AnimationID animationId, AnimationID? backup=null) {
		DisableCurrentFrame();
		
		if (currentAnimation != animationId && (!backup.HasValue || backup.Value != currentAnimation) ) {
			currentAnimation = animationId;
			currentFrame = 0;
			FlipIfNeeded();
			return EnableCurrentFrame();
		} else {
			return NextFrame();
		}
	}


	Facing direction = Facing.LEFT;

	public void SetFacing(Facing direction) {
		this.direction = direction;
	}

	public SealedFrame NextFrame() {
		DisableCurrentFrame();
		SealedAnimation sa = GetAnimationById(currentAnimation);
		int numberOfFrames = sa.NumberOfFrames();
		if (currentFrame + 1 == numberOfFrames) {
			if (sa.followingAnimation == sa.id) {
				currentFrame = sa.loopBackFrame;
			} else {
				return StartAnimationOrNext(sa.followingAnimation);
			}
		} else {
			currentFrame += 1;
		}
		FlipIfNeeded();
		return EnableCurrentFrame();
	}

	private void FlipIfNeeded() {
		if (GetCurrentFrame().data.direction == direction ^ flipped) {
			Flip ();
		}
	}

	private SealedFrame GetCurrentFrame() {
		SealedAnimation sa = GetAnimationById(currentAnimation);
		return sa.GetFrame(currentFrame);
	}

	private AnimationID CurrentAnimation() {
		return currentAnimation;
	}


	public void EnableAllAnimations() {
		foreach (SealedAnimation sa in animationIDToAnimationObject.Values) {
			sa.MakeMeActive();
		}
	}

	private void DisableCurrentFrame() {
		GetCurrentFrame().gameObject.SetActive(false);
	}

	private SealedFrame EnableCurrentFrame() {
		GetCurrentFrame().gameObject.SetActive(true);
		return GetCurrentFrame();
	}


	private SealedAnimation GetAnimationById(AnimationID id) {
		return animationIDToAnimationObject[id];
	}
}
