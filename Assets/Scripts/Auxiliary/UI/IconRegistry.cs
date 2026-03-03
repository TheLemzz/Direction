using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IconRegistry", menuName = "Settings/IconRegistry")]
public class IconRegistry : ScriptableObject
{
    [System.Serializable]
    public struct IconEntry
    {
        public FieldIconType Type;
        public Sprite Icon;
    }

    [SerializeField] private List<IconEntry> _icons;

    private Dictionary<FieldIconType, Sprite> _lookup;

    public Sprite GetIcon(FieldIconType type)
    {
        if (type == FieldIconType.None) return null;

        if (_lookup == null)
        {
            _lookup = new Dictionary<FieldIconType, Sprite>();
            foreach (var entry in _icons)
            {
                if (!_lookup.ContainsKey(entry.Type))
                {
                    _lookup.Add(entry.Type, entry.Icon);
                }
            }
        }

        return _lookup.TryGetValue(type, out var sprite) ? sprite : null;
    }
}