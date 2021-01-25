using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class DiscSlice : MonoBehaviour
{
    private RectTransform _icon;

    private DiscMenu _discMenu;
    private RectTransform _rtOut;
    private RectTransform _rtIn;
    private Vector2 _iconDir;

    private Image _img;
    private float _maxAngle;
    private float _minAngle;
    private bool _hover;


    // Inits the disc slice
    public float InitDiskSlice(int data, DiscMenu discMenu) {
        _discMenu = discMenu;
        transform.SetParent(_discMenu.transform);
        transform.localScale = Vector3.one;

        _rtOut = transform.GetChild(0).GetComponent<RectTransform>();
        _rtIn = GetComponent<RectTransform>();

        Image imgIn = _rtIn.GetComponent<Image>();
        imgIn.rectTransform.localPosition = Vector2.zero;
        _img = _rtOut.GetComponent<Image>();
        
        _icon = transform.GetChild(1).GetComponent<RectTransform>();
        _icon.GetComponent<Image>().sprite = _discMenu.DiscDatas[data].Icon;

        ResetSlice();

        float fill = 1f / _discMenu.DiscDatas.Length;
        float angle = fill * 360;
        float rot = Mathf.Clamp(angle * (data + 1) , 0, 360);
        if (rot == 360) {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            _minAngle = 360f - (fill * 360);
            _maxAngle = 360f;
        }
        else {
            transform.rotation = Quaternion.Euler(0, 0, rot);
            _minAngle = rot - (fill * 360);
            _maxAngle = rot;
        }

        return angle;
    }

    // Checks the angle, if in interval
    public void CheckHover(float angle) {
        if (angle > _minAngle && angle < _maxAngle) {
            MouseOn();
        }
        else {
            MouseOut();
        }

        float dist = (_rtOut.sizeDelta.x + _rtIn.sizeDelta.x) / 4f; 
        _icon.localPosition = _iconDir * dist;
    }

    public void MouseOn() {
        if (_hover) {
            return;
        }
        _hover = true;

        float sizeOut = 100f * _discMenu.OuterRadius + 50f * _discMenu.HoverScale;
        float sizeIn = 100f * _discMenu.InnerRadius + 50f * _discMenu.HoverScale;
        Tween tween;
        
        tween = _rtOut.DOSizeDelta(Vector2.one * sizeOut, _discMenu.HoverTime);
        tween.SetEase(_discMenu.HoverInEase);
    
        tween = _rtIn.DOSizeDelta(Vector2.one * sizeIn, _discMenu.HoverTime); 
        tween.SetEase(_discMenu.HoverInEase);
        
        tween = _img.DOColor(_discMenu.HoverColor, _discMenu.HoverTime);
        tween.SetEase(_discMenu.HoverInEase);
    }
    
    private void MouseOut() {
        if (!_hover) {
            return;
        }
        _hover = false;

        float sizeOut = 100f * _discMenu.OuterRadius;
        float sizeIn = 100f * _discMenu.InnerRadius;
        Tween tween;

        tween = _rtOut.DOSizeDelta(Vector2.one * sizeOut, _discMenu.HoverTime);
        tween.SetEase(_discMenu.HoverOutEase);

        tween = _rtIn.DOSizeDelta(Vector2.one * sizeIn, _discMenu.HoverTime); 
        tween.SetEase(_discMenu.HoverOutEase);

        tween = _img.DOColor(_discMenu.NormalColor, _discMenu.HoverTime * 2);
        tween.SetEase(_discMenu.HoverOutEase);
        
    }

    // When slice count is not changed
    public void ResetSlice() {
        _rtOut.sizeDelta = Vector2.one * 100f * _discMenu.OuterRadius;
        _rtIn.sizeDelta = Vector2.one * 100f * _discMenu.InnerRadius;
        _img.fillAmount = 1f / _discMenu.DiscDatas.Length;
        _img.color = _discMenu.NormalColor;

        if (_hover) {
            _rtOut.sizeDelta += Vector2.one * 50 * _discMenu.HoverScale;
            _rtIn.sizeDelta += Vector2.one * 50f * _discMenu.HoverScale;
            _img.color = _discMenu.HoverColor;
        }

        float angle = 1f / _discMenu.DiscDatas.Length * 360;
        float dist = (_rtOut.sizeDelta.x + _rtIn.sizeDelta.x) / 4f;
        _iconDir.x = Mathf.Cos(-(angle / 2f) * Mathf.Deg2Rad);
        _iconDir.y = Mathf.Sin(-(angle / 2f) * Mathf.Deg2Rad);
        _icon.localPosition = _iconDir * dist;   
    }

    public void SetHover() {
        _hover = true;
    }
}