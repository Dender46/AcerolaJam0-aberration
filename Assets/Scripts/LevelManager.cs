using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<TextAsset> _levelInfos = new();

    private int _currentLevel = 0;
    private int _currentItemIndex = 0;
    private LevelInfo _currentLevelInfo;

    [Serializable]
    public struct LevelInfo
    {
        [Serializable]
        public enum ItemType
        {
            Regular,
            Damage,
            Heal,
            Defence,
            Enemy
        }

        public List<ItemType> items;

        public List<string> regularItems;
        public List<string> damageItems;
        public List<string> healItems;
        public List<string> defenceItems;
        public List<string> enemyItems;
    }

    private void Awake()
    {
        var levelInfo = _levelInfos[_currentLevel];
        _currentLevelInfo = JsonUtility.FromJson<LevelInfo>(levelInfo.ToString());
    }

    public GameObject GetNextItem()
    {
        if (_currentItemIndex >= _currentLevelInfo.items.Count)
        {
            return null;
        }
        GameObject result;
        string itemResourceStr = "ConveyorItems/";
        var itemType = _currentLevelInfo.items[_currentItemIndex];
        switch (itemType)
        {
            case LevelInfo.ItemType.Regular:
                itemResourceStr += "Regular/" + GetRandomFromList(_currentLevelInfo.regularItems);
                break;
            case LevelInfo.ItemType.Damage:
                itemResourceStr += "Damage/" + GetRandomFromList(_currentLevelInfo.damageItems);
                break;
            case LevelInfo.ItemType.Heal:
                itemResourceStr += "Heal/" + GetRandomFromList(_currentLevelInfo.healItems);
                break;
            case LevelInfo.ItemType.Defence:
                itemResourceStr += "Defence/" + GetRandomFromList(_currentLevelInfo.defenceItems);
                break;
            case LevelInfo.ItemType.Enemy:
                itemResourceStr += "Enemy/" + GetRandomFromList(_currentLevelInfo.enemyItems);
                break;
        }
        Debug.Log(itemResourceStr);
        result = Resources.Load<GameObject>(itemResourceStr);
        _currentItemIndex++;

        return result;
    }

    private string GetRandomFromList(List<string> list)
    {
        return list[Random.Range(0, list.Count)];
    }
}
