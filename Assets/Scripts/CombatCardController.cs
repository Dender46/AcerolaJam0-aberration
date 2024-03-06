using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ItemEquipable;

public class CombatCardController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private Image _itemPreview;

    public EquipableInfo assignedEquipment;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignItem(EquipableInfo equipableInfo)
    {
        assignedEquipment = equipableInfo;
        _itemName.text = equipableInfo.name;
        _itemPreview.sprite = equipableInfo.preview;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        CombatSystem.instance.PlayerAttacksWith(assignedEquipment);
        Destroy(gameObject);
    }
}
