using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInteractionObject : MonoBehaviour
{
    //To ensure the OnTriggerEnter method works, either object that collide must have a Rigidbody componentadn both needs a collider
    //If you don't want that object to be affected by physics, you can set the Rigidbody as "Kinematic"
    //More info here: https://docs.unity3d.com/ScriptReference/Collider.OnTriggerEnter.html

    private void OnTriggerEnter(Collider other)
    {
        //Tags can be used to identify different types of objects. Every object can only have 1 tag.
        //Here we ensure the collision is between this object and the player
        if(other.tag == "Player")
        {
            //When that happens, we add an object to the Singleton Game Manager using the following line
            InventoryManager.Instance.AddObjectsToInventory(1);

            //We then destroy this item since we collected it
            Destroy(this.gameObject);
        }
    }
}
