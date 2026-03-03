using System;
using UnityEngine;

[Icon("Assets/Logo/direction.jpg")]
[CreateAssetMenu(fileName = "ProductInfo", menuName = "Product/ProductInfo")]
public sealed class TestingProductInfo : ScriptableObject
{
    [Header("Общие настройки:")]
    [Space(5), SerializeField] private string _productName;
    [SerializeField, TextArea] private string _productDescription;
    [SerializeField] private string _productVersion;
    [SerializeField, Range(0, 20)] private int _sceneIndex;

    [Space(25), Header("Настройки сохранения:")]
    [Space(5), ConstantsDropper(typeof(Constants)), SerializeField] private string _saveType;

    public int SceneIndex => _sceneIndex;
    public string ProductName => _productName;
    public string ProductDescription => _productDescription;
    public string ProductVersion => _productVersion;
    public string SaveType => _saveType;
}