using System;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

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

        [HideInInspector] public string name;
        [HideInInspector] public Sprite preview;

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
        info.name = gameObject.name.Substring(1, gameObject.name.Length - gameObject.name.LastIndexOf("(Clone)"));
        info.preview = itemPreview;
        if (info != null && info.type == EquipableInfo.Type.None)
        {
            throw new UnityException("EquipableStats.Type.None of " + gameObject.name);
        }
    }
}
