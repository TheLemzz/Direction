using TMPro;
using UnityEngine;

public sealed class ProductInfoWrapper : MonoBehaviour
{
    [SerializeField, Tooltip("Информация о продукте...")] private TestingProductInfo _productInfo;

    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _description;

    private void OnValidate()
    {
        UpdateInfo();
    }

    private void Start()
    {
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        _name.text = $"{_productInfo.ProductName} (v. {_productInfo.ProductVersion}):";
        _description.text = _productInfo.ProductDescription;
    }

    public void SetNewProductInfo(TestingProductInfo productInfo)
    {
        _productInfo = productInfo;
        UpdateInfo();
    }
}
