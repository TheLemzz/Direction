using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public sealed class WarningAllert : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private RawImage _image;

    public WarningAllert SetText(string text)
    {
        _text.text = text;
        //StartCoroutine(AnimationCoroutine());
        return this;
    }

    private IEnumerator AnimationCoroutine()
    {
        WaitForSeconds wait = new(0.5f);

        while (true)
        {
            print('a');
            yield return wait;
            _text.enabled = !_text.enabled;
            _image.enabled = !_image.enabled;
        }
    }
}
