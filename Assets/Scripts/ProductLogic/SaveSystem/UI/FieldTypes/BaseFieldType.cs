public abstract class BaseFieldType
{
    public string Name;
    public string Description;
    public bool Tooltip;
    public string TooltipText;

    public FieldIconType IconType;

    public BaseFieldType(string name, string description, bool tooltip, FieldIconType icon, string tooltipText = "")
    {
        Name = name;
        Description = description;
        Tooltip = tooltip;
        TooltipText = tooltipText;
        IconType = icon;
    }
}
