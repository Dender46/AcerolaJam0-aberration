using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private RectTransform _taskCanvas;
    [SerializeField] private TMP_Text _taskDescription;
    [SerializeField] private List<TextAsset> _levelInfos = new();

    private string _taskDescriptionStr = "Decline items which are spoiled <color=#0083BB>beyond <SPOIL_TARGET>%</color>\r\n";

    private int _currentSpoilTarget = -1;
    private int _currentLevel = -1;
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

        public int isTutorial;
        public List<int> pricesRange;
        public List<int> spoilRangeTarget;

        public float spoiledChance;
        public List<ItemType> items;

        public List<string> regularItems;
        public List<string> damageItems;
        public List<string> healItems;
        public List<string> defenceItems;
        public List<string> enemyItems;
    }

    public float currentSpoilTarget => _currentSpoilTarget;
    public int currentLevel => _currentLevel;
    public bool levelIsTutorial => _currentLevelInfo.isTutorial == 1;

    public static LevelManager instance { private set; get; }

    private void Awake()
    {
        MoveToNextLevel();

        instance = this;
    }

    public void MoveToNextLevel()
    {
        _currentItemIndex = 0;
        _currentLevel++;
        var levelInfoJson = _levelInfos[_currentLevel].ToString();
        _currentLevelInfo = JsonUtility.FromJson<LevelInfo>(levelInfoJson);

        if (_currentLevelInfo.spoilRangeTarget.Count == 0)
        {
            _taskCanvas.gameObject.SetActive(false);
            _currentSpoilTarget = -1;
        }
        else
        {
            _taskCanvas.gameObject.SetActive(true);
            _currentSpoilTarget = _currentLevelInfo.spoilRangeTarget[Random.Range(0, _currentLevelInfo.spoilRangeTarget.Count)];
            var textWithSpoilTarget = _taskDescriptionStr.Replace("<SPOIL_TARGET>", _currentSpoilTarget.ToString());
            _taskDescription.text = textWithSpoilTarget;
        }
    }

    public int GetPossibleFixedPrice()
    {
        return _currentLevelInfo.pricesRange.Count != 0
            ? _currentLevelInfo.pricesRange[Random.Range(0, _currentLevelInfo.pricesRange.Count)]
            : -1;
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

        result = Resources.Load<GameObject>(itemResourceStr);
        if (!result)
        {
            Debug.LogError("Uknown prefab in level info: " + itemResourceStr);
        }

        _currentItemIndex++;

        return result;
    }

    private string GetRandomFromList(List<string> list)
    {
        return list[Random.Range(0, list.Count)];
    }
}
