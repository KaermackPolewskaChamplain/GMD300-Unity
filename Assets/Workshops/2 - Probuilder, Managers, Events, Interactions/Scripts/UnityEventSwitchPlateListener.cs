using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class UnityEventSwitchPlateListener : MonoBehaviour
{
    public Material InactiveMaterial;
    public Material ActiveMaterial;

    private MeshRenderer meshRenderer;

    private bool isActive = false;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void OnSwitchPress()
    {
        if (isActive)
        {
            meshRenderer.material = InactiveMaterial;
            isActive = false;
        }
        else
        {
            meshRenderer.material = ActiveMaterial;
            isActive = true;
        }
    }
}
