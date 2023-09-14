using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloadOnTriggerEnter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            InventoryManager.Instance.ResetInventory();

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Vector4(1, 0, 0, 0.5f);
        Gizmos.DrawCube(GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.size + Vector3.one * 0.1f);
    }
}
