using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataToObject : MonoBehaviour {

	private static DataToObject instance;
	public static DataToObject Instance
	{
		get
		{
			if (instance != null)
				return instance;

			instance = FindObjectOfType<DataToObject>();
			return instance;
		}
	}

	[SerializeField] float xSpacing = 1f;
	[SerializeField] float xSpacingSmallSubtract = 1f;
	[SerializeField] float xSpacingMAdd = 0.2f;
	[SerializeField] float ySpacing = 2.5f;

	[SerializeField] Material letterMat;
	[SerializeField] Color mainColor = Color.white;
	[SerializeField] Color negativeColor = Color.red;
	[SerializeField] Color positiveColor = Color.green;

	[SerializeField] char[] alphabet;

	[SerializeField] GameObject[] threeDAlphabet;

	[SerializeField] RotateLogo logoScript;

	[SerializeField] char[] smallKerningCharacters;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MakeStockQuote(DataRequest.StockInformation info){
		logoScript.SetLogoImage(info.logo);

		GameObject holder = new GameObject(info.symbol + " Holder");
		// holder.transform.position = new Vector3(5.0f, 0, 0);

		MakeThreeDText(info.companyName, ySpacing, .6f, mainColor, true).transform.parent = holder.transform;
		MakeThreeDText(info.symbol, 0, 0.4f, mainColor).transform.parent = holder.transform;
		MakeThreeDText(info.latestPrice.ToString("F2") + " " + info.currency, -ySpacing, 0.4f, mainColor).transform.parent = holder.transform;
		
		float changePercent = info.changePercent * 100.0f;
		Color changeCol = positiveColor;
		if(Mathf.Sign(changePercent) < 0){
			changeCol = negativeColor;
		}

		MakeThreeDText(info.change.ToString("F2") + " (" + changePercent.ToString() + "%)", -ySpacing * 2, 0.4f, changeCol).transform.parent = holder.transform;
		
		AnimationManager.Instance.StartAnimation(holder.transform);
	}

	GameObject MakeThreeDText(string word, float yPos, float parentScale, Color col, bool largest = false){
		GameObject parent = new GameObject(word);
		List<GameObject> containedCharacters3D = new List<GameObject>();
		List<int> indexerReferences = new List<int>();
		word = word.ToLower();
		for(int i = 0; i < word.Length; i++){
			for(int j = 0; j < alphabet.Length; j++){
				if(alphabet[j] == word[i]){
					containedCharacters3D.Add(threeDAlphabet[j]);
					indexerReferences.Add(j);
				}
			}
		}

		float spacing = 0f;
		for(int i = 0; i < containedCharacters3D.Count; i++){
			GameObject newObj = Instantiate(containedCharacters3D[i]) as GameObject;
			Renderer rend = newObj.GetComponent<Renderer>();

			bool smallCharacter = false;
			char currentCharacter = alphabet[indexerReferences[i]];
			for(int j = 0; j < smallKerningCharacters.Length; j++){
				if(smallKerningCharacters[j] == currentCharacter){
					smallCharacter = true;
					break;
				}
			}

			if(smallCharacter)
				spacing -= xSpacingSmallSubtract;
			else if(currentCharacter == 'm')
				spacing += xSpacingMAdd;

			newObj.transform.position = new Vector3(spacing, 0, 0);
			spacing += xSpacing;

			if(smallCharacter)
				spacing -= xSpacingSmallSubtract;
			else if(currentCharacter == 'm')
				spacing += xSpacingMAdd;

			if(rend != null){
				rend.material = letterMat;
				rend.material.color = col;
				newObj.transform.position -= newObj.transform.InverseTransformPoint(new Vector3(rend.bounds.center.x, 0, 0));
			}
			newObj.transform.localScale = new Vector3(newObj.transform.localScale.x, newObj.transform.localScale.y, newObj.transform.localScale.z * 2.0f);
			newObj.transform.parent = parent.transform;
		}
		if(largest)
			AnimationManager.Instance.endXPos = Mathf.Abs(spacing * parentScale) + 10f;

		parent.transform.localScale *= parentScale;

		parent.transform.position = new Vector3(0, yPos, 0);

		return parent;
	}
}
