using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Transform _slotsContainer;

    [Serializable]
    struct SlotInfo
    {
        public Transform t;
        public Image iconImage;

        public EquipableStats stats;
    }

    private SlotInfo[] _slots;
    private int _slotsCount = 0;

    void Awake()
    {
        _slots = new SlotInfo[_slotsContainer.childCount];
        for (int i = 0; i < _slotsContainer.childCount; i++)
        {
            var slotT = _slotsContainer.GetChild(i);
            _slots[i].iconImage = slotT.GetChild(0).GetComponentInChildren<Image>();
        }
    }

    public void Put(EquipableBehaviour item)
    {
        _slots[_slotsCount].stats = item.itemStats;
        _slots[_slotsCount].iconImage.sprite = item.itemPreview;
        _slots[_slotsCount].iconImage.color = Color.white;
        _slotsCount++;
    }
}
