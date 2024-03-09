using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ItemEquipable;

public class PlayerInventory : MonoBehaviour
{
    [Header("RegularInventory")]
    [SerializeField] private Transform _slotsContainer;
    [Header("Combat")]
    [SerializeField] private RectTransform _combatCardsContainer;
    [SerializeField] private GameObject _combatCardPrefab;
    [SerializeField] private float _containerHideOffsetY = 90.0f;
    [Header("__Debug")]
    [SerializeField] private GameObject __debugEquipmentHeal;
    [SerializeField] private GameObject __debugEquipmentDamage;

    [Serializable]
    class SlotInfo
    {
        public Image iconImage;

        public EquipableInfo equipmentInfo;
    }

    class CombatSlotInfo
    {
        public CombatCardController cardController;
        public RectTransform rectTransform;
    }

    private SlotInfo[] _slots;
    private int _slotsCount = 0;

    private List<CombatSlotInfo> _combatCards = new();
    private float _containerWidth;

    private readonly Color WHITE_ALPHA_1 = new(1.0f, 1.0f, 1.0f, 1.0f);
    private readonly Color WHITE_ALPHA_0 = new(0.0f, 0.0f, 0.0f, 0.0f);

    public static PlayerInventory instance { private set; get; }

    void Awake()
    {
        _containerWidth = _combatCardsContainer.sizeDelta.x;

        _slots = new SlotInfo[_slotsContainer.childCount];
        for (int i = 0; i < _slotsContainer.childCount; i++)
        {
            var slotT = _slotsContainer.GetChild(i);
            _slots[i] = new SlotInfo() {
                iconImage = slotT.GetChild(0).GetComponent<Image>()
            };
        }

        instance = this;
    }

    public void Put(EquipableInfo info)
    {
        _slots[_slotsCount].equipmentInfo = info;
        _slots[_slotsCount].iconImage.sprite = info.preview;
        _slots[_slotsCount].iconImage.color = WHITE_ALPHA_1;
        _slotsCount++;
    }

    public void TurnItemsToCards()
    {
        _combatCards.Clear();
        foreach (var inventorySlot in _slots)
        {
            if (inventorySlot.equipmentInfo == null)
            {
                continue;
            }

            var newCardTransform = Instantiate(_combatCardPrefab).GetComponent<RectTransform>();
            newCardTransform.SetParent(_combatCardsContainer, false);
            newCardTransform.anchoredPosition = Vector2.zero;
            newCardTransform.localScale = Vector3.one;
            var combatController = newCardTransform.GetComponent<CombatCardController>();
            combatController.AssignItem(inventorySlot.equipmentInfo);
            _combatCards.Add(new CombatSlotInfo() {
                cardController = combatController,
                rectTransform = newCardTransform
            });

            // Clear items from inventory
            inventorySlot.equipmentInfo = null;
            inventorySlot.iconImage.sprite = null;
            inventorySlot.iconImage.color = WHITE_ALPHA_0;
        }
        RecalcCardsContainerSpacing();
    }

    public void TurnCardsToItems()
    {
        _slotsCount = 0;
        foreach (var combatCard in _combatCards)
        {
            if (combatCard != null)
            {
                Put(combatCard.cardController.assignedEquipment);
                Destroy(combatCard.rectTransform.gameObject);
            }
        }
    }

    public void OnCardClick(CombatCardController card)
    {
        for (int i = 0; i < _combatCards.Count; i++)
        {
            if (_combatCards[i].cardController == card)
            {
                _combatCards.RemoveAt(i);
                break;
            }
        }
        Destroy(card.gameObject);
        RecalcCardsContainerSpacing();
    }

    [ContextMenu("__RecalcCardsContainerSpacing")]
    public void RecalcCardsContainerSpacing()
    {
        if (_combatCards.Count == 0)
        {
            return;
        }

        var cardSize = 80;
        var cardSizeHalf = cardSize / 2;
        var childCount = _combatCards.Count;
        var extraSpaceTaken = (childCount * cardSize) - _containerWidth;

        if (extraSpaceTaken > 0.0f)
        {
            float containerBounds = _containerWidth / 2 - cardSizeHalf;
            for (int i = 0; i < childCount; i++)
            {
                var lerpt = (float)i / (childCount-1);
                var newPos = Mathf.Lerp(-containerBounds, containerBounds, lerpt);

                _combatCards[i].rectTransform.anchoredPosition = new Vector2(newPos, 0);
            }
        }
        else
        {
            if (childCount == 1)
            {
                _combatCards[0].rectTransform.anchoredPosition = Vector2.zero;
                return;
            }

            for (int i = 0; i < childCount; i++)
            {
                _combatCards[i].rectTransform.anchoredPosition = new Vector2(i * cardSize, 0);
            }
            float offset = childCount % 2 == 0
                ? Mathf.Floor(childCount / 2) * cardSize - cardSizeHalf
                : Mathf.Floor(childCount / 2) * cardSize;

            for (int i = 0; i < childCount; i++)
            {
                var childRect = _combatCards[i].rectTransform;
                var pos = childRect.anchoredPosition;
                childRect.anchoredPosition = new Vector2(pos.x - offset, 0);
            }
        }
    }

    public void HideCombatCards()
    {
        _combatCardsContainer.anchoredPosition = new Vector2(0.0f, _containerHideOffsetY);
    }

    public void ShowCombatCards()
    {
        _combatCardsContainer.anchoredPosition = Vector2.zero;
    }

    [ContextMenu("__DebugPutHeal")]
    public void __DebugPutHeal()
    {
        __debugEquipmentHeal.GetComponent<ItemEquipable>().info.preview = __debugEquipmentHeal.GetComponent<ItemEquipable>().itemPreview;
        Put(__debugEquipmentHeal.GetComponent<ItemEquipable>().info);
    }

    [ContextMenu("__DebugPutDamage")]
    public void __DebugPutDamage()
    {
        __debugEquipmentDamage.GetComponent<ItemEquipable>().info.preview = __debugEquipmentDamage.GetComponent<ItemEquipable>().itemPreview;
        Put(__debugEquipmentDamage.GetComponent<ItemEquipable>().info);
    }
}
