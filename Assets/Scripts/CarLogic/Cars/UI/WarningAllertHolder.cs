using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class WarningAllertHolder : MonoBehaviour
{
    [SerializeField] private WarningAllert _allertPrefab;

    private readonly Dictionary<string, WarningAllert> _warnings = new();

    public WarningAllert AddWarningAllert(string description)
    {
        if (IsHasWarningWithDescription(description)) return _warnings[description];

        WarningAllert allert = Instantiate(_allertPrefab, transform);
        _warnings[description] = allert;
        return allert.SetText(description);
    }

    public bool IsHasWarningWithDescription(string description)
    {
        return _warnings.Keys.Contains(description);
    }

    public void TryRemoveWarningAllert(string data)
    {
        if (!IsHasWarningWithDescription(data)) return;

        Destroy(_warnings[data].gameObject);
        _warnings.Remove(data);
    }
}
