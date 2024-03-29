
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetIconView : BaseUiView
{
    #region PrivateFileds

    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _viewImage;

    #endregion

    #region AccessFields

    public bool isActive => _viewImage?.enabled ?? false;
    public Image Icon => _icon;
    public TextMeshProUGUI Text => _text;
    public Image ViewImage => _viewImage;

    #endregion

    #region AccessMethods
    public void SetText(string levelnumber)
    {
        if (_text && isActive)
        {
            _text.enabled = true;
            _text.text = levelnumber;
        }
    }

    public void SetIcon(Sprite sprite)
    {
        if (_icon && isActive)
        {
            _icon.enabled = true;
            _icon.sprite = sprite;
        }
    }

    public void DisableText()
    {
        if (_text)
        {
            _text.enabled = false;
        }
    }

    public void DisableIcon()
    {
        if (_icon)
        {
            _icon.enabled = false;
        }
    }

    public override void Disable()
    {
        if (_viewImage && isActive)
        {
            _viewImage.enabled = false;
            DisableIcon();
            DisableText();
        }
    }

    public override void Enable()
    {
        if (_viewImage && !isActive)
        {
            _viewImage.enabled = true;
        }
    }

    public void CopyValues(TargetIconView otherView)
    {
        _icon.sprite = otherView.Icon.sprite;
        _text.text = otherView.Text.text;
    }
    #endregion
}