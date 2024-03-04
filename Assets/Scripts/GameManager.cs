using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ConveyorController conveyorController;
    public PlayerInventory inventory;

    public GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void UI_ApproveCurrentItem()
    {
        if (!conveyorController.IsMoving())
            conveyorController.ResumeConveyor();
    }

    public void UI_DeclineCurrentItem()
    {
        if (!conveyorController.IsMoving())
            conveyorController.ResumeConveyor();
    }

    public void UI_GrabCurrentItem()
    {
        if (!conveyorController.IsMoving())
        {
            var grabbedItem = conveyorController.GrabCurrentItem();
            // TODO: move to inventory
            inventory.Put(grabbedItem.GetComponent<EquipableBehaviour>());
            Destroy(grabbedItem);
            conveyorController.ResumeConveyor();
        }
    }

}
