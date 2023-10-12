using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProBuilderRepairDefaultMaterialForHDRP : MonoBehaviour
{
    public Material ProBuilderDefaultMaterialForHDRP;
    public bool ExecuteRepairSequence = false;

    private void OnValidate()
    {
        if (ExecuteRepairSequence == true)
        {
            if (ProBuilderDefaultMaterialForHDRP == null)
            {
                Debug.LogError("Cannot replace probuilder material without a replacement material. Please set ProBuilderDefaultHDRPMaterial.");
                ExecuteRepairSequence = false;

                return;
            }

            MeshRenderer[] meshRendererSceneList = FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (MeshRenderer meshRenderer in meshRendererSceneList)
            {
                if (meshRenderer.sharedMaterial.name == "ProBuilderDefault")
                {
                    meshRenderer.sharedMaterial = ProBuilderDefaultMaterialForHDRP;
                }
            }

            ExecuteRepairSequence = false;
        }
    }
}