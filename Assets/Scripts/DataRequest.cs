using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class DataRequest : MonoBehaviour {
	
	private static DataRequest instance;
	public static DataRequest Instance
	{
		get
		{
			if (instance != null)
				return instance;

			instance = FindObjectOfType<DataRequest>();
			return instance;
		}
	}

	//Alpha Vantage stuff - use for TSX
	string aVApiKey = "apikey=OLGP5CSXWZ4A4GFA";
	string aVFunctionKeyword = "function=GLOBAL_QUOTE";
	string aVNameFunction = "function=SYMBOL_SEARCH";
	string aVApiUrl = "https://www.alphavantage.co/query?";

	//IEX stuff - use for US stocks
	string iexApiUrl = "https://api.iextrading.com/1.0/stock/";
	string iexRequestQuote = "/quote";
	string iexRequestLogo = "/logo";

	public int stockIndex = 0;

	public void ShowNextStock(){
		StartCoroutine(RequestStockData(StockListHolder.symbolsUS[stockIndex]));
		stockIndex = (stockIndex + 1) % StockListHolder.symbolsUS.Length;
	}

	IEnumerator RequestStockData(string stockSymbol, bool isTSX = false){
		UnityWebRequest www = new UnityWebRequest(iexApiUrl + stockSymbol + iexRequestQuote);
		Texture2D logoToSet = null;
		string tsxName = "";
		string tsxCurrency = "USD";

		if(isTSX){
			//Get the name of the stock
			UnityWebRequest nameWWW = new UnityWebRequest(aVApiUrl + aVNameFunction + "&keywords=" + stockSymbol + "&" + aVApiKey);
			Debug.Log("Name Search URL: " + nameWWW.url);
			nameWWW.downloadHandler = new DownloadHandlerBuffer();
			yield return nameWWW.SendWebRequest();
			if(nameWWW.isNetworkError || nameWWW.isHttpError){
				Debug.Log(nameWWW.error);
				ShowNextStock();
				yield break;
			} else{
				Debug.Log(nameWWW.downloadHandler.text);
				JSONNode matches = JSON.Parse(nameWWW.downloadHandler.text);

				if(matches["Note"] != null || matches["Error Message"]){  //This means we have asked the api too many times and need to go to the next stock
					ShowNextStock();
					yield break;
				}

				JSONArray matchesArray = (JSONArray)matches["bestMatches"];

				if(matchesArray == null || matchesArray.Count <= 0){
					ShowNextStock();
					yield break;
				}

				JSONNode mainMatch = matchesArray[0];
				tsxName = mainMatch["2. name"];
				tsxCurrency = mainMatch["8. currency"];
				stockSymbol = mainMatch["1. symbol"];
				Debug.Log(tsxName);
			}
			www.url = (aVApiUrl + aVFunctionKeyword + "&symbol=" + stockSymbol + "&" + aVApiKey);
		} else{
			UnityWebRequest logoWWW = new UnityWebRequest("https://storage.googleapis.com/iex/api/logos/" + stockSymbol + ".png");
			Debug.Log(logoWWW.url);
			logoWWW.downloadHandler = new DownloadHandlerTexture();
			yield return logoWWW.SendWebRequest();
			if(logoWWW.isNetworkError || logoWWW.isHttpError){
				Debug.Log(logoWWW.error);
			} else{
				logoToSet = DownloadHandlerTexture.GetContent(logoWWW);
			}
		}

		www.downloadHandler = new DownloadHandlerBuffer();
		Debug.Log("URL: " + www.url);
		yield return www.SendWebRequest();

		if(www.isNetworkError || www.isHttpError) {
			Debug.Log(www.error);
			if(isTSX){		
				ShowNextStock();
			} else{
				StartCoroutine(RequestStockData(stockSymbol, true));
			}
			yield break;
		}
		else {
			if(isTSX){
				Debug.Log(www.downloadHandler.text);
				JSONNode dataJSON = JSON.Parse(www.downloadHandler.text);

				if(dataJSON["Note"] || dataJSON["Error Message"] != null){
					ShowNextStock();
					yield break;
				}

				JSONNode global = dataJSON["Global Quote"];

				if(global == null || global["Error Message"] != null){ //We didn't get anything back and likely used all our api requests
					ShowNextStock();
					yield break;
				}

				Debug.Log(global);
				StockInformation stockInfo = new StockInformation(tsxName, global["01. symbol"], global["05. price"], global["02. open"], global["09. change"], global["10. change percent"], tsxCurrency, global["08. previous close"]);
				DataToObject.Instance.MakeStockQuote(stockInfo);
			} else{
				Debug.Log(www.downloadHandler.text);
				StockInformation stockInfo = JsonUtility.FromJson<StockInformation>(www.downloadHandler.text);
				stockInfo.logo = logoToSet;
				DataToObject.Instance.MakeStockQuote(stockInfo);
			}
		}
	}

	[System.Serializable]
	public class StockInformation{
		public string companyName;
		public string symbol;
		public float latestPrice;
		public float open;
		public float change;
		public float changePercent;
		public string currency = "USD";
		public float previousClose;
		public Texture2D logo;

		public StockInformation(){

		}

		public StockInformation(string cn, string s, float lp, float o, float c, float cp, string cu, float pc){
			companyName = cn;
			symbol = s;
			latestPrice = lp;
			open = o;
			change = c;
			changePercent = cp;
			currency = cu;
			previousClose = pc;

			logo = null;
		}
	}
}
