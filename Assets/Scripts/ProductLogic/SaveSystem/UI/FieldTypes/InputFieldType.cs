public class InputFieldType : BaseFieldType
{
    public string DefaultText;

    public InputFieldType(string name, string description, bool tooltip, FieldIconType icon, string defaultText, string tooltipText = "") : base(name, description, tooltip, icon, tooltipText)
    {
        DefaultText = defaultText;
    }
}
