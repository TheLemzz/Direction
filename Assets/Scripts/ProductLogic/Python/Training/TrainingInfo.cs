
public struct TrainingInfo
{
    public float VisibilityRange;
    public uint ClassId;

    public TrainingInfo(float visibilityRange, uint classId)
    {
        VisibilityRange = visibilityRange;
        ClassId = classId;
    }
}
