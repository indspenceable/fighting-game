using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorUtil {
	public static Rect DraggableResizeableRect(Rect box) {
		box.position = Handles.Slider2D(box.position + box.size/2f, Vector2.zero, new Vector3(1,0), new Vector3(0, 1), 1, Handles.SphereCap, 0f);
		box.position -= box.size/2;

		Vector2 pos = box.position;
		Vector2 size = box.size;
		Vector3[] verts = new Vector3[] {
			new Vector3(pos.x + size.x, pos.y, 0), 
			new Vector3(pos.x + size.x, pos.y + size.y, 0), 
			new Vector3(pos.x, pos.y + size.y, 0), 
			new Vector3(pos.x, pos.y, 0)
		};
		Handles.DrawSolidRectangleWithOutline(verts, new Color(1f,1f,1f,0.2f), new Color(0f,0f,0f,1f));
		
		float newSizeX = Handles.ScaleValueHandle(size.x, pos + new Vector2(size.x, size.y/2), Quaternion.identity, 0.8f, Handles.CubeCap, 3f);	
		float newSizeY = Handles.ScaleValueHandle(size.y, pos + new Vector2(size.x/2, size.y), Quaternion.identity, 0.8f, Handles.CubeCap, 3f);

		box.size = new Vector2(newSizeX, newSizeY);
		return box;
	}

	public static Texture2D textureFromSprite(Sprite sprite)
	{
		if (sprite == null)
			return null;
		if(sprite.rect.width != sprite.texture.width){
			Texture2D newText = new Texture2D((int)sprite.rect.width,(int)sprite.rect.height);
			Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x, 
			                                             (int)sprite.textureRect.y, 
			                                             (int)sprite.textureRect.width, 
			                                             (int)sprite.textureRect.height );
			newText.SetPixels(newColors);
			newText.Apply();
			return newText;
		} else
			return sprite.texture;
	}

	public static void DraggableResizeableRectWithSprite(Vector2 position, Sprite sprite) {
		if (sprite != null) {
			Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height,
			                           sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
			GUI.DrawTextureWithTexCoords(new Rect(position, new Vector2(10,10)), sprite.texture, spriteRect);
		}
	}
}
