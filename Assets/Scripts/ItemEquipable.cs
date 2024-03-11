using System;
using UnityEngine;

public class ItemEquipable : MonoBehaviour
{
    [Serializable]
    public class EquipableInfo
    {
        [Flags]
        public enum Type
        {
            None    = 0,
            Heal    = 1,
            Damage  = 1 << 1,
            Defence = 1 << 2,
        }

        public string name;
        public Type type;
        [HideInInspector] public Sprite preview; // asigned from this component OnEnable()

        [Space(5)]
        public int damage = 10;
        public int defence = 2;

        [Space(5)]
        public int restoreHp = 2;
        public int restoreDp = 2;

        [Space(5)]
        public int cost = 2;
        public int haveUses = 2;
    }

    public EquipableInfo info;
    public Sprite itemPreview;

    private void OnEnable()
    {
        info.preview = itemPreview;
        if (info != null && info.type == EquipableInfo.Type.None)
        {
            throw new UnityException("EquipableStats.Type.None of " + gameObject.name);
        }
    }
}
