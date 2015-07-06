using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpaceController : MonoBehaviour {

	const float EARTH_RADIUS = 6372795.0f;

	public Text textField;
	string endText = "";
	bool gpsEnable = false;
	public GoogleMap MAP = null;

	public float[] inp = new float[4];

	//LocationInfo currentGPSPosition;

	IEnumerator Start()
	{
		// First, check if user has location service enabled
		if (!Input.location.isEnabledByUser){
			AddField("GPS failed");
			yield break;
		}
		
		// Start service before querying location
		Input.location.Start();
		
		// Wait until service initializes
		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
			yield return new WaitForSeconds(1);
			maxWait--;
		}
		
		// Service didn't initialize in 20 seconds
		if (maxWait < 1)
		{
			AddField("Timed out");
			yield break;
		}
		
		// Connection has failed
		if (Input.location.status == LocationServiceStatus.Failed)
		{
			AddField("Unable to determine device location");
			yield break;
		}
		else
		{
			// Access granted and location value could be retrieved
			AddField("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
			gpsEnable = true;
		}
		
		// Stop service if there is no need to query location updates continuously
		//Input.location.Stop();
	}

	public void CalcMeters(){
		AddField(calculateTheDistance(inp[0], inp[1], inp[2], inp[3]) + " meters");
	}

	public void FindMe(){
		//if (gpsEnable == true){
			float lt = Input.location.lastData.latitude;
			float ln = Input.location.lastData.longitude;
			MAP.AddPoint(lt, ln);
		//}
	}

	public void ZoomMap(int num){
		MAP.zoom += num;
		MAP.Refresh();
	}

		/*
	 * Расстояние между двумя точками
	 * $φA, $λA - широта, долгота 1-й точки,
	 * $φB, $λB - широта, долгота 2-й точки
	 * Написано по мотивам http://gis-lab.info/qa/great-circles.html
	 * Михаил Кобзарев <kobzarev@inforos.ru>
	 *
	 */
	float calculateTheDistance (float lt1, float lt2, float ln1, float ln2) {
		
		// перевести координаты в радианы
		float lat1 = lt1 * Mathf.PI / 180;
		float lat2 = lt2 * Mathf.PI / 180;
		float long1 = ln1 * Mathf.PI / 180;
		float long2 = ln2 * Mathf.PI / 180;
		
		// косинусы и синусы широт и разницы долгот
		float cl1 = Mathf.Cos(lat1);
		float cl2 = Mathf.Cos(lat2);
		float sl1 = Mathf.Sin(lat1);
		float sl2 = Mathf.Sin(lat2);
		float delta = long2 - long1;
		float cdelta = Mathf.Cos(delta);
		float sdelta = Mathf.Sin(delta);
		
		// вычисления длины большого круга
		float y = Mathf.Sqrt(Mathf.Pow(cl2 * sdelta, 2) + Mathf.Pow(cl1 * sl2 - sl1 * cl2 * cdelta, 2));
		float x = sl1 * sl2 + cl1 * cl2 * cdelta;
		
		//
		float ad = Mathf.Atan2(y, x);
		float dist = ad * EARTH_RADIUS;
		
		return dist;
	}

//	void RetrieveGPSData()
//	{
//		currentGPSPosition = Input.location.lastData;
//		string gpsString = "::" + currentGPSPosition.latitude + "//" + currentGPSPosition.longitude;
//		AddField(gpsString);
//	}

	void Update () {
		if (gpsEnable == true){
			AddField("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
		}

		PrintText();
	}

	void AddField(string txt){
		endText = endText + "\n" + txt;
	}

	void PrintText(){
		textField.text = endText;
		endText = "";
	}
}
