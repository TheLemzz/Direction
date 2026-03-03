using UnityEngine;

[System.Serializable]
public struct TrainingOptions
{
    public TrainingData TrainingData;
    [Range(2, 130)] public float Range;
}