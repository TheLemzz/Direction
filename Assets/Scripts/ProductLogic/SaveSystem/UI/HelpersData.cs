
using UnityEngine;

[System.Serializable]
public struct HelpersData
{
    [ConstantsDropper(typeof(Constants))] public string FileType;
    public MonoBehaviour Component;
}
