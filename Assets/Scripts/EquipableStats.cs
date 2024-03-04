using System;

[Serializable]
public class EquipableStats
{
    public enum Type
    {
        Heal,
        Damage,
        Defence,
    }

    public int damage = 10;
    public int defence = 2;

    public int restoreHp = 2;
    public int haveUses = 2;
}
