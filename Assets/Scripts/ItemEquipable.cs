using System;
using UnityEngine;

public class ItemEquipable : MonoBehaviour
{
    [Serializable]
    public class EquipableInfo
    {
        public enum Type
        {
            None,
            Heal,
            Damage,
            Defence,
        }

        public string name;
        [HideInInspector] public Sprite preview; // asigned from this component OnEnable()

        public Type type;
        public int damage = 10;
        public int defence = 2;

        public int restoreHp = 2;
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
