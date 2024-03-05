using UnityEngine;

public class ItemEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;

    private void OnEnable()
    {
        if (enemyPrefab == null)
        {
            throw new UnityException("Enemy Item has no assigned enemy: " + gameObject.name);
        }
    }
}
