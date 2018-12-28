using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RotateLogo : MonoBehaviour {

	[SerializeField] float speed = 10f;

	Material mat;
	Texture2D startTex;

	public Texture2D defaultFailTex;
	Color32[] failTexColors;

	Vector3 startScale;

	void Awake(){
		mat = GetComponent<Renderer>().material;
		startTex = (Texture2D)mat.mainTexture;
		StartCoroutine(GetDefaultFailTex());
		startScale = transform.localScale;
	}

	IEnumerator GetDefaultFailTex(){
		defaultFailTex = new Texture2D(2,2);
		WWW www = new WWW("file://noimage");
		yield return www;
		www.LoadImageIntoTexture(defaultFailTex);
		failTexColors = defaultFailTex.GetPixels32();
	}

	// Update is called once per frame
	void Update () {
		transform.Rotate(0, speed * Time.deltaTime, 0);

		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
		}
	}

	public void SetLogoImage(Texture2D tex){
		Debug.Log(tex);
		if(tex == null || tex == defaultFailTex){
			transform.localScale = startScale;
			mat.mainTexture = startTex;
			return;
		}

		if(tex.width == defaultFailTex.width && tex.height == defaultFailTex.height){
			//Check the pixel array to make sure it's not the red question mark
			Color32[] newTexPixels = tex.GetPixels32();

			int correctPixels = 0;

			for(int i = 0; i < newTexPixels.Length; i++){
				if(newTexPixels[i].r == failTexColors[i].r && newTexPixels[i].g == failTexColors[i].g && newTexPixels[i].b == failTexColors[i].b){
					correctPixels++;
				}
			}

			Debug.Log((float)correctPixels / (float)newTexPixels.Length);

			if((float)correctPixels / (float)newTexPixels.Length > 0.95f){
				transform.localScale = startScale;
				mat.mainTexture = startTex;
				return;
			}
		}

		if(tex.height < tex.width)
			transform.localScale = new Vector3(startScale.x, startScale.y * (float)tex.height / (float)tex.width, startScale.z);
		else
			transform.localScale = new Vector3(startScale.x * (float)tex.width / (float)tex.height, startScale.y, startScale.z);

		mat.mainTexture = tex;
	}
}
