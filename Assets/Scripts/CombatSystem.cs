using TMPro;
using UnityEngine;
using static ItemEquipable;

public class CombatSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text _enemyNameTextUI;
    [SerializeField] private TMP_Text _enemyHpTextUI;

    private EnemyBehaviour _currentEnemy;

    public static CombatSystem instance { private set; get; }

    void Start()
    {
        _enemyNameTextUI.gameObject.SetActive(false);
        _enemyHpTextUI.gameObject.SetActive(false);

        instance = this;
    }

    public void StartTheFight(GameObject enemyGO)
    {
        _enemyNameTextUI.gameObject.SetActive(true);
        _enemyHpTextUI.gameObject.SetActive(true);

        _currentEnemy = enemyGO.GetComponent<EnemyBehaviour>();
        UpdateUI();
        GameManager.instance.inventory.TurnItemsToCards();
    }

    public void EndTheFight()
    {
        _enemyNameTextUI.gameObject.SetActive(false);
        _enemyHpTextUI.gameObject.SetActive(false);
    }

    public void PlayerAttacksWith(EquipableInfo equipableInfo)
    {
        _currentEnemy.hp -= equipableInfo.damage;

        UpdateUI();
    }

    private void UpdateUI()
    {
        _enemyNameTextUI.text = _currentEnemy.enemyName;
        _enemyHpTextUI.text = "HP: " + _currentEnemy.hp;
    }
}
