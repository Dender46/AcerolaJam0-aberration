using TMPro;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text _enemyNameTextUI;
    [SerializeField] private TMP_Text _enemyHpTextUI;

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

        var enemyBeh = enemyGO.GetComponent<EnemyBehaviour>();
        _enemyNameTextUI.text = enemyBeh.enemyName;
        _enemyHpTextUI.text = "HP: " + enemyBeh.maxHp;
        //GameManager.instance.inventory.TurnItemsToCards();
    }

    public void EndTheFight()
    {
        _enemyNameTextUI.gameObject.SetActive(false);
        _enemyHpTextUI.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
