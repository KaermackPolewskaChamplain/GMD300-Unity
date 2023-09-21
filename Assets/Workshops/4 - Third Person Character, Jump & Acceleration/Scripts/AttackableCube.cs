using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AttackableCube : MonoBehaviour
{
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnPlayerAttack(Vector3 hitDirection)
    {
        rb.AddForce(hitDirection.normalized * 10, ForceMode.Impulse);
    }
}
