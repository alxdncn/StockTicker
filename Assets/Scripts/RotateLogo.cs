using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RotateLogo : MonoBehaviour {

	[SerializeField] float speed = 10f;

	Material mat;
	Texture2D startTex;

	public Texture2D defaultFailTex;

	void Awake(){
		mat = GetComponent<Renderer>().material;
		startTex = (Texture2D)mat.mainTexture;
		StartCoroutine(GetDefaultFailTex());
	}

	IEnumerator GetDefaultFailTex(){
		defaultFailTex = new Texture2D(2,2);
		WWW www = new WWW("file://noimage");
		yield return www;
		www.LoadImageIntoTexture(defaultFailTex);
	}

	// Update is called once per frame
	void Update () {
		transform.Rotate(0, speed * Time.deltaTime, 0);
	}

	public void SetLogoImage(Texture tex){
		Debug.Log(tex);
		if(tex == null || tex == defaultFailTex){
			mat.mainTexture = startTex;
			return;
		}
		mat.mainTexture = tex;
	}
}
