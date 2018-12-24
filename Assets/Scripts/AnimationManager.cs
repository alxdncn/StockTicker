using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	// Use this for initialization
	void Start () {
		DataRequest.Instance.ShowNextStock();
	}
	
	// Update is called once per frame
	void Update () {
		if(objectToAnimate != null){
			if(animTimer > animTimes[animationIndex]){
				animationIndex++;
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
	}

	public void StartAnimation(Transform trans){
		animTimer = 0f;
		objectToAnimate = trans;
		endPos[endPos.Length - 1].x = endXPos;
		// startPos = objectToAnimate.position;
	}


}
