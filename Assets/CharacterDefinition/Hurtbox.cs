using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class Hurtbox : MonoBehaviour {
	public BoxCollider2D myCollider;
	void Start() {
		myCollider = GetComponent<BoxCollider2D>();
	}
	
	void OnDrawGizmos() {
		if (myCollider == null) {
			Start();
		}
		Gizmos.color = Color.green;
		
		float top = myCollider.offset.y + (myCollider.size.y / 2f);
		float btm = myCollider.offset.y - (myCollider.size.y / 2f);
		float left = myCollider.offset.x - (myCollider.size.x / 2f);
		float right = myCollider.offset.x + (myCollider.size.x /2f);
		
		Vector3 topLeft = transform.TransformPoint (new Vector3( left, top, 0f));
		Vector3 topRight = transform.TransformPoint (new Vector3( right, top, 0f));
		Vector3 btmLeft = transform.TransformPoint (new Vector3( left, btm, 0f));
		Vector3 btmRight = transform.TransformPoint (new Vector3( right, btm, 0f));
		
		Gizmos.DrawLine(topLeft, topRight);
		Gizmos.DrawLine(topRight, btmRight);
		Gizmos.DrawLine(btmRight, btmLeft);
		Gizmos.DrawLine(btmLeft, topLeft);
	}
}

