using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
/*

[CustomEditor(typeof(CharacterDefinition))] 
public class CharacterEditorOld : Editor {

	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(CharacterEditor));
	}

	public void Update()
	{
		// This is necessary to make the framerate normal for the editor window.
		Repaint();
	}

	CharacterDefinition oldCharacter;
	int currentSelectedAnimation = 0;
	int selectedAnimationFrame = 0;

	void OnSceneGUI () {

		// short circuit if we don't have a character definition selected
		if (Selection.activeGameObject == null)
			return;

		CharacterDefinition character = Selection.activeGameObject.GetComponent<CharacterDefinition>();
		if (oldCharacter != character) {
			selectedAnimationFrame = -1;
			oldCharacter = character;
		}
		if (character == null)
			return;

		if (character.animations == null) {
			character.animations = new List<CharacterDefinition.AnimationData>();
		}
			
		EditorUtility.SetDirty(character);
		Handles.BeginGUI();

		List<CharacterDefinition.AnimationID> animationNameEnumList = new List<CharacterDefinition.AnimationID>();
		List<string> animationNames = new List<string>();
		foreach (CharacterDefinition.AnimationID animationID in Enum.GetValues(typeof(CharacterDefinition.AnimationID))) {
			animationNameEnumList.Add(animationID);
			animationNames.Add(animationID.ToString());
		}
		int selected = EditorGUILayout.Popup(currentSelectedAnimation, animationNames.ToArray());
		if (selected != currentSelectedAnimation) {
			selectedAnimationFrame = -1;
		}
		CharacterDefinition.AnimationID id = animationNameEnumList[selected];
		updateGUIForSelectedAnimation(character.findOrCreateAnimationByID(id));
		currentSelectedAnimation = selected;

		Handles.EndGUI();
	}

	void updateGUIForSelectedAnimation(CharacterDefinition.AnimationData animationData) {
		EditorGUILayout.BeginHorizontal();
		int i = 0;
		foreach (CharacterDefinition.Frame frame in animationData.frames) {
			if (GUILayout.Button("Frame: " + i)) {
				selectedAnimationFrame = i;
				// TODO make this display a rendered version of that frame.
			}
			i+=1;
		}
		EditorGUILayout.EndHorizontal();

		if (selectedAnimationFrame >= animationData.frames.Count)
			selectedAnimationFrame = -1;
		if (selectedAnimationFrame == -1)
			return;

//		selectedFrame = null;
		updateGUIForselectedFrame(animationData.frames[selectedAnimationFrame]);
	}

	CharacterDefinition.Frame selectedFrame;

	void updateGUIForselectedFrame(CharacterDefinition.Frame frame) {
		selectedFrame = frame;
		EditorGUILayout.LabelField("Frame: " + selectedAnimationFrame);

		frame.duration = EditorGUILayout.IntField("Duration", frame.duration);
//		frame.sprite = EditorGUILayout.ObjectField(frame.sprite, typeof(Sprite), true) as Sprite;
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("+Hitbox")) {
			CharacterDefinition.Hitbox box = new CharacterDefinition.Hitbox();
			box.box.size = Vector2.one;
			frame.hitboxes.Add(box);
		}
		if (GUILayout.Button("+Hurtbox")) {
			Rect box = new Rect(Vector2.zero, Vector2.one);
			frame.hurtboxes.Add(box);
		}
		if (GUILayout.Button("+Sprite")) {
			CharacterDefinition.SpriteWithRect sprite = new CharacterDefinition.SpriteWithRect();
			sprite.position = Vector2.zero;
			frame.sprites.Add(sprite);
		}

		EditorGUILayout.EndHorizontal();
		Handles.EndGUI();

		Handles.color = Color.red;
		CharacterDefinition.Hitbox hitboxToDelete = null;

		for (int i = 0; i < selectedFrame.hitboxes.Count;i += 1) {
			CharacterDefinition.Hitbox hitbox = selectedFrame.hitboxes[i];
			Handles.BeginGUI();
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Hitbox: " + i);
				if (GUILayout.Button("Delete Hitbox")) {
					hitboxToDelete = hitbox;
				}
				hitbox.knockbackAmount = EditorGUILayout.FloatField("scalar", hitbox.knockbackAmount);
				hitbox.knockbackAngle = EditorGUILayout.FloatField("angle", hitbox.knockbackAngle);
				EditorGUILayout.EndHorizontal();
			}
			Handles.EndGUI();
			hitbox.box = EditorUtil.DraggableResizeableRect(hitbox.box);
			Handles.DrawLine(hitbox.box.position, hitbox.box.position + new Vector2(Mathf.Cos(hitbox.knockbackAngle), Mathf.Sin(hitbox.knockbackAngle)));
		}
		selectedFrame.hitboxes.Remove(hitboxToDelete);

		Handles.color = Color.yellow;
		Rect? hurtboxToDelete = null;

		for (int i = 0; i < selectedFrame.hurtboxes.Count; i+=1) {
			Rect hurtbox = selectedFrame.hurtboxes[i];
			Handles.BeginGUI();
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Hurtbox: " + i);
				if (GUILayout.Button("Delete Hurtbox")) {
					hurtboxToDelete = hurtbox;
				}
				EditorGUILayout.EndHorizontal();
			}
			Handles.EndGUI();
			selectedFrame.hurtboxes[i] = EditorUtil.DraggableResizeableRect(hurtbox);

		}
		if (hurtboxToDelete.HasValue) {
			selectedFrame.hurtboxes.Remove (hurtboxToDelete.Value);
		}

		Handles.color = Color.blue;
		CharacterDefinition.SpriteWithRect spriteWithRectToDelete = null;
		for (int i = 0; i < selectedFrame.sprites.Count; i+=1) {
			CharacterDefinition.SpriteWithRect spriteWithRect = selectedFrame.sprites[i];
			Handles.BeginGUI();
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Sprite: " + i);
				if (GUILayout.Button("Delete Sprite")) {
					spriteWithRectToDelete = spriteWithRect;
				}
				spriteWithRect.sprite = EditorGUILayout.ObjectField(spriteWithRect.sprite, typeof(Sprite), false) as Sprite;
				EditorGUILayout.EndHorizontal();
			}
			Handles.EndGUI();
			EditorUtil.DraggableResizeableRectWithSprite(spriteWithRect.position, spriteWithRect.sprite);
			
		}
		selectedFrame.sprites.Remove (spriteWithRectToDelete);

		Handles.BeginGUI();
	}
}
*/