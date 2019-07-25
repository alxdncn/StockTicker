using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LookingGlass;

public class AnimationManager : MonoBehaviour {

	private static AnimationManager instance;
	public static AnimationManager Instance
	{
		get
		{
			if (instance != null)
				return instance;

			instance = FindObjectOfType<AnimationManager>();
			return instance;
		}
	}

	[SerializeField] AnimationCurve[] rotateAnimCurve;
	[SerializeField] AnimationCurve[] positionAnimCurve;

	[SerializeField] Vector3[] startPos;
	[SerializeField] Vector3[] endPos;
	[SerializeField] Vector3[] startEulers;
	[SerializeField] Vector3[] endEulers;

	[SerializeField] float[] animTimes;

	public float endXPos = 70;

	int animationIndex = 0;
	float animTimer = 0f;

	Transform objectToAnimate;

	int pauseIndex = 1;
	bool goToNext = true;
	float goToNextRegularTime;

	// Use this for initialization
	void Start () {
		goToNextRegularTime = animTimes[pauseIndex];
		DataRequest.Instance.ShowNextStock();
	}
	
	// Update is called once per frame
	void Update () {
		if(objectToAnimate != null){
			if(animTimer > animTimes[animationIndex]){
				animationIndex++;
				// if(animationIndex == pauseIndex + 1){
				// 	if(goToNext)
				// 		animTimes[pauseIndex] = goToNextRegularTime;
				// 	else
				// 		animTimes[pauseIndex] = Mathf.Infinity;
				// }
				animTimer = 0f;
				if(animationIndex >= animTimes.Length){
					animationIndex = 0;
					DataRequest.Instance.ShowNextStock();
					Destroy(objectToAnimate.gameObject);
					objectToAnimate = null;
				}
				return;
			}
			float tPos = positionAnimCurve[animationIndex].Evaluate(animTimer/animTimes[animationIndex]);
			float tRot = rotateAnimCurve[animationIndex].Evaluate(animTimer/animTimes[animationIndex]);
			objectToAnimate.position = Vector3.Lerp(startPos[animationIndex], endPos[animationIndex], tPos);
			objectToAnimate.rotation = Quaternion.Euler(Vector3.Lerp(startEulers[animationIndex], endEulers[animationIndex], tRot));
			animTimer += Time.deltaTime;
		}

		if(ButtonManager.GetButtonDown(ButtonType.SQUARE) || ButtonManager.GetButtonDown(ButtonType.CIRCLE)){
			goToNext = !goToNext;
			if(!goToNext){
				animTimes[pauseIndex] = Mathf.Infinity;
			} else{
				animTimes[pauseIndex] = goToNextRegularTime;
			}
		}
		if(ButtonManager.GetButtonDown(ButtonType.LEFT)){
			if(animationIndex == pauseIndex){
				DataRequest.Instance.stockIndex -= 2;
				if(DataRequest.Instance.stockIndex < 0){
					DataRequest.Instance.stockIndex = StockListHolder.symbolsUS.Length - 2;
				}
				animationIndex ++;
				animTimer = 0f;
			}
		}
		if(ButtonManager.GetButtonDown(ButtonType.RIGHT)){
			if(animationIndex == pauseIndex){
				animationIndex ++;
				animTimer = 0f;
			}
		}
	}

	public void StartAnimation(Transform trans){
		animTimer = 0f;
		objectToAnimate = trans;
		endPos[endPos.Length - 1].x = endXPos;
		// startPos = objectToAnimate.position;
	}


}
