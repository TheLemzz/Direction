using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewField : MonoBehaviour
{
    [SerializeField] private TMP_InputField _field;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Image _image;

    private ScrollViewFieldHolder _holder;

    public ScrollViewField Init(string name, string description, Sprite icon, ScrollViewFieldHolder holder, string value = "")
    {
        _image.sprite = icon;
        _name.text = name;
        _description.text = description;
        _field.text = value;
        _holder = holder;

        return this;
    }

    public TMP_InputField GetField()
    {
        return _field;
    }

    public string GetContent()
    {
        return _text.text.Sanitize();
    }

    public void Delete()
    {
        _holder.Delete(this);
    }
}
