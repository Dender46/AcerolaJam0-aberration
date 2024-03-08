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

    private RectTransform _containerRect;
    private float _containerWidth;

    public float additionalOffset = 0.0f;

    void Awake()
    {
        _containerRect = _combatCardsContainer.GetComponent<RectTransform>();
        _containerWidth = _containerRect.sizeDelta.x;

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

        RecalcCardsContainerSpacing();
    }

    [ContextMenu("__RecalcCardsContainerSpacing")]
    public void RecalcCardsContainerSpacing()
    {
        if (_containerRect.childCount == 0)
        {
            return;
        }

        var cardSize = 80;
        var cardSizeHalf = cardSize / 2;
        var childCount = _containerRect.childCount;
        var extraSpaceTaken = (childCount * cardSize) - _containerWidth;

        if (extraSpaceTaken > 0.0f)
        {
            float containerBounds = _containerWidth / 2 - cardSizeHalf;
            for (int i = 0; i < childCount; i++)
            {
                var lerpt = (float)i / (childCount-1);
                var newPos = Mathf.Lerp(-containerBounds, containerBounds, lerpt);

                var childRect = _containerRect.GetChild(i).GetComponent<RectTransform>();
                childRect.anchoredPosition = new Vector2(newPos, 0);
            }
        }
        else
        {
            if (childCount == 1)
            {
                _containerRect.GetChild(0).GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                return;
            }

            for (int i = 0; i < childCount; i++)
            {
                _containerRect.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(i * cardSize, 0);
            }
            float offset = childCount % 2 == 0
                ? Mathf.Floor(childCount / 2) * cardSize - cardSizeHalf
                : Mathf.Floor(childCount / 2) * cardSize;

            for (int i = 0; i < childCount; i++)
            {
                var childRect = _containerRect.GetChild(i).GetComponent<RectTransform>();
                var pos = childRect.anchoredPosition;
                childRect.anchoredPosition = new Vector2(pos.x - offset, 0);
            }
        }
    }
}
