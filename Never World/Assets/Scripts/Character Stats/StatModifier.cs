public enum StatModifierType{
	Flat,
	Percent_FROM_BASE,
	Percent
}

public class StatModifier{

	public readonly float Value;
	public readonly StatModifierType Type;
	public readonly int Order;

	public StatModifier(float value, StatModifierType type, int order){
		this.Value = value;
		this.Type = type;
		this.Order = order;
	}

	public StatModifier(float value, StatModifierType type): this(value,type,(int)type){}

}
