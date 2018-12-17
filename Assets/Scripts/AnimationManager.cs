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

	[SerializeField] AnimationCurve rotateAnimCurve;
	[SerializeField] AnimationCurve positionAnimCurve;

	[SerializeField] Vector3 startPos;
	[SerializeField] Vector3 endPos;
	[SerializeField] Vector3 startEulers;
	[SerializeField] Vector3 endEulers;

	[SerializeField] float animTime = 5f;
	float animTimer = 0f;

	Transform objectToAnimate;

	// Use this for initialization
	void Start () {
		DataRequest.Instance.ShowNextStock();
	}
	
	// Update is called once per frame
	void Update () {
		if(objectToAnimate != null){
			if(animTimer > animTime){
				DataRequest.Instance.ShowNextStock();
				Destroy(objectToAnimate.gameObject);
				objectToAnimate = null;
				return;
			}
			float tPos = positionAnimCurve.Evaluate(animTimer/animTime);
			float tRot = rotateAnimCurve.Evaluate(animTimer/animTime);
			objectToAnimate.position = Vector3.Lerp(startPos, endPos, tPos);
			objectToAnimate.rotation = Quaternion.Euler(Vector3.Lerp(startEulers, endEulers, tRot));
			animTimer += Time.deltaTime;
		}
	}

	public void StartAnimation(Transform trans){
		animTimer = 0f;
		objectToAnimate = trans;
		// startPos = objectToAnimate.position;
	}


}
