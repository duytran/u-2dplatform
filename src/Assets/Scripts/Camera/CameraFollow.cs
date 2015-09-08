// This is fun project with Unity and 2dplatform.
// The source code is referencing from:
// (*) Sebastian Lague [https://www.youtube.com/playlist?list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz]

using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	/// <summary>
	/// The target of camera follow
	/// </summary>
	public Controller2D target;

	/// <summary>
	/// The camera vertical offset
	/// </summary>
	public float verticalOffset;

	/// <summary>
	/// The camera horizontal look ahead distance
	/// </summary>
	public float lookAheadDstX;

	/// <summary>
	/// The camera horizontal smooth time
	/// </summary>
	public float lookSmoothTimeX;

	/// <summary>
	/// The vertical smooth time
	/// </summary>
	public float verticalSmoothTime;

	/// <summary>
	/// The focus area size
	/// </summary>
	public Vector2 focusAreaSize;

	/// <summary>
	/// The focus area struct
	/// </summary>
	FocusArea __focusArea;

	/// <summary>
	/// The camera horizontal current look ahead
	/// </summary>
	float __currentLookAheadX;

	/// <summary>
	/// The camera horizontal target look ahead
	/// </summary>
	float __targetLookAheadX;

	/// <summary>
	/// The camera horizontal look ahead direction
	/// </summary>
	float __lookAheadDirX;

	/// <summary>
	/// The camera horizontal smooth look velocity
	/// </summary>
	float __smoothLookVelocityX;

	/// <summary>
	/// The camera vertical smooth velocity
	/// </summary>
	float __smoothVelocityY;

	/// <summary>
	/// The camera look ahead stopped flag
	/// </summary>
	bool __lookAheadStopped;

	/// <summary>
	/// The start method
	/// </summary>
	void Start () {
		__focusArea = new FocusArea (target.collider.bounds, focusAreaSize);
	}

	/// <summary>
	/// The late update method
	/// </summary>
	void LateUpdate () {
		// Update focus area
		__focusArea.Update (target.collider.bounds);

		// Compute the focus position
		Vector2 focusPosition = __focusArea.centre + Vector2.up * verticalOffset;

		// If focus area has horizontal move
		if ( __focusArea.velocity.x != 0 ) {

			// Get look ahead direction from velocity
			__lookAheadDirX = Mathf.Sign (__focusArea.velocity.x );

			// If player input and focus area have same direction and greater Zero
			if (Mathf.Sign (target.playerInput.x) == Mathf.Sign (__focusArea.velocity.x) &&
			    target.playerInput.x != 0) {
				// Mark look ahead continue and compute compute target horizontal look ahead
				__lookAheadStopped = false;
				__targetLookAheadX = __lookAheadDirX * lookAheadDstX;
			}else {
				// Otherwise, player input and focus area different
				// Let make look ahead stopped and compute target horizontal look ahead
				if (!__lookAheadStopped) {
					__lookAheadStopped = true;
					__targetLookAheadX = __currentLookAheadX + (__lookAheadDirX* lookAheadDstX - __currentLookAheadX)/4f;
				}
			}
		}

		// Compute smooth horizontal look ahead move
		__currentLookAheadX = Mathf.SmoothDamp ( __currentLookAheadX, __targetLookAheadX, ref __smoothLookVelocityX, lookSmoothTimeX);

		// Compute smooth vertical look ahead move
		focusPosition.y = Mathf.SmoothDamp (transform.position.y, focusPosition.y, ref __smoothVelocityY, verticalSmoothTime);

		// Compute new position for camera
		focusPosition += Vector2.right * __currentLookAheadX;
		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
	}

	/// <summary>
	/// The on draw gizmos method
	/// </summary>
	void OnDrawGizmos () {
		Gizmos.color = new Color (1, 0, 0, .5f);
		Gizmos.DrawCube (__focusArea.centre, __focusAreaSize);
	}

	/// <summary>
	/// The focus area struct
	/// This struct contain info about focus area of camera and target
	/// </summary>
	struct FocusArea {
		public Vector2 centre;
		public Vector2 velocity;
		float left, right;
		float top, bottom;

		/// <summary>
		/// The constructor of struct
		/// </summary>
		/// <param name="targetBounds">targetBounds</param>
		/// <param name="size">size</param>
		public FocusArea ( Bounds targetBounds, Vector2 size) {
			left = targetBounds.center.x - size.x/2;
			right = targetBounds.center.x + size.x/2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;
			centre = new Vector2 ( (left + right)/2, (top + bottom)/2 );
		}

		/// <summary>
		/// The update infor for focus area with target
		/// </summary>
		/// <param name="targetBounds">targetBounds</param>
		public void Update (Bounds targetBounds) {
			// Comupute the horizontal shifting
			float shiftX = 0;
			if ( targetBounds.min.x < left ) {
				shiftX = targetBounds.min.x - left;
			} else if ( targetBounds.max.x > right ) {
				shiftX = targetBounds.max.x - right;
			}

			left += shiftX;
			right += shiftX;

			// Comupute the vertical shifting
			float shiftY = 0;
			if ( targetBounds.min.y < bottom ) {
				shiftY = targetBounds.min.y - bottom;
			} else if ( targetBounds.max.y > top ) {
				shiftY = targetBounds.max.y - top;
			}

			top += shiftY;
			bottom += shiftY;

			// Comute the focus area velocity exactly with the shifting
			velocity = new Vector2 (shiftX, shiftY);
			centre = new Vector2 ( (left + right)/2, (top + bottom)/2 );
		}

	}

}
