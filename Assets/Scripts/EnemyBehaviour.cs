using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public string enemyName;

    [Header("Stats")]
    public int maxHp = 10;
    public int hp;
    public int damage = 2;

    private void Start()
    {
        hp = maxHp;
        if (enemyName.Length == 0)
        {
            throw new UnityException("Enemy doesn't have a name in eney beh: " + gameObject.name);
        }
    }

    private void Update()
    {

    }

}
