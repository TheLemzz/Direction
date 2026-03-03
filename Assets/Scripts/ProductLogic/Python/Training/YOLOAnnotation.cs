/// <summary>
/// Class for serialize YOLO-like annotations (.txt).
/// </summary>
[System.Serializable]
public struct YOLOAnnotation
{
    public int ClassId;
    public float CenterX;
    public float CenterY;
    public float Width;
    public float Height;
}