using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private RectTransform _taskCanvas;
    [SerializeField] private TMP_Text _taskTitle;
    [SerializeField] private TMP_Text _taskDescription;
    [SerializeField] private TextAsset _allObjectivesJson;
    [SerializeField] private List<TextAsset> _levelInfos = new();

    [Serializable]
    public struct SerializedObjective
    {
        [Serializable]
        public struct Info
        {
            public int id;
            public string text;
        }

        public List<Info> info;
    }

    public class Objective
    {
        [Serializable]
        public enum Type { ApproveAll, SpoilTarget, SpecificItems }
        public Type type;

        public bool isApprove = true;
        public int spoilTarget = -1;

        public List<string> specificItems = new();
    }

    private SerializedObjective _serializedObjectives;
    private List<Objective> _objectives = new();


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
            Enemy,
        }

        public int isTutorial;
        public int removeDeclineButton;
        public List<int> pricesRange;
        public List<int> spoilRangeTarget;

        public List<Objective.Type> objectives;
        public List<ItemType> items;
        public float spoiledChance;

        public List<string> regularItems;
        public List<string> damageItems;
        public List<string> healItems;
        public List<string> defenceItems;
        public List<string> enemyItems;
    }

    public int currentLevel => _currentLevel;
    public bool levelIsTutorial => _currentLevelInfo.isTutorial == 1;
    public bool removeDeclineButton => _currentLevelInfo.removeDeclineButton == 1;

    public static LevelManager instance { private set; get; }

    private void Awake()
    {
        _serializedObjectives = JsonUtility.FromJson<SerializedObjective>(_allObjectivesJson.text);
        MoveToNextLevel();

        instance = this;
    }

    public void MoveToNextLevel()
    {
        _currentItemIndex = 0;
        _currentLevel++;
        var levelInfoJson = _currentLevel < _levelInfos.Count 
            ? _levelInfos[_currentLevel].text 
            : _levelInfos[_levelInfos.Count-1].text;
        _currentLevelInfo = JsonUtility.FromJson<LevelInfo>(levelInfoJson);

        // set objectives
        _objectives.Clear();
        var isApprove = Random.Range(0.0f, 1.0f) > 0.5f;
        foreach (var objectiveType in _currentLevelInfo.objectives)
        {
            var newObjective = new Objective {
                type = objectiveType,
                isApprove = isApprove,
            };
            switch (objectiveType)
            {
                case Objective.Type.ApproveAll:
                    break;
                case Objective.Type.SpoilTarget:
                    newObjective.spoilTarget = GetRandomFromList(_currentLevelInfo.spoilRangeTarget);
                    break;
                case Objective.Type.SpecificItems:
                    var randomIndices = GetRandomIndices(_currentLevelInfo.regularItems.Count);
                    foreach (var index in randomIndices)
                    {
                        newObjective.specificItems.Add(_currentLevelInfo.regularItems[index]);
                    }
                    break;
            }
            _objectives.Add(newObjective);
        }

        // update objective screen
        var allObjectivesText = "";
        var approveOrDeclineStr = isApprove ? "<color=#0fac44>Approve</color>" : "<color=#d32d2d>Decline</color>";
        for (int i = 0; i < _objectives.Count; i++)
        {
            allObjectivesText += i + 1 + ". ";

            var objective = _objectives[i];
            string description = _serializedObjectives.info[(int)objective.type].text;
            switch (objective.type)
            {
                case Objective.Type.ApproveAll: break;
                case Objective.Type.SpoilTarget:
                    description = description.Replace("<USER_CHOICE>", approveOrDeclineStr);
                    description = description.Replace("<SPOIL_TARGET>", objective.spoilTarget.ToString());
                    break;
                case Objective.Type.SpecificItems:
                    string itemsStr = "";
                    foreach (var item in objective.specificItems)
                    {
                        itemsStr += "\n • " + item[1..]; 
                    }
                    // BE CAREAFUL
                    description = description.Replace("<USER_CHOICE>", _objectives.Count == 1 ? approveOrDeclineStr : "<b>AND</b>");
                    description = description.Replace("<SPECIFIC_ITEMS>", itemsStr);
                    break;
            }

            allObjectivesText += description;
            if (i != _objectives.Count - 1)
                allObjectivesText += "\n";
        }

        // Update task screen
        _taskTitle.text = "Day " + (_currentLevel+1) + " Objective:";
        _taskDescription.text = allObjectivesText;
    }

    public bool ItemMeetsObjectives(bool playerApproves, ConveyorItem item)
    {
        var shouldBees = new List<bool>();
        foreach (var objective in _objectives)
        {
            var shouldBe = false;
            switch (objective.type)
            {
                case Objective.Type.ApproveAll: 
                    shouldBe = true;
                    break;
                case Objective.Type.SpoilTarget:
                    var isSpoiled = item.spoilLevel >= objective.spoilTarget;
                    shouldBe = objective.isApprove ? isSpoiled : !isSpoiled;
                    break;
                case Objective.Type.SpecificItems:
                    var isInAList = false;
                    foreach (var itemName in objective.specificItems)
                    {
                        if (item.gameObject.name.Contains(itemName))
                        {
                            isInAList = true;
                            break;
                        }
                    }
                    shouldBe = objective.isApprove ? isInAList : !isInAList;
                    break;
            }
            shouldBees.Add(shouldBe);
        }

        // two mission results must match
        var finalResultShouldBe = shouldBees.TrueForAll((val) => val == true);
        return playerApproves == finalResultShouldBe;
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

    private List<int> GetRandomIndices(int listCount)
    {
        var availableIndices = new List<int>();
        for (int i = 0; i < listCount; i++)
            availableIndices.Add(i);

        var resultIndices = new List<int>();
        var resultCount = listCount / 2;
        for (int i = 0; i < resultCount; i++)
        {
            var index = Random.Range(0, availableIndices.Count);
            resultIndices.Add(availableIndices[index]);
            availableIndices.RemoveAt(index);
        }
        return availableIndices;
    }

    private int GetRandomFromList(List<int> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    private string GetRandomFromList(List<string> list)
    {
        return list[Random.Range(0, list.Count)];
    }
}
