using UnityEngine;

[CreateAssetMenu(fileName = "TrainingData", menuName = "AI/TrainingData")]
public sealed class TrainingData : ScriptableObject
{
    [SerializeField] private string _classification;
    [SerializeField, Range(2, 130)] private float _range;

    public float Range => _range;
    public string Classification => _classification;
}