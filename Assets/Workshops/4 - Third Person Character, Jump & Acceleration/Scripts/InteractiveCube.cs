using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class InteractiveCube : MonoBehaviour
{
    Renderer objectRenderer;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    public void OnPlayerInteract()
    {
        Debug.Log("INTERACTED");
        objectRenderer.material.color = Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1, 1, 1);
    }
}
