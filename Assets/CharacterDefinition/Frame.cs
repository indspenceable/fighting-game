using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Frame : MonoBehaviour {
	public int index;
	public FrameData data;

	public List<Hitbox> GetHitboxes() {
		return new List<Hitbox>(GetComponentsInChildren<Hitbox>(true));
	}
	public List<Hurtbox> GetHurtboxes() {
		return new List<Hurtbox>(GetComponentsInChildren<Hurtbox>(true));
	}
	public List<SpriteRenderer> GetSprites() {
		return new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>(true));
	}

	public void AddHitbox() {
		GameObject go = new GameObject("Hitbox");
		go.AddComponent<Hitbox>();
		go.transform.parent = transform;
	}
	public void AddHurtbox() {
		GameObject go = new GameObject("Hurtbox");
		go.AddComponent<Hurtbox>();
		go.transform.parent = transform;
	}

	public void AddSprite(Sprite sprite, Material spriteMaterial) {
		GameObject go = new GameObject("Sprite");
		SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = sprite;
		spriteRenderer.material = spriteMaterial;
		go.transform.parent = transform;
	}
}
