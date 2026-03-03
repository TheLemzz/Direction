using UnityEngine;

public struct CrackSpawnData
{
    public GameObject Crack;
    public Vector3 WantedPosition;

    public CrackSpawnData(GameObject crack, Vector3 wantedPosition)
    {
        this.Crack = crack;
        this.WantedPosition = wantedPosition;
    }
}
