public class SliderFieldType : BaseFieldType
{
    public int Min;
    public int Max;
    public int Value;
    public bool WholeNumbers;
    public string Symbol;

    public SliderFieldType(string name, string description, bool tooltip, FieldIconType icon, int min, int max, int defaultValue, bool wholeNumbers, string symbol = "", string tooltipText = "") : base(name, description, tooltip, icon, tooltipText)
    {
        Min = min;
        Max = max;
        Value = defaultValue;
        WholeNumbers = wholeNumbers;
        Symbol = symbol;
    }
}
