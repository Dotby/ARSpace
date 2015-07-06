using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpaceController : MonoBehaviour {

	public Text textField;
	string endText = "";
	bool gpsEnable = false;
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
