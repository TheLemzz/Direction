public class ScrollViewFieldType : BaseFieldType
{
    public string FieldName;

    public ScrollViewFieldType(string name, string description, bool tooltip, FieldIconType icon, string fieldName) : base(name, description, tooltip, icon)
    {
        FieldName = fieldName;
    }
}
