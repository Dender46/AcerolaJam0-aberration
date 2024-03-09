using System;
using System.Collections;
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
    [Header("PlayerStats")]
    public int playerHP = 10;
    public int playerDP = 0;
    [Header("CombatParams")]
    public float attackWaitTime = 1.0f;

    private EnemyBehaviour _currentEnemy;
    public bool isPlayerTurn = true;
    
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
        UpdatePlayerAndEnemyUI();
        GameManager.instance.inventory.TurnItemsToCards();
        isPlayerTurn = true;
    }

    public void EndTheFight(bool isWin)
    {
        if (isWin)
        {
            _currentEnemy.OnDefeated();
            DisableUI();
            GameManager.instance.inventory.TurnCardsToItems();

            onEnemyDefeated?.Invoke(this, EventArgs.Empty);
        }
    }

    public void PlayerMove(EquipableInfo equipableInfo)
    {
        if (equipableInfo.type.HasFlag(EquipableInfo.Type.Heal)) {
            playerHP += equipableInfo.restoreHp;
        }
        if (equipableInfo.type.HasFlag(EquipableInfo.Type.Defence)) {
            playerHP += equipableInfo.restoreDp;
        }
        if (equipableInfo.type.HasFlag(EquipableInfo.Type.Damage)) {
            PlayerAttacksWith(equipableInfo.damage);
        }

        isPlayerTurn = false;
        UpdatePlayerAndEnemyUI();
        StartCoroutine(PlayerMoveInternal(equipableInfo));
    }

    private IEnumerator PlayerMoveInternal(EquipableInfo equipableInfo)
    {
        yield return new WaitForSeconds(attackWaitTime);

        // if enemy was killed or player lost
        if (GameManager.instance.gameState == GameManager.GameState.FightingEnemy)
        {
            Debug.Log("EnemyAttacks");
            EnemyAttacks(_currentEnemy.damage);
            yield return new WaitForSeconds(attackWaitTime);
            Debug.Log("Player can attack");
            isPlayerTurn = true;
        }
    }

    public void EnemyAttacks(int dmg)
    {
        if (playerDP >= dmg)
        {
            playerDP -= dmg;
        }
        else
        {
            var dmgAfterDP = dmg - playerDP;
            playerDP = 0;
            playerHP -= dmgAfterDP;
        }

        if (playerHP <= 0)
        {
            EndTheFight(false);
        }
        UpdatePlayerAndEnemyUI();
    }

    public void PlayerAttacksWith(int dmg)
    {
        _currentEnemy.hp -= dmg;
        if (_currentEnemy.hp <= 0)
        {
            EndTheFight(true);
        }
        UpdatePlayerAndEnemyUI();
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

    private void UpdatePlayerAndEnemyUI()
    {
        _enemyNameTextUI.text = _currentEnemy.enemyName;
        _enemyHPTextUI.text = "HP: " + _currentEnemy.hp;

        _playerHPTextUI.text = playerHP.ToString();
        _playerDPTextUI.text = playerDP.ToString();
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
