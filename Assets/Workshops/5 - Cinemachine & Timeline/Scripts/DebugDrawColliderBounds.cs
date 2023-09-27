using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[ExecuteInEditMode]
public class DebugDrawColliderBounds : MonoBehaviour
{
#if UNITY_EDITOR

    BoxCollider boxCollider;

    private void OnValidate()
    {
        boxCollider = GetComponent<BoxCollider>();

        //Due to how the gizmos works, we don't want to modify the collider size and center, we use instead the transform position and scale
        boxCollider.size = Vector3.one;
        boxCollider.center = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Vector4(0, 1, 1, 0.5f);

        //This is a bit advanced, but it handles the position, rotation and scale of the gizmos space.
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
#endif
}
