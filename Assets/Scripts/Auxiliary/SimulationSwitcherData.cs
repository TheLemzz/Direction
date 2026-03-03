using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
public struct SimulationSwitcherData
{
    public Transform Pos;
    public PostProcessProfile postProcessProfile;
    public Material Skybox;
    public TestingProductInfo ProductInfo;

    public readonly int SceneIndex => ProductInfo.SceneIndex;
}