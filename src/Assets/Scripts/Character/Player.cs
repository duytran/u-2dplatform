// This is fun project with Unity and 2dplatform.
// The source code is referencing from
// (*) Sebastian Lague [https://www.youtube.com/playlist?list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz]

using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	/// <summary>
	/// The max jump height
	/// </summary>
	public float maxJumpHeight = 4;

	/// <summary>
	/// The min jump height
	/// </summary>
	public float minJumpHeight = 1;

	/// <summary>
	/// The time to jump apex
	/// This filed used to calculator Gravity
	/// </summary>
	public float timeToJumpApex = .4f;

	/// <summary>
	/// The acceleration time air borne
	/// </summary>
	float __accelerationTimeAirborne = .2f;

	/// <summary>
	/// The acceleration time grounded
	/// </summary>
	float __accelerationTimeGrounded = .1f;

	/// <summary>
	/// The move speed
	/// </summary>
	float __moveSpeed = 6;

	/// <summary>
	/// The wall jump climb
	/// </summary>
	public Vector2 wallJumpClimb;

	/// <summary>
	/// The wall jump off
	/// </summary>
	public Vector2 wallJumpOff;

	/// <summary>
	/// The wall leap
	/// </summary>
	public Vector2 wallLeap;

	/// <summary>
	/// The wall sliding speed max
	/// </summary>
	public float wallSlidingSpeedMax = 3f;

	/// <summary>
	/// The wall stick time
	/// </summary>
	public float wallStickTime = .25f;

	/// <summary>
	/// The time to wall unstick
	/// </summary>
	float __timeToWallUnstick;

	/// <summary>
	/// The character gravity
	/// </summary>
	float __gravity;

	/// <summary>
	/// The max jump velocity
	/// </summary>
	float __maxJumpVelocity;

	/// <summary>
	/// The min jump velocity
	/// </summary>
	float __minJumpVelocity;

	/// <summary>
	/// The character velocity
	/// </summary>
	__velocity velocity;

	/// <summary>
	/// The horizontal velocity smoothing
	/// </summary>
	float __velocityXSmoothing;

	/// <summary>
	/// The reference component
	/// Controller 2D
	/// </summary>
	__controller controller;

	/// <summary>
	/// The start method
	/// </summary>
	void Start() {
		// Get controller 2D component
		__controller = GetComponent<Controller2D> ();

		// Compute the character gravity with formular
		__gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);

		// Compute the max jump velocity
		__maxJumpVelocity = Mathf.Abs(__gravity) * timeToJumpApex;

		// Compute the min jump velocity
		__minJumpVelocity = Mathf.Sqrt ( 2 * Mathf.Abs (__gravity) * minJumpHeight );

		print ("Gravity: " + __gravity + "  Jump Velocity: " + __maxJumpVelocity);
	}

	/// <summary>
	/// The update method
	/// </summary>
	void Update() {
		// Get the horizontal and vertical input
		Vector2 input = new Vector2 (
			Input.GetAxisRaw ("Horizontal"),
			Input.GetAxisRaw ("Vertical")
			);

		// Get wall horizontal direction of controller
		int wallDirX = (__controller.collisions.left) ? -1 : 1;

		// Compute the target horizontal velocity and velocity of character
		float targetVelocityX = input.x * __moveSpeed;
		__velocity.x = Mathf.SmoothDamp (__velocity.x,
											targetVelocityX,
											ref __velocityXSmoothing,
											(__controller.collisions.below)?
												__accelerationTimeGrounded:
												__accelerationTimeAirborne);

		// Are character sliding on wall?
		bool wallSliding = false;
		if ((__controller.collisions.left || __controller.collisions.right) &&
		    !__controller.collisions.below && __velocity.y < 0) {

			// The character is sliding on wall
			wallSliding = true;

			// Adjust the sliding velocity
			if ( __velocity.y < -wallSlidingSpeedMax )
			{
				__velocity.y = -wallSlidingSpeedMax;
			}

			// If character is stick on wall in a chunk time,
			// let make sure velocity is zero
			if ( __timeToWallUnstick > 0 ) {

				__velocityXSmoothing = 0;
				__velocity.x = 0;

				// Update stick time
				if (input.x != wallDirX && input.x != 0 ) {
					__timeToWallUnstick -= Time.deltaTime;
				}else {
					__timeToWallUnstick = wallStickTime;
				}

			} else {
				__timeToWallUnstick = wallStickTime;
			}
		}

		// Make character jump when press down Space
		if (Input.GetKeyDown (KeyCode.Space)) {

			// If character is sliding
			if ( wallSliding ) {
				if ( wallDirX == input.x ){
					__velocity.x = -wallDirX * wallJumpClimb.x;
					__velocity.y = wallJumpClimb.y;
				}else if (input.x == 0) {
					__velocity.x = -wallDirX * wallJumpOff.x;
					__velocity.y = wallJumpOff.y;
				}else {
					__velocity.x = -wallDirX * wallLeap.x;
					__velocity.y = wallLeap.y;
				}
			}

			// If character is standing on a ground
			// let's make sure character with max jump velocity
			// (Hard Press Space)
			if ( __controller.collisions.below ) {
				__velocity.y = __maxJumpVelocity;
			}
		}

		// Make character with min jump velocity when get input space up
		// (Soft Press Space)
		if ( Input.GetKeyUp (KeyCode.Space) ) {
			if ( __velocity.y > __minJumpVelocity ) {
				__velocity.y = __minJumpVelocity;
			}
		}

		// Update vertical velocity of character with gravity
		__velocity.y += __gravity * Time.deltaTime;

		// Move controller with velocity and input
		__controller.Move (__velocity * Time.deltaTime, input) ;

		// If character is collision with top or standing on a ground
		// Make sure velocity vertical is Zero
		if (__controller.collisions.above || __controller.collisions.below) {
			__velocity.y = 0;
		}
	}
}
