using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    //Sometimes, you want to be able to access a Game Manager from ANYWHERE in your code, without needing to reference it
    //While indeed you could reference the game manager directly in a script, there is a pattern that allows you to publicly access a manager class

    //Introducing the singleton pattern!
    //One Manager to rule them all...

    //Essentially, a singleton manager allows you to have a unique instance of a manager accessible publicly anywhere.
    //The singleton pattern also ensures that the manager is unique. No other instance can exist, there can be only 1 singleton manager of a particular type
    //Any class that uses a singleton pattern should have "Manager" in its name, implying it's unique architecture and usage.

    //This code below is one of the simplest functional singleton pattern that works in Unity, but it can be further expanded to your needs.
    //Please refer to the Reference Material folder for additional implementations

    //The "Instance" variable will keep track of the singleton manager reference
    //This is HOW you will access your singleton, by simply calling SimpleGameManagerSingleton.Instance
    //While you can name this variable however you want, the standard is to name it "Instance"
    //The instance type MUST be the same type as your manager script since it will self reference itself.

    //You can notice that it has 2 modifiers, Public and Static.
    //Public allows the variable to be accessible from other classes/scripts
    //Static makes the variable essentially "Global" and "Shared"

    //What does that mean?
    //The value of a static variable is global and shared to all instances of a class.
    //For example, if I have a class named "EnemyController" and I have a static variable named "TotalEnemyCount", all enemies that use that script will share the same value for "TotalEnemyCount".
    //If one of the enemy changes the static variable, ALL enemies will see the updated value of the static variable.
    //This is why we say it is "shared" and "global" between all instances.

    //To make a singleton, we use this property.
    //No matter in which script you are, if you type SimpleGameManagerSingleton.Instance, you will be able to access the same manager instance.
    //The singleton will also ensure that copies of the manager gets destroyed, so that only 1 single instance can exist at all times.

    //The Instance singleton variable MUST be Public, Static and use the same type as the script to work
    public static InventoryManager Instance;

    //A simple example using the singleton pattern. Please check the TriggerInteractionObject to see how we use this
    public int ObjectCountExample = 0;

    //OnEnable gets called when the component is starting initialization, after the awake method
    private void Awake()
    {
        RegisterManager();
    }

    //OnDisable gets called when the component or gameobject gets disabled or destroyed
    //We ensure to destroy the singleton instance in this case as the manager wouldn't exist anymore
    private void OnDisable()
    {
        UnregisterManager();
    }

    private void RegisterManager()
    {
        //If the Instance variable is not set yet, then make sure that this instance actually becomes the Singleton Instance.
        //We set DonDestroyOnLoad to this gameobject, ensuring that we don't lose this manager when we load a scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        //If the singleton instance is actually set AND it is not this object, we destroy it.
        //That means this instance shouldn't exist, as we MUST have only 1 instance of that manager using the singleton pattern
        //So we Destroy(this), meaning we destroy this instance of the component SimpleGameManagerSingleton
        else if(Instance != this)
        {
            Destroy(this);
        }
    }

    //If we disable the object, we check if it is the actual Instance.
    //If so, then we set Instance to null, since we're destroying this manager and that another one will probably (hopefully) replace it.
    private void UnregisterManager()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    //We'll use another script in the scene and call the following methods to modify the number of keys kept in inventory, showcasing the power of the singleton manager!
    public void AddObjectsToInventory(int numberToAdd)
    {
        ObjectCountExample += numberToAdd;
    }

    public void RemoveObjectsFromInventory(int numberToRemove)
    {
        ObjectCountExample -= numberToRemove;

        if(ObjectCountExample < 0)
        {
            ObjectCountExample = 0;
        }
    }

    public void ResetInventory()
    {
        ObjectCountExample = 0;
    }

    public int GetInventoryObjectTotal()
    {
        return ObjectCountExample;
    }
}
