using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGameManagerSingleton : MonoBehaviour
{
    //Sometimes, you want to be able to access a Game Manager from everywhere in your code, for instance when you want to edit the player score
    //While your script could directly reference the game manager, there is a better way for systems that involve a single unique manager.

    //Introducing the singleton pattern!
    //In essence, a singleton can be accessed from pretty much anywhere and will always reference to your single manager.
    //You can use this pattern to keep track of active values during gameplay, generally involving high level game logic.
    //For example, it can be used in Game Logic Managers, Score Managers, Player Managers and any management class that will be unique.
    //The singleton pattern CANNOT be used for any object that will/can be duplicated, such as objects and enemies.
    //If you need a singleton pattern to keep track of enemy count for instance, you can place that logic into an EnemyManager class instead.
    //Generally, any class that uses a singleton pattern will have "Manager" in its name, implying it's unique architecture and usage.

    //This is probably one of the simplest functional singleton pattern that works in Unity, but it can be further expanded of course.
    //More info about other implementations here: https://learn.microsoft.com/en-us/previous-versions/msp-n-p/ff650849(v=pandp.10)

    //This "Instance" variable will be used to keep track of the singleton reference
    //While you can name this variable however you want, the standard is to name it "Instance"
    //The variable type MUST be the same type as your manager script since it will self reference itself.

    //You can notice that it has 2 modifiers, Public and Static.
    //Public allows the variable to be accessible from other classes/scripts
    //Static makes the variable essentially "Global"

    //What does that mean?
    //The value of this variable is global. Even if I were to attach this script on several gameobjects, the value of this variable would be
    //exactly the same no matter where I access it and no matter the object.
    //This is what makes the singleton pattern work.
    //No matter where you are in the code, if you type SimpleGameManagerSingleton.Instance, you will always get the same value.
    //And what is that value? Check the RegisterManager and UnregisterManager functions to know!

    public static SimpleGameManagerSingleton Instance;

    public int ObjectCountExample = 0;

    //OnEnable gets called when the component is starting initialization, after the awake method
    private void OnEnable()
    {
        RegisterManager();
    }

    //OnDisable gets called when the component or gameobject gets disabled or destroyed
    private void OnDisable()
    {
        UnregisterManager();
    }

    private void RegisterManager()
    {
        //If the Instance variable is not set yet, then make sure that this component object actually becomes the Instance.
        //We set DonDestroyOnLoad to this gameobject, ensuring that we don't lose this manager when we load a scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        //If the instance is actually set, that means another object got set as the Singleton instance of the manager.
        //That means this object shouldn't exist, as we MUST have only 1 instance of any type of manager using the singleton pattern
        //So we Destroy(this), meaning we destroy this instance of the component SimpleGameManagerSingleton
        else
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

    public int GetInventoryObjectTotal()
    {
        return ObjectCountExample;
    }
}
