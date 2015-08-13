using UnityEngine;
using System.Collections;

public class SealedAnimation : MonoBehaviour {
	public AnimationID id;
	public AnimationID followingAnimation;
	public int loopBackFrame;
	public int ticksPerFrame;

	private SealedFrame[] frames;
	public void SetFrames(SealedFrame[] frames) {
		this.frames = frames;
	}
	public int NumberOfFrames() {
		return ticksPerFrame * frames.Length;
	}
	public SealedFrame GetFrame(int count) {
		return frames[count / ticksPerFrame];
	}

	public void MakeMeActive() {
		gameObject.SetActive(true);
		foreach(SealedFrame sf in frames) {
			sf.gameObject.SetActive(false);
			sf.SetupHitboxes();
		}
	}
}
