using System;
using TMPro;
using UnityEngine;
using static ItemEquipable;

public class CombatSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text _enemyNameTextUI;
    [SerializeField] private TMP_Text _enemyHPTextUI;
    [Space(7)]
    [SerializeField] private TMP_Text _playerHPTextUI;
    [SerializeField] private TMP_Text _playerDPTextUI;
    [Space(7)]
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _playerCombatStatsUI;

    private EnemyBehaviour _currentEnemy;

    public event EventHandler onEnemyDefeated;

    public static CombatSystem instance { private set; get; }

    void Start()
    {
        DisableUI();
        instance = this;
    }

    public void StartTheFight(GameObject enemyGO)
    {
        _currentEnemy = enemyGO.GetComponent<EnemyBehaviour>();
        EnableUI();
        UpdateEnemyUI();
        GameManager.instance.inventory.TurnItemsToCards();
    }

    public void EndTheFight()
    {
        _currentEnemy.OnDefeated();
        DisableUI();
        GameManager.instance.inventory.TurnCardsToItems();

        onEnemyDefeated?.Invoke(this, EventArgs.Empty);
    }

    public void PlayerAttacksWith(EquipableInfo equipableInfo)
    {
        PlayerAttacksWith(equipableInfo.damage);
    }

    public void PlayerAttacksWith(int dmg)
    {
        _currentEnemy.hp -= dmg;
        if (_currentEnemy.hp <= 0.0f)
        {
            EndTheFight();
        }
        UpdateEnemyUI();
    }

    private void EnableUI()
    {
        _enemyNameTextUI.gameObject.SetActive(true);
        _enemyHPTextUI.gameObject  .SetActive(true);
        _playerCombatStatsUI       .SetActive(true);

        _inventoryUI               .SetActive(false);
    }

    private void DisableUI()
    {
        _enemyNameTextUI.gameObject.SetActive(false);
        _enemyHPTextUI.gameObject  .SetActive(false);
        _playerCombatStatsUI       .SetActive(false);

        _inventoryUI               .SetActive(true);
    }

    private void UpdateEnemyUI()
    {
        _enemyNameTextUI.text = _currentEnemy.enemyName;
        _enemyHPTextUI.text = "HP: " + _currentEnemy.hp;
    }

    [ContextMenu("__DebugAttackEnemy1")]
    public void __DebugAttackEnemy1()
    {
        PlayerAttacksWith(1);
    }

    [ContextMenu("__DebugAttackEnemy2")]
    public void __DebugAttackEnemy2()
    {
        PlayerAttacksWith(2);
    }

    [ContextMenu("__DebugAttackEnemy5")]
    public void __DebugAttackEnemy5()
    {
        PlayerAttacksWith(5);
    }
}
