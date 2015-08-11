using UnityEngine;
using System.Collections;

public abstract class CharacterController : MonoBehaviour {
	public abstract float Horizontal();
	public abstract bool Jumping();
	public abstract bool Crouching();
	public abstract bool Attacking ();

	public class PlayerCharacterController : CharacterController {
		public override float Horizontal() {
			return Input.GetAxis("Horizontal");
		}

		public override bool Jumping() {
			return (Input.GetAxis("Vertical") > 0.5f || Input.GetButton("Jump"));
		}

		public override bool Crouching() {
			return (Input.GetAxis("Vertical") < -0.5f);
		}
		public override bool Attacking() {
			return (Input.GetKeyDown(KeyCode.K));
		}
	}

	public class DoNothingCharacterController : CharacterController {
		public override float Horizontal() {
			return 0f;
		}
		
		public override bool Jumping() {
			return false;
		}

		public override bool Crouching() {
			return false;
		}

		public override bool Attacking() {
			return false;
		}
	}
}
