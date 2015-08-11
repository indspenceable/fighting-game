using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class CharacterEditor : EditorWindow {
	[MenuItem ("Window/Character Editor")]

	public static void ShowWindow () {
		EditorWindow.GetWindow(typeof(CharacterEditor));
	}

	Color CURRENTLY_SELECTED_COLOR = Color.cyan;

	public void Update()
	{
		// This is necessary to make the framerate normal for the editor window.
		Repaint();
	}

	CharacterDefinition selectedCharacter = null;

	GameObject previouslySelectedObject = null;

	void OnGUI() {
		if (Selection.activeGameObject != previouslySelectedObject) {
			SetVariablesToSelectedObject();
		}

		RenderSelectCharacterBar();
		GUILayout.Space(10f);
		if (selectedCharacter != null) {
			RenderSelectedCharacterDetails();
			EnableAndDisableAnimationGameObjects();
			RenderCurrentlySelectedAnimationDetails();

			DisableAllFrames();
			RenderCurrentAnimationFrameDetails();
		}

		previouslySelectedObject = Selection.activeGameObject;
	}

	void SetVariablesToSelectedObject() {
		GameObject active = Selection.activeGameObject;
		if (active == null) {
			selectedCharacter = null;
			return;
		}
		Frame[] f = active.GetComponentsInParent<Frame>(true);
		if (f.Any()) {
			currentlySelectedFrameIndex = f[0].index;
		}
		CharacterAnimation[] a = active.GetComponentsInParent<CharacterAnimation>(true);
		if (a.Any()) {
			AnimationID[] idsArray = Enum.GetValues(typeof(AnimationID)).Cast<AnimationID>().ToArray();
			selectedAnimationIdIndex = new List<AnimationID>(idsArray).IndexOf(a[0].id);
		}
		CharacterDefinition[] c = active.GetComponentsInParent<CharacterDefinition>(true);
		if (c.Any()) {
			selectedCharacter = c[0];
		}
	}

	void RenderSelectCharacterBar() {
		List<CharacterDefinition> characters = CharacterManager.Instance().GetCharacters();
		if (GUILayout.Button("New character")) {
			CharacterManager.Instance().BuildEmptyCharacter();
		}
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("---")) {
			selectedCharacter = null;
		}
		foreach (CharacterDefinition c in characters) {
			Color oldColor = GUI.color;
			if (c == selectedCharacter) {
				GUI.color = CURRENTLY_SELECTED_COLOR;
			}
			if (GUILayout.Button(c.CharacterName)) {
				selectedCharacter = c;
			}
			GUI.color = oldColor;
		}
		EditorGUILayout.EndHorizontal();
	}



	int selectedAnimationIdIndex = 0;
	CharacterAnimation animation;
	void RenderSelectedCharacterDetails() {
		selectedCharacter.CharacterName = GUILayout.TextField(selectedCharacter.CharacterName);
		AnimationID[] idsArray = Enum.GetValues(typeof(AnimationID)).Cast<AnimationID>().ToArray();
		selectedAnimationIdIndex = EditorGUILayout.Popup(selectedAnimationIdIndex, idsArray.Select (id => id.ToString()).ToArray());
		animation = selectedCharacter.GetAnimation(idsArray[selectedAnimationIdIndex]);
	}

	void EnableAndDisableAnimationGameObjects() {
		foreach (CharacterAnimation characterAnimation in selectedCharacter.GetAllAnimations()) {
			characterAnimation.gameObject.SetActive(false);
		}
	}

	int currentlySelectedFrameIndex = 0;

	void NormalizeAnimationFrame() {
		int count = animation.GetFrames().Count;
		if (currentlySelectedFrameIndex >= count) {
			currentlySelectedFrameIndex = count - 1;
		}
		if (currentlySelectedFrameIndex < 0) {
			currentlySelectedFrameIndex = 0;
		}
	}

	void RenderCurrentlySelectedAnimationDetails() {
		animation.gameObject.SetActive(true);

		EditorGUILayout.BeginHorizontal();
		List<AnimationID> idsArray = new List<AnimationID>(Enum.GetValues(typeof(AnimationID)).Cast<AnimationID>().ToArray());
		int animationID = idsArray.IndexOf(animation.followingAnimation);
		EditorGUILayout.LabelField("Transition to:");
		int newID = EditorGUILayout.Popup(animationID, idsArray.Select (id => id.ToString()).ToArray());

		animation.followingAnimation = idsArray[newID];
		if (animation.followingAnimation == animation.id) {
			animation.loopBackFrame = EditorGUILayout.IntField("Loop back to: ", animation.loopBackFrame);
		}
		EditorGUILayout.EndHorizontal();
		RenderAnimationControlsBar(animation.GetFrames().Count);
		RenderAnimationFrames();
	}
	void RenderAnimationControlsBar(int count) {
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("<<")) {
			currentlySelectedFrameIndex = 0;
		}
		if (GUILayout.Button("<")) {
			currentlySelectedFrameIndex -= 1;
		}
		if (GUILayout.Button("-")) {
			animation.RemoveFrame(currentlySelectedFrameIndex);
		}
		if (GUILayout.Button("+")) {
			animation.AddEmptyFrameAfter(currentlySelectedFrameIndex);
			currentlySelectedFrameIndex += 1;
		}
		if (GUILayout.Button("x2")) {
			animation.DuplicateFrame(currentlySelectedFrameIndex);
			currentlySelectedFrameIndex += 1;
		}
		if (GUILayout.Button(">")) {
			currentlySelectedFrameIndex += 1;
		}
		if (GUILayout.Button(">>")) {
			currentlySelectedFrameIndex = count - 1;
		}
		GUILayout.EndHorizontal();
	}


	void RenderAnimationFrames() {
		GUILayout.BeginHorizontal();
		animation.EnsureAtLeastOneFrame();
		int i = 0;
		foreach (Frame frame in animation.GetFrames()) {
			Color oldColor = GUI.color;
			if (i == currentlySelectedFrameIndex) {
				GUI.color = CURRENTLY_SELECTED_COLOR;
			}
			if (GUILayout.Button(frame.index.ToString())) {
				currentlySelectedFrameIndex = frame.index;
				Selection.activeGameObject = frame.gameObject;
			}
			GUI.color = oldColor;

			i += 1;
		}
		GUILayout.EndHorizontal();
	}

	void DisableAllFrames() {
		foreach(Frame f in animation.GetFrames()) {
			f.gameObject.SetActive(false);
		}
	}

	void RenderCurrentAnimationFrameDetails() {
		NormalizeAnimationFrame();
		Frame frame = animation.GetFrames().ToArray()[currentlySelectedFrameIndex];
		frame.gameObject.SetActive(true);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Select")) {
			Selection.activeGameObject = frame.gameObject;
		}
		if (GUILayout.Button("Add Hitbox")) {
			frame.AddHitbox();
		}
		if (GUILayout.Button("Add Hurtbox")) {
			frame.AddHurtbox();
		}
		Sprite sprite = EditorGUILayout.ObjectField(null, typeof(Sprite), false) as Sprite;
		if (sprite != null) {
			frame.AddSprite(sprite, CharacterManager.Instance().spriteMaterial);
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		GUILayout.Label("Hitboxes");
		foreach(Hitbox hb in frame.GetHitboxes()) {
			if (GUILayout.Button ("select")) {
				Selection.activeGameObject = hb.gameObject;
			}
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.Label("Hurtboxes");
		foreach(Hurtbox hb in frame.GetHurtboxes()) {
			if (GUILayout.Button ("select")) {
				Selection.activeGameObject = hb.gameObject;
			}
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.Label("Sprites");
		foreach(SpriteRenderer spriteRenderer in frame.GetSprites()) {
			GUILayout.BeginHorizontal();
			if (GUILayout.Button ("select")) {
				Selection.activeGameObject = spriteRenderer.gameObject;
			}
			spriteRenderer.sprite = EditorGUILayout.ObjectField(spriteRenderer.sprite, typeof(Sprite), false) as Sprite;
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

}
