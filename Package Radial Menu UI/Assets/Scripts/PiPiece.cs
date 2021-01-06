using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class PiPiece : MonoBehaviour
{
    private bool isOver;
    private Image thisImg;

    [HideInInspector] [SerializeField] private HueColor hue;
    [HideInInspector] [SerializeField] private Color normalColor;
    [HideInInspector] [SerializeField] private Color disabledColor = Color.black;
    [SerializeField] private Color materialColor;
    [HideInInspector] [SerializeField] private UnityAction<int> clickEvent;
    [SerializeField] [HideInInspector] private PiUI parent;

    private float maxAngle;
    private float minAngle;
    private bool isInteractable;
    private bool lastFrameIsOver;


    private void Start() {
        thisImg = GetComponent<Image>( );
        thisImg.color = normalColor;
        transform.localScale = Vector2.one;
    }

    public void ManualUpdate() {
        Vector2 inputAxis = parent.joystickInput;
        if (isInteractable) {
            
            if (isOver && transform.localScale.sqrMagnitude < (Vector2.one * parent.hoverScale).sqrMagnitude) {
                transform.localScale = Vector2.Lerp(transform.localScale, Vector2.one * parent.hoverScale, Time.deltaTime * 10f);
            }
            else if (transform.localScale.sqrMagnitude > 1 && !isOver) {
                transform.localScale = Vector2.Lerp(transform.localScale, Vector2.one, Time.deltaTime * 10f);
            }

            Vector2 mousePos = Input.mousePosition;
            Vector2 temp = mousePos - (Vector2)transform.position;
            float angle = (Mathf.Atan2(temp.y, temp.x) * Mathf.Rad2Deg);
            angle = (angle + 360) % 360;

            if (angle < maxAngle && angle > minAngle) {
                isOver = true;
            }
            else if (parent.useController && isInteractable) {
                temp = inputAxis;
                angle = (Mathf.Atan2(temp.y, temp.x) * Mathf.Rad2Deg);
                angle = (angle + 360) % 360;
                if (angle == 0)
                {
                    angle += 1;
                }
                if (angle < maxAngle && angle >= minAngle && inputAxis != Vector2.zero)
                {
                    isOver = true;
                }
                else
                {
                    isOver = false;
                }
            }
            else {
                isOver = false;
            }

            if (!parent.interactable) {
                isOver = false;
            }

            if (isOver && parent.interactable)
            {
                transform.SetAsLastSibling( );
                
                if (Input.GetMouseButtonUp(0) || parent.useController && parent.joystickButton) {
                    clickEvent?.Invoke(1);
                }
            }
        }
        else
        {
            thisImg.color = disabledColor;
            transform.localScale = Vector2.Lerp(transform.localScale, Vector2.one, Time.deltaTime * 10f);
        }
        if (transform.rotation.eulerAngles.z == 359f || transform.rotation.eulerAngles.z == 0)
        {
            transform.rotation = Quaternion.identity;
        }
        if (transform.rotation.eulerAngles.z == 359f || transform.rotation.eulerAngles.z == 0 && parent.openedMenu)
        {
            transform.rotation = Quaternion.identity;
            maxAngle = 359f;
            minAngle = 359f - (thisImg.fillAmount * 360);
        }
        else if (parent.interactable)
        {
            maxAngle = transform.rotation.eulerAngles.z;
            minAngle = transform.rotation.eulerAngles.z - (thisImg.fillAmount * 360);
        }
        
        if (lastFrameIsOver != isOver && isInteractable && parent.interactable) {
            if (isOver) {
                Debug.Log("over");
            }
        }
        
        if (isOver) {
            parent.overMenu = true;
        }
        lastFrameIsOver = isOver;
    }

    public void SetData(PiUI.PiData piData, PiUI creator)
    {
        parent = creator;
        if (!thisImg )
        {
            thisImg = GetComponent<Image>( );
        }

        normalColor = piData.buttonColor;
        materialColor = piData.materialColor;
        clickEvent = piData.onSlicePressed;
        maxAngle = transform.rotation.eulerAngles.z;
        minAngle = transform.rotation.eulerAngles.z - (thisImg.fillAmount * 360);
        if (transform.rotation.eulerAngles.z == 359f || transform.rotation.eulerAngles.z == 0)
        {
            transform.rotation = Quaternion.identity;
            maxAngle = 359f;
            minAngle = 359f - (thisImg.fillAmount * 360);
        }

        isInteractable = piData.isInteractable;
    }
}