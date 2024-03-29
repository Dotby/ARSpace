using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoogleMap : MonoBehaviour
{
	public enum MapType
	{
		RoadMap,
		Satellite,
		Terrain,
		Hybrid
	}
	public bool loadOnStart = true;
	public bool autoLocateCenter = true;
	public GoogleMapLocation centerLocation;
	public int zoom = 13;
	public MapType mapType;
	public int size = 512;
	public bool doubleResolution = false;
	public List<GoogleMapMarker> markers = new List<GoogleMapMarker>();
	public GoogleMapPath[] paths;
	public GameObject mapQuad;
	
	void Start() {
		if(loadOnStart) Refresh();	
	}
	
	public void Refresh() {
		Debug.Log("Refresh...");

		if(autoLocateCenter && (markers.Count == 0 && paths.Length == 0)) {
			Debug.LogError("Auto Center will only work if paths or markers are used.");	
		}
		StartCoroutine(_Refresh());
	}
	
	IEnumerator _Refresh ()
	{
		string url = "http://maps.googleapis.com/maps/api/staticmap";
		string qs = "";
		if (!autoLocateCenter) {
			if (centerLocation.address != "")
				qs += "center=" +  (centerLocation.address);
			else {
				qs += "center=" + WWW.EscapeURL (string.Format ("{0},{1}", centerLocation.latitude, centerLocation.longitude));
			}
		
			qs += "&zoom=" + zoom.ToString ();
		}
		qs += "&size=" + WWW.EscapeURL (string.Format ("{0}x{0}", size));
		qs += "&scale=" + (doubleResolution ? "2" : "1");
		qs += "&maptype=" + mapType.ToString ().ToLower ();
		bool usingSensor = false;
#if UNITY_IPHONE
		usingSensor = Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running;
#endif
		qs += "&sensor=" + (usingSensor ? "true" : "false");
		
		foreach (GoogleMapMarker i in markers) {
			qs += "&markers=" + string.Format ("size:{0}|color:{1}|label:{2}", i.size.ToString ().ToLower (), i.color, i.label);
			foreach (var loc in i.locations) {
				if (loc.address != ""){
					Debug.Log("NON esc: " + loc.address + " esc: " + WWW.EscapeURL (loc.address));
					qs += "|" + WWW.EscapeURL (loc.address);
				}
				else{
					Debug.Log("NON esc: " + string.Format ("{0},{1}", loc.latitude, loc.longitude) + " esc: " + WWW.EscapeURL (string.Format ("{0},{1}", loc.latitude, loc.longitude)));
					qs += "|" + WWW.EscapeURL (string.Format ("{0},{1}", loc.latitude, loc.longitude));
				}
			}
		}
		
		foreach (var i in paths) {
			qs += "&path=" + string.Format ("weight:{0}|color:{1}", i.weight, i.color);
			if(i.fill) qs += "|fillcolor:" + i.fillColor;
			foreach (GoogleMapLocation loc in i.locations) {
				if (loc.address != "")
					qs += "|" + WWW.EscapeURL (loc.address);
				else
					qs += "|" + WWW.EscapeURL (string.Format ("{0},{1}", loc.latitude, loc.longitude));
			}
		}
		
		
		WWW req = new WWW(url + "?" + qs);
		Debug.Log("URL: " +url + "?" + qs);
		yield return req;
		while (!req.isDone)
			yield return null;
		if (req.error == null) {
			var tex = new Texture2D (size, size);
			tex.LoadImage (req.bytes);
			mapQuad.GetComponent<Renderer>().material.mainTexture = tex;
		}
		else{
			Debug.Log("Error: " + req.error);
		}
	}

	public void AddPoint(float lt, float ln){
		Debug.Log("AddPoint: " + lt + " : " + ln);
		GoogleMapMarker mk = new GoogleMapMarker();
		mk.color = GoogleMapColor.red;
		mk.label = "Moscow";

		GoogleMapLocation loc = new GoogleMapLocation();
		loc.address = "";
		loc.latitude = lt;
		loc.longitude = ln;

		mk.locations = new GoogleMapLocation[1];
		mk.locations[0] = loc;

		markers.Add(mk);

		Refresh();
	}

}

public enum GoogleMapColor
{
	black,
	brown,
	green,
	purple,
	yellow,
	blue,
	gray,
	orange,
	red,
	white
}

[System.Serializable]
public class GoogleMapLocation
{
	public string address;
	public float latitude;
	public float longitude;
}

[System.Serializable]
public class GoogleMapMarker
{
	public enum GoogleMapMarkerSize
	{
		Tiny,
		Small,
		Mid
	}
	public GoogleMapMarkerSize size;
	public GoogleMapColor color;
	public string label;
	public GoogleMapLocation[] locations;
	
}

[System.Serializable]
public class GoogleMapPath
{
	public int weight = 5;
	public GoogleMapColor color;
	public bool fill = false;
	public GoogleMapColor fillColor;
	public GoogleMapLocation[] locations;	
}