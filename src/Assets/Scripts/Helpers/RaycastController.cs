// This is fun project with Unity and 2dplatform.
// The source code is referencing from:
// (*) Sebastian Lague [https://www.youtube.com/playlist?list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz]

using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class RaycastController : MonoBehaviour {

	/// <summary>
	/// The layer collision mask is use for this raycast controller
	/// </summary>
	public LayerMask collisionMask;

	/// <summary>
	/// The skin width of object
	/// </summary>
	public const float skinWidth = .015f;

	/// <summary>
	/// The number of horizontal raycast
	/// </summary>
	public int horizontalRayCount = 4;

	/// <summary>
	/// The number of vertical raycast
	/// </summary>
	public int verticalRayCount = 4;

	/// <summary>
	/// The horizontal raycast spacing
	/// </summary>
	[HideInInspector]
	public float horizontalRaySpacing;

	/// <summary>
	/// The vertical raycast spacing
	/// </summary>
	[HideInInspector]
	public float verticalRaySpacing;

	/// <summary>
	/// The box collider 2d component
	/// </summary>
	[HideInInspector]
	public BoxCollider2D collider;

	/// <summary>
	/// The raycast origins struct data
	/// </summary>
	public RaycastOrigins raycastOrigins;

	/// <summary>
	/// The awake method
	/// </summary>
	public virtual void Awake() {
		// Get box collider 2d component from object
		collider = GetComponent<BoxCollider2D> ();
	}

	/// <summary>
	/// The start method
	/// </summary>
	public virtual void Start() {
		// Call calculator raycast spacing in start
		CalculateRaySpacing ();
	}

	/// <summary>
	/// Update raycast origins info
	/// </summary>
	public void UpdateRaycastOrigins() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	/// <summary>
	/// Calculator the raycast spacing in object
	/// </summary>
	public void CalculateRaySpacing() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	/// <summary>
	/// The raycast origins struct
	/// </summary>
	public struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}
