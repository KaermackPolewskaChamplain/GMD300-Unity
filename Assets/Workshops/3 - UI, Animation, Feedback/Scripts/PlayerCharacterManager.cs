using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCharacterManager : MonoBehaviour
{
    public static PlayerCharacterManager Instance;

    private void Awake()
    {
        RegisterManager();
    }

    private void OnDisable()
    {
        UnregisterManager();
    }

    private void RegisterManager()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void UnregisterManager()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }
}
