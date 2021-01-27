using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class DiscSlice : MonoBehaviour
{
    private Image _icon;
    private DiscData _discData = new DiscData();

    private DiscMenu _discMenu;
    private RectTransform _rtOut;
    private RectTransform _rtIn;
    private Vector2 _iconDir;

    private Image _img;
    private float _maxAngle;
    private float _minAngle;
    private bool _hover;


    // Inits the disc slice
    public void InitDiskSlice(int data, DiscMenu discMenu) {
        _discMenu = discMenu;
        _discData.SetData(_discMenu.DiscDatas[data]);

        transform.SetParent(_discMenu.transform);
        transform.localScale = Vector3.one;

        _rtOut = transform.GetChild(0).GetComponent<RectTransform>();
        _rtIn = GetComponent<RectTransform>();

        Image imgIn = _rtIn.GetComponent<Image>();
        imgIn.rectTransform.localPosition = Vector2.zero;
        _img = _rtOut.GetComponent<Image>();
        
        _icon = transform.GetChild(1).GetComponent<Image>();
        _icon.sprite = _discData.Icon;

        ResetSlice(true, false);

        float fill = 1f / _discMenu.DiscDatas.Length;
        float rot = Mathf.Clamp(fill * 360 * (data + 1) , 0, 360);
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

        _icon.transform.rotation = Quaternion.identity;
    }

    public void ManualUpdate() {
        float dist = (_rtOut.sizeDelta.x + _rtIn.sizeDelta.x) / 4f; 
        _icon.transform.localPosition = _iconDir * dist;

        if (_discMenu.Opened) {   
            Vector2 dir = Input.mousePosition - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle = (angle + 360) % 360;
            if (angle > _minAngle && angle < _maxAngle) {
                MouseOn();
            }
            else {
                MouseOut();
            }
        }
    }

    // Checks the angle, if in interval
    public bool IsHover() {
        return _hover;
    }

    public DiscData GetData() {
        return _discData;
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

        tween = FadeIcon(1, _discMenu.HoverTime * 2);

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

        tween = FadeIcon(_discMenu.HoverColor.a, _discMenu.HoverTime * 2);
    }

    public void SetRtOutSize(float radius) {
        _rtOut.sizeDelta = Vector2.one * 100 * radius;
    }

    public void SetRtInSize(float radius) {
        _rtIn.sizeDelta = Vector2.one * 100 * radius;
    }

    public Tween ScaleRtOut(float radius, float time) {
        return _rtOut.DOSizeDelta(Vector2.one * 100f * radius, time);
    }

    public Tween ScaleRtIn(float radius, float time) {
        return _rtIn.DOSizeDelta(Vector2.one * 100f * radius, time);
    }

    public void SetImgAlpha(float alpha) {
        Color color = _img.color;
        color.a = alpha * _discMenu.NormalColor.a;
        _img.color = color;
    }

    public void SetIconAlpha(float alpha) {
        Color color = _icon.color;
        color.a = alpha;
        _icon.color = color;
    }

    public Tween FadeImage(float alpha, float time) {
        return _img.DOFade(alpha * _discMenu.NormalColor.a, time);
    }

    public Tween FadeIcon(float alpha, float time) {
        return _icon.DOFade(alpha, time);
    }

    // When slice count is not changed
    public void ResetSlice(bool active, bool hover) {
        SetRtOutSize(_discMenu.OuterRadius);
        SetRtInSize(_discMenu.InnerRadius);
        _img.fillAmount = 1f / _discMenu.DiscDatas.Length;
        _img.color = _discMenu.NormalColor;
        SetImgAlpha(1);
        SetIconAlpha(_discMenu.HoverColor.a);

        if (hover) {
            _img.color = _discMenu.HoverColor;
            SetIconAlpha(1);
            SetRtOutSize(_discMenu.OuterRadius + 0.5f * _discMenu.HoverScale);
            SetRtInSize(_discMenu.InnerRadius + 0.5f * _discMenu.HoverScale);
        }
        
        float angle = 1f / _discMenu.DiscDatas.Length * 360;
        float dist = (_rtOut.sizeDelta.x + _rtIn.sizeDelta.x) / 4f;
        _iconDir.x = Mathf.Cos(-(angle / 2f) * Mathf.Deg2Rad);
        _iconDir.y = Mathf.Sin(-(angle / 2f) * Mathf.Deg2Rad);
        _icon.transform.localPosition = _iconDir * dist;   
        
        _hover = false;

        gameObject.SetActive(active);
    }

    public void SetHover() {
        _rtOut.sizeDelta += Vector2.one * 50 * _discMenu.HoverScale;
        _rtIn.sizeDelta += Vector2.one * 50f * _discMenu.HoverScale;
        _img.color = _discMenu.HoverColor;
    }
}