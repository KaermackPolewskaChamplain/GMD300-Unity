using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class InventoryDoorCheckAnimation : MonoBehaviour
{
    public int ObjectsRequiredToUnlock = 7;

    public MeshRenderer[] doorSlotList;

    public Material UnactiveDoorSlotMaterial;
    public Material ActiveDoorSlotMaterial;
    
    private int lastUpdatedSlot = 0;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isOpened", false);

        foreach (MeshRenderer doorSlot in doorSlotList)
        {
            if(doorSlot == null)
            {
                Debug.LogError("Must assign a meshrenderer to each doorSlot in the list", this);
                this.enabled = false;
            }

            doorSlot.material = UnactiveDoorSlotMaterial;
        }
    }

    private void Update()
    {
        if(InventoryManager.Instance.GetInventoryObjectTotal() > lastUpdatedSlot)
        {
            lastUpdatedSlot++;
            UpdateDoorSlot(lastUpdatedSlot);

            if(lastUpdatedSlot == ObjectsRequiredToUnlock)
            {
                animator.SetBool("isOpened", true);
            }
        }
    }

    private void UpdateDoorSlot(int slotIndex)
    {
        //We're applying -1 since the array index starts at 0
        doorSlotList[slotIndex - 1].material = ActiveDoorSlotMaterial;
    }

    private void OnValidate()
    {
        if(doorSlotList.Length != ObjectsRequiredToUnlock)
        {
            doorSlotList = new MeshRenderer[ObjectsRequiredToUnlock];
        }
    }
}
