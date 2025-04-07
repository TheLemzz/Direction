using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class WarningAlertHolder : MonoBehaviour
{
    [SerializeField] private WarningAlert _alertPrefab;

    private readonly Dictionary<string, WarningAlert> _warnings = new();

    public WarningAlert AddWarningAlert(string description)
    {
        if (IsHasWarningWithDescription(description)) return _warnings[description];

        WarningAlert allert = Instantiate(_alertPrefab, transform);
        _warnings[description] = allert;
        return allert.SetText(description);
    }

    public bool IsHasWarningWithDescription(string description)
    {
        return _warnings.Keys.Contains(description);
    }

    public void TryRemoveWarningAlert(string data)
    {
        if (!IsHasWarningWithDescription(data)) return;

        Destroy(_warnings[data].gameObject);
        _warnings.Remove(data);
    }
}
