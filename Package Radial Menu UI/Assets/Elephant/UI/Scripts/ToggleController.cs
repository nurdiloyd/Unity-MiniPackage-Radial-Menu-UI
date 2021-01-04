using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
	public  bool isOn;

	public Sprite toggleOnBGImage;
	public Sprite toggleOffBGImage;
	
	public Image toggleBgImage;
	public RectTransform toggle;

	public RectTransform handle;

	private float handleSize;
	private float onPosX;
	private float offPosX;

	public float handleOffset;
	

	public float speed;
	static float t = 0.0f;

	private bool switching = false;


	public void SetupToggle()
	{
		StartCoroutine(Setup());

	}

	public IEnumerator Setup()
	{
		
		yield return new WaitForEndOfFrame();

		
		handleSize = handle.rect.width;
		float toggleSizeX = toggle.rect.width;
		
		onPosX = toggleSizeX * 0.5f - handleSize * 0.5f - handleSize * 0.1f;
		
		offPosX = onPosX * -1;

		if(isOn)
		{
			toggleBgImage.sprite = toggleOnBGImage;
			handle.localPosition = new Vector3(onPosX, 0f, 0f);
		}
		else
		{
			toggleBgImage.sprite = toggleOffBGImage;
			handle.localPosition = new Vector3(offPosX, 0f, 0f);
		}
		
	}

	
		
	void Update()
	{

		if(switching)
		{
			Toggle(isOn);
		}
	}

	public void DoYourStaff()
	{
//		Debug.Log(isOn);
	}

	public void Switching()
	{
		switching = true;
	}
	
	


	public void Toggle(bool toggleStatus)
	{
//		if(!onIcon.active || !offIcon.active)
//		{
//			onIcon.SetActive(true);
//			offIcon.SetActive(true);
//		}
		
		if(toggleStatus)
		{
			toggleBgImage.sprite = toggleOffBGImage;
			handle.localPosition = SmoothMove(handle.gameObject, onPosX, offPosX);
		}
		else
		{
			toggleBgImage.sprite = toggleOnBGImage;
			handle.localPosition = SmoothMove(handle.gameObject, offPosX, onPosX);
		}
			
	}


	Vector3 SmoothMove(GameObject toggleHandle, float startPosX, float endPosX)
	{
		
		Vector3 position = new Vector3 (Mathf.Lerp(startPosX, endPosX, t += speed * Time.deltaTime), 0f, 0f);
		StopSwitching();
		return position;
	}

	Color SmoothColor(Color startCol, Color endCol)
	{
		Color resultCol;
		resultCol = Color.Lerp(startCol, endCol, t += speed * Time.deltaTime);
		return resultCol;
	}

	CanvasGroup Transparency (GameObject alphaObj, float startAlpha, float endAlpha)
	{
		CanvasGroup alphaVal;
		alphaVal = alphaObj.gameObject.GetComponent<CanvasGroup>();
		alphaVal.alpha = Mathf.Lerp(startAlpha, endAlpha, t += speed * Time.deltaTime);
		return alphaVal;
	}

	void StopSwitching()
	{
		if(t > 1.0f)
		{
			switching = false;

			t = 0.0f;
			switch(isOn)
			{
			case true:
				isOn = false;
				DoYourStaff();
				break;

			case false:
				isOn = true;
				DoYourStaff();
				break;
			}

		}
	}

	
}
