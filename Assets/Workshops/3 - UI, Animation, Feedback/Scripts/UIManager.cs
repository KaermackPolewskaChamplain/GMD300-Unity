using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Canvas), typeof(Animator))]
public class UIManager : MonoBehaviour
{
    //Singleton instance
    public static UIManager Instance;
    private Animator animator;

    private CharacterController playerController;

    public TMP_Text CollectiblePanelTextLabel;

    public float timeToShowCollectiblePanel = 2;
    public float timeToHideCollectiblePanel = 1f;
    private float collectiblePanelTimer = 1;

    //The number of collectibles in the inventory last time the UI checked
    //Gets updated every update after comparing the value of the InventoryManager
    private int lastCollectibleCount = 0;

    private void Awake()
    {
        RegisterSingletonInstance();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        playerController = PlayerCharacterManager.Instance.GetComponent<CharacterController>();
    }

    private void OnDisable()
    {
        UnregisterSingletonInstance();
    }

    private void RegisterSingletonInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    private void UnregisterSingletonInstance()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    //We're updating the UI in late update to ensure ALL changes in the update loop have been already completed.
    private void LateUpdate()
    {
        ProcessCollectiblePanelVisibility();
    }

    private void ProcessCollectiblePanelVisibility()
    {
        //The magnitude of a vector is its length. Here, we check if the velocity of the player is higher than 0.1 in any direction.
        //If so, show the UI panels, otherwise hide those unneeded for gameplay.
        if (playerController.velocity.magnitude > 0.1f)
        {
            collectiblePanelTimer -= Time.deltaTime / timeToHideCollectiblePanel;
        }
        else
        {
            //Decrease the value of the timer every frame
            //When the timer is down to 0, show the collectible UI panel
            collectiblePanelTimer += Time.deltaTime / timeToShowCollectiblePanel;
        }

        //Clamp the collectiblePanelTimer value between 0 and 1
        collectiblePanelTimer = Mathf.Clamp(collectiblePanelTimer, 0, 1);

        if (collectiblePanelTimer == 1)
        {
            ShowCollectiblePanel(true);
        }
        else if (collectiblePanelTimer == 0)
        {
            ShowCollectiblePanel(false);
        }

        //Here, we check whether the count of collectibles has been updated since last check
        //If so, we show the collectible panel and update the text value
        if (InventoryManager.Instance.GetInventoryObjectTotal() != lastCollectibleCount)
        {
            lastCollectibleCount = InventoryManager.Instance.GetInventoryObjectTotal();

            UpdateCollectiblePanel();
        }
    }

    private void ShowCollectiblePanel(bool showPanel)
    {
        animator.SetBool("showCollectiblePanel", showPanel);
    }

    private void UpdateCollectiblePanel()
    {
        animator.SetTrigger("updateCollectiblePanel");

        //Whenever we update the collectible panel, we update the text value first
        CollectiblePanelTextLabel.text = InventoryManager.Instance.GetInventoryObjectTotal().ToString();

        //Update the timer value to make sure the UI stays on screen for a while
        collectiblePanelTimer = 1;

        //Then we trigger showing the collectible panel
        ShowCollectiblePanel(true);
    }

    public void ShowInteractPrompt(bool showPrompt)
    {
        animator.SetBool("showInteractPrompt", showPrompt);
    }
}
