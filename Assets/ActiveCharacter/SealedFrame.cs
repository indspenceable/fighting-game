﻿using UnityEngine;
using System.Collections;

public class SealedFrame : MonoBehaviour {
	public SealedAnimation mySealedAnimation;
	public FrameData data;
	public Hitbox[] hitboxes;
	public Hurtbox[] hurtboxes;
	public SpriteRenderer[] sprites;

	public void SetupHitboxes() {
		foreach(Hitbox hb in hitboxes) {
			hb.Setup();
		}
	}
}
