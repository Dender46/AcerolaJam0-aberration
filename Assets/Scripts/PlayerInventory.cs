using System;
using UnityEngine;
using UnityEngine.UI;
using static ItemEquipable;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Transform _slotsContainer;
    [SerializeField] private Transform _combatCardsContainer;
    [SerializeField] private GameObject _combatCardPrefab;

    [Serializable]
    struct SlotInfo
    {
        public Transform t;
        public Image iconImage;

        public EquipableInfo equipmentInfo;
    }

    private SlotInfo[] _slots;
    private int _slotsCount = 0;

    void Awake()
    {
        _slots = new SlotInfo[_slotsContainer.childCount];
        for (int i = 0; i < _slotsContainer.childCount; i++)
        {
            var slotT = _slotsContainer.GetChild(i);
            _slots[i].iconImage = slotT.GetChild(0).GetComponent<Image>();
        }
    }

    public void Put(EquipableInfo info)
    {
        _slots[_slotsCount].equipmentInfo = info;
        _slots[_slotsCount].iconImage.sprite = info.preview;
        _slots[_slotsCount].iconImage.color = Color.white;
        _slotsCount++;
    }

    public void TurnItemsToCards()
    {


        foreach (var inventorySlot in _slots)
        {
            if (inventorySlot.equipmentInfo == null)
            {
                continue;
            }

            var newCardTransform = Instantiate(_combatCardPrefab).GetComponent<RectTransform>();
            newCardTransform.parent = _combatCardsContainer;
            newCardTransform.anchoredPosition = Vector2.zero;
            newCardTransform.localScale = Vector3.one;
            newCardTransform.GetComponent<CombatCardController>().AssignItem(inventorySlot.equipmentInfo);
        }
    }
}
