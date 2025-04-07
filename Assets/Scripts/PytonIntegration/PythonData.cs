using UnityEngine;

[System.Serializable]
[Icon("Assets/Design/Python.png")]
public class PythonData
{
    [Tooltip("������ ���� � .py �������.")] public string ScriptPath;
    [Tooltip("�������� output ����� ���������� ������ ���������? Editor only.")] public bool ShowOutput;
    [Tooltip("��������� ������ ��� ���� �������?")] public bool HideConsole = true;
    [Tooltip("����������� �� ������ ����������� �� ���������� ���� ���������? ���� ��, �� ��������� ������� ������� ����������, ���� ����� ��������� ����-���������.")] public bool Eternal;
    [Tooltip("������� ����������, ���� ����� ��������� ����-��������� ��� ���������� ������ �������. ����� �������� ������, ���� Eternal - false.")] public string WorkPath;
    [Space, TextArea, Tooltip("���������, ��������� ��� ������� Python-�������. ���������� ����� �������.")] public string Args;
}
