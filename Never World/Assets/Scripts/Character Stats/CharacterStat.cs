using System.Collections.Generic;
using System;

public class CharacterStat {

	public float BaseValue;

	public readonly List<StatModifier> statModifiers;
	private bool hasChanged = true;
	private float _value;

	public float Value {
		get{
			if(!hasChanged)return _value;
			else{
				_value = CalculateFinalValue();
				hasChanged = false;
				return _value;
			}
		}
	}

	public CharacterStat(float baseValue){
		this.BaseValue = baseValue;
		statModifiers = new List<StatModifier>();
	}

	public void AddModifier(StatModifier modifier){
		hasChanged = true;
		this.statModifiers.Add(modifier);
		this.statModifiers.Sort(CompareModifierOrder);
	}

	private int CompareModifierOrder(StatModifier a, StatModifier b){
		//first check by order
		if(a.Order < b.Order)return -1;
		else if(a.Order > b.Order)return 1;
		else {//then check by type
			/*if((int)a.Type < (int)b.Type) return -1;
			else if((int)a.Type > (int)b.Type) return 1;
			else return 0;*/
			return 0;
		}
	}

	public bool RemoveModifier(StatModifier modifier){
		hasChanged = true;
		return statModifiers.Remove(modifier);
	}

	private float CalculateFinalValue(){
		float finalValue = BaseValue;
		for(int i=0;i<statModifiers.Count;i++){
			StatModifier modifier = statModifiers[i];
			if(modifier.Type == StatModifierType.Flat){
				finalValue += modifier.Value;
			} else if(modifier.Type == StatModifierType.Percent_FROM_BASE) {
				finalValue += (BaseValue * modifier.Value);
			} else {
				finalValue *= 1+ modifier.Value;
			}
		}
		return (float)Math.Round(finalValue,4);
	}
}
