using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScrollViewFieldHolder : BaseFieldHolder
{
    [SerializeField] private ScrollViewField _prefab;
    [SerializeField] private RectTransform _content;
    [SerializeField] private TextMeshProUGUI _emptyText;

    private readonly HashSet<string> _contents = new();

    private Sprite _fieldSprite;

    private string _fieldName;
    private string _fieldDescription;

    public ScrollViewFieldHolder Init(ScrollViewFieldType info, Sprite sprite, VariableReference reference)
    {
        InternalInit(info, sprite, reference);

        _fieldDescription = info.Description;
        _fieldName = info.FieldName;
        _fieldSprite = sprite;

        if (reference.Get() != null)
            foreach (string path in (string[])reference.Get())
            {
                CreateField(path);
                _contents.Add(path);
            }

        return this;
    }

    public void Add()
    {
        CreateField();
    }

    public void Delete(ScrollViewField scrollViewField)
    {
        Destroy(scrollViewField.gameObject);
        OnValueChanged();
    }

    private void CreateField(string value = "")
    {
        Instantiate(_prefab, _content).Init(_fieldName, _fieldDescription, _fieldSprite, this, value).GetField().onEndEdit.AddListener(OnEndEdit);
        _emptyText.gameObject.SetActive(false);
    }

    private void OnEndEdit(string arg0)
    {
        OnValueChanged();
    }

    public override void OnValueChanged()
    {
        _contents.Clear();

        foreach (ScrollViewField item in _content.GetComponentsInChildren<ScrollViewField>(true))
        {
            _contents.Add(item.GetContent());
        }

        if (_content.childCount <= 1) _emptyText.gameObject.SetActive(true);

        _reference.Set(_contents.ToArray());
        Debug.Log($"set new value array: {_contents.ToArray().Length}. Count: {_contents.Count}");
    }
}
