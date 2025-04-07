using UnityEngine;

[Icon("Assets/Logo/direction.jpg")]
[CreateAssetMenu(fileName = "ProductInfo", menuName = "Product/ProductInfo")]
public sealed class TestingProductInfo : ScriptableObject
{
    [SerializeField] private string _productName;
    [SerializeField] private string _productDescription;
    [SerializeField] private string _productVersion;

    public string ProductName => _productName;
    public string ProductDescription => _productDescription;
    public string ProductVersion => _productVersion;
}
