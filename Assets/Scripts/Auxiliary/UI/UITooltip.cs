using UnityEngine;

public sealed class UITooltip : MonoBehaviour
{
    private SmoothAlphaController _alphaController;
    private ElementSizer _sizer;

    private void Start()
    {
        _alphaController = GetComponent<SmoothAlphaController>();
        _sizer = GetComponent<ElementSizer>();
        _alphaController.FadeOut();
    }

    public void ChangeVisibility()
    {
        _alphaController.FadeIn();
    }
}
