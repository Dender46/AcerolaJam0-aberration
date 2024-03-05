using System;
using UnityEngine;

public class EquipableBehaviour : MonoBehaviour
{
    [Serializable]
    public class EquipableStats
    {
        public enum Type
        {
            None,
            Heal,
            Damage,
            Defence,
        }

        public Type type;
        public int damage = 10;
        public int defence = 2;

        public int restoreHp = 2;
        public int haveUses = 2;
    }

    public EquipableStats itemStats;
    public Sprite itemPreview;

    private void OnEnable()
    {
        if (itemStats != null && itemStats.type == EquipableStats.Type.None)
        {
            throw new UnityException("EquipableStats.Type.None of " + gameObject.name);
        }
    }
}
