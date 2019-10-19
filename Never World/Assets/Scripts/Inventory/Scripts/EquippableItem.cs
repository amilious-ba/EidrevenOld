using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType {Helmet, Chest, Gloves, Boots, Weapon1, Weapon2, Accessory1, Acessory2}

[CreateAssetMenu]
public class EquippableItem : Item {
    
    public int StrengthBonus;
    public int AgilityBonus;
    public int IntelligenceBonus;
    public int VitalityBonus;
    [Space]
    public float StrengthPercentBonus;
    public float AgilityPercentBonus;
    public float IntelligencePercentBonus;
    public float VitalityPercentBonus;
    [Space]
    public EquipmentType EquipmentType;

}
