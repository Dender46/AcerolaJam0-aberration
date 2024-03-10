using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ItemEquipable;

public class CombatCardController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private Image _itemPreview;
    [SerializeField] private RectTransform _statsContainer;
    [SerializeField] private GameObject _cardStatPrefab;

    [Header("References")]
    [SerializeField] private Sprite _damageIcon;
    [SerializeField] private Sprite _healIcon;
    [SerializeField] private Sprite _defenceIcon;

    [HideInInspector] public bool wasUsed = false;

    public EquipableInfo assignedEquipment;

    public void AssignItem(EquipableInfo equipableInfo)
    {
        assignedEquipment = equipableInfo;
        _itemName.text = equipableInfo.name;
        _itemPreview.sprite = equipableInfo.preview;

        // create icon stats for card
        if (equipableInfo.type.HasFlag(EquipableInfo.Type.Damage))
        {
            CreateStat(_damageIcon, equipableInfo.damage);
        }
        if (equipableInfo.type.HasFlag(EquipableInfo.Type.Heal))
        {
            CreateStat(_healIcon, equipableInfo.restoreHp);
        }
        if (equipableInfo.type.HasFlag(EquipableInfo.Type.Defence))
        {
            CreateStat(_defenceIcon, equipableInfo.restoreDp);
        }
    }

    private void CreateStat(Sprite statIcon, int value)
    {
        var newStat = Instantiate(_cardStatPrefab).GetComponent<RectTransform>();
        newStat.SetParent(_statsContainer, false);
        newStat.anchoredPosition = Vector2.zero;
        newStat.GetComponentInChildren<Image>().sprite = statIcon;
        newStat.GetComponentInChildren<TMP_Text>().text = value.ToString();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (CombatSystem.instance.isPlayerTurn)
        {
            wasUsed = true;
            PlayerInventory.instance.OnCardClick(this);
            CombatSystem.instance.PlayerMove(assignedEquipment);
        }
    }
}
