using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CharacterAnimation : MonoBehaviour {
	public AnimationID id;
	public int loopBackFrame = 0;
	public AnimationID followingAnimation;

	public List<Frame> GetFrames() {
		List<Frame> frames = new List<Frame>(GetComponentsInChildren<Frame>(true).OrderBy(f => f.index));
		for (int i = 0; i < frames.Count; i+= 1) {
			frames[i].index = i;
			frames[i].name = "Frame " + i;
		}
		return frames;
	}

	public void EnsureAtLeastOneFrame() {
		if (GetFrames().Count == 0) {
			BuildFrameWithIndex(0);
		}
	}

	public void RemoveFrame (int currentlySelectedFrameIndex)
	{
		foreach (Frame f in GetFrames().Where(f => f.index == currentlySelectedFrameIndex)) {
			DestroyImmediate(f.gameObject);
		}
	}

	public void AddEmptyFrameAfter (int currentlySelectedFrameIndex)
	{
		foreach (Frame f in GetFrames().Where(f => f.index > currentlySelectedFrameIndex)) {
			f.index += 1;
		}
		BuildFrameWithIndex(currentlySelectedFrameIndex + 1);
	}

	public void DuplicateFrame (int currentlySelectedFrameIndex)
	{
		List<Frame> frames = GetFrames();
		foreach (Frame f in frames.Where(f => f.index > currentlySelectedFrameIndex)) {
			f.index += 1;
		}
		GameObject go = Instantiate(frames[currentlySelectedFrameIndex].gameObject) as GameObject;
		go.transform.parent = transform;
		Frame newFrame = go.GetComponent<Frame>();
		newFrame.index += 1;
	}

	void BuildFrameWithIndex(int index) {
		Frame frame = new GameObject("Frame").AddComponent<Frame>();
		frame.transform.parent = transform;
		frame.index = index;
	}
}
