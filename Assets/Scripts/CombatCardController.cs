using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ItemEquipable;

public class CombatCardController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private Image _itemPreview;

    [HideInInspector] public bool wasUsed = false;

    public EquipableInfo assignedEquipment;

    public void AssignItem(EquipableInfo equipableInfo)
    {
        assignedEquipment = equipableInfo;
        _itemName.text = equipableInfo.name;
        _itemPreview.sprite = equipableInfo.preview;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (CombatSystem.instance.isPlayerTurn)
        {
            wasUsed = true;
            GameManager.instance.inventory.OnCardClick(this);
            CombatSystem.instance.PlayerMove(assignedEquipment);
        }
    }
}
