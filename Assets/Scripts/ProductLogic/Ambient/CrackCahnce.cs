using UnityEngine;

[System.Serializable]
public struct CrackChance
{
    public GameObject Crack;
    [Range(0, 100)] public int Chance;
}