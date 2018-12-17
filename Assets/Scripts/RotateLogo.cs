using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLogo : MonoBehaviour {

	[SerializeField] float speed = 10f;

	Material mat;
	Texture startTex;

	void Awake(){
		mat = GetComponent<Renderer>().material;
		startTex = mat.mainTexture;
	}

	// Update is called once per frame
	void Update () {
		transform.Rotate(0, speed * Time.deltaTime, 0);
	}

	public void SetLogoImage(Texture tex){
		Debug.Log(tex);
		if(tex == null){
			mat.mainTexture = startTex;
			return;
		}
		mat.mainTexture = tex;
	}
}
