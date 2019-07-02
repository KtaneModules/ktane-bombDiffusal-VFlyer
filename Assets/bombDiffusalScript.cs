using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class bombDiffusalScript : MonoBehaviour 
{
	public KMBombInfo bomb;
	public KMAudio Audio;

    static System.Random rnd = new System.Random();

	public TextMesh licenseNoText;
	public GameObject[] plates;

	//Logging
	static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

	int port1, port2;
	String licenseNo;
	List<int> deliveryNo;
	int destination;

	void Awake()
	{
		moduleId = moduleIdCounter++;
		//btn.OnInteract += delegate () { PressButton(); return false; };
	}

	void Start () 
	{
		SetUpPorts();
		GenerateNumbers();
		CalcDestination();
	}

	void SetUpPorts()
	{
		port1 = rnd.Next() % 13;
		port2 = rnd.Next() % 13;

        Debug.LogFormat("[Bomb Diffusal #{0}] Port 1 is {1}. Port 2 is {2}.", moduleId, GetPortName(port1), GetPortName(port2));
		
		plates[0].transform.Find(GetPortName(port1)).gameObject.SetActive(true);
		plates[1].transform.Find(GetPortName(port2)).gameObject.SetActive(true);
	}
	
	void GenerateNumbers()
	{
		do {
			GenerateLicenseNo();
			GenerateDeliveryNo();
		} while (CheckValidDeliveryNo());

		licenseNoText.text = "Software License No.:\n" + licenseNo;

        Debug.LogFormat("[Bomb Diffusal #{0}] Software licesnse number is {1}.", moduleId, licenseNo);
        Debug.LogFormat("[Bomb Diffusal #{0}] Delivery number is {1}.", moduleId, GetValues(deliveryNo.ToArray()));
	}

	void CalcDestination()
	{
		for(int i = 9; i >= 0; i--)
		{
			if(deliveryNo.FindAll(x => x == i).Count() == 1)
			{
				destination = (deliveryNo.IndexOf(i) + 1) * 100;
        		Debug.LogFormat("[Bomb Diffusal #{0}] Delivery sector is {1}.", moduleId, GetSectorName(deliveryNo.IndexOf(i) + 1));
				break;
			}
		}

		if(destination == 600)
		{
			destination += (deliveryNo.ElementAt(5) % 5) * 10;
			destination += deliveryNo.ElementAt(4) % 5;
        	Debug.LogFormat("[Bomb Diffusal #{0}] (Delivery is in space. Reversing number for next step.)", moduleId, GetValues(deliveryNo.ToArray()));
		}
		else
		{
			destination += (deliveryNo.ElementAt(0) % 5) * 10;
			destination += deliveryNo.ElementAt(1) % 5;
		}

        Debug.LogFormat("[Bomb Diffusal #{0}] Delivery area is {1}.", moduleId, GetAreaName(destination));
	}

	void GenerateLicenseNo()
	{
		licenseNo = "";

		int nLetters = rnd.Next() % 3 + 2;
		int nNumbers = 6 - nLetters;

		for(int i = 0; i < nLetters; i++)
			licenseNo += (char) (rnd.Next() % 26 + 65);
		for(int i = 0; i < nNumbers; i++)
			licenseNo += (char) (rnd.Next() % 10 + 48);

		licenseNo = GetValues(licenseNo.ToArray().OrderBy(x => rnd.Next()).ToArray());
	}

	void GenerateDeliveryNo()
	{
		deliveryNo = new List<int>();
		String sn = bomb.GetSerialNumber();

		for(int i = 0; i < sn.Length; i++)
		{
			if((int) sn[i] >= 65 && (int) sn[i] <= 69)
			{
				if((int) licenseNo[i] >= 65 && (int) licenseNo[i] <= 69) deliveryNo.Add(3);
				else if((int) licenseNo[i] >= 70 && (int) licenseNo[i] <= 74) deliveryNo.Add(9);
				else if((int) licenseNo[i] >= 75 && (int) licenseNo[i] <= 79) deliveryNo.Add(6);
				else if((int) licenseNo[i] >= 80 && (int) licenseNo[i] <= 84) deliveryNo.Add(3);
				else if((int) licenseNo[i] >= 85 && (int) licenseNo[i] <= 89) deliveryNo.Add(3);
				else if(((int) licenseNo[i] >= 48 && (int) licenseNo[i] <= 52) || (int) licenseNo[i] == 90) deliveryNo.Add(1);
				else deliveryNo.Add(4);
			}
			else if((int) sn[i] >= 70 && (int) sn[i] <= 74)
			{
				if((int) licenseNo[i] >= 65 && (int) licenseNo[i] <= 69) deliveryNo.Add(5);
				else if((int) licenseNo[i] >= 70 && (int) licenseNo[i] <= 74) deliveryNo.Add(6);
				else if((int) licenseNo[i] >= 75 && (int) licenseNo[i] <= 79) deliveryNo.Add(8);
				else if((int) licenseNo[i] >= 80 && (int) licenseNo[i] <= 84) deliveryNo.Add(5);
				else if((int) licenseNo[i] >= 85 && (int) licenseNo[i] <= 89) deliveryNo.Add(9);
				else if(((int) licenseNo[i] >= 48 && (int) licenseNo[i] <= 52) || (int) licenseNo[i] == 90) deliveryNo.Add(5);
				else deliveryNo.Add(0);
			}
			else if((int) sn[i] >= 75 && (int) sn[i] <= 79)
			{
				if((int) licenseNo[i] >= 65 && (int) licenseNo[i] <= 69) deliveryNo.Add(6);
				else if((int) licenseNo[i] >= 70 && (int) licenseNo[i] <= 74) deliveryNo.Add(2);
				else if((int) licenseNo[i] >= 75 && (int) licenseNo[i] <= 79) deliveryNo.Add(7);
				else if((int) licenseNo[i] >= 80 && (int) licenseNo[i] <= 84) deliveryNo.Add(3);
				else if((int) licenseNo[i] >= 85 && (int) licenseNo[i] <= 89) deliveryNo.Add(8);
				else if(((int) licenseNo[i] >= 48 && (int) licenseNo[i] <= 52) || (int) licenseNo[i] == 90) deliveryNo.Add(6);
				else deliveryNo.Add(7);
			}
			else if((int) sn[i] >= 80 && (int) sn[i] <= 84)
			{
				if((int) licenseNo[i] >= 65 && (int) licenseNo[i] <= 69) deliveryNo.Add(1);
				else if((int) licenseNo[i] >= 70 && (int) licenseNo[i] <= 74) deliveryNo.Add(4);
				else if((int) licenseNo[i] >= 75 && (int) licenseNo[i] <= 79) deliveryNo.Add(6);
				else if((int) licenseNo[i] >= 80 && (int) licenseNo[i] <= 84) deliveryNo.Add(9);
				else if((int) licenseNo[i] >= 85 && (int) licenseNo[i] <= 89) deliveryNo.Add(8);
				else if(((int) licenseNo[i] >= 48 && (int) licenseNo[i] <= 52) || (int) licenseNo[i] == 90) deliveryNo.Add(9);
				else deliveryNo.Add(1);
			}
			else if((int) sn[i] >= 85 && (int) sn[i] <= 89)
			{
				if((int) licenseNo[i] >= 65 && (int) licenseNo[i] <= 69) deliveryNo.Add(2);
				else if((int) licenseNo[i] >= 70 && (int) licenseNo[i] <= 74) deliveryNo.Add(3);
				else if((int) licenseNo[i] >= 75 && (int) licenseNo[i] <= 79) deliveryNo.Add(9);
				else if((int) licenseNo[i] >= 80 && (int) licenseNo[i] <= 84) deliveryNo.Add(7);
				else if((int) licenseNo[i] >= 85 && (int) licenseNo[i] <= 89) deliveryNo.Add(2);
				else if(((int) licenseNo[i] >= 48 && (int) licenseNo[i] <= 52) || (int) licenseNo[i] == 90) deliveryNo.Add(7);
				else deliveryNo.Add(2);
			}
			else if(((int) sn[i] >= 48 && (int) sn[i] <= 52) || (int) sn[i] == 90)
			{
				if((int) licenseNo[i] >= 65 && (int) licenseNo[i] <= 69) deliveryNo.Add(5);
				else if((int) licenseNo[i] >= 70 && (int) licenseNo[i] <= 74) deliveryNo.Add(1);
				else if((int) licenseNo[i] >= 75 && (int) licenseNo[i] <= 79) deliveryNo.Add(8);
				else if((int) licenseNo[i] >= 80 && (int) licenseNo[i] <= 84) deliveryNo.Add(2);
				else if((int) licenseNo[i] >= 85 && (int) licenseNo[i] <= 89) deliveryNo.Add(8);
				else if(((int) licenseNo[i] >= 48 && (int) licenseNo[i] <= 52) || (int) licenseNo[i] == 90) deliveryNo.Add(4);
				else deliveryNo.Add(4);
			}
			else
			{
				if((int) licenseNo[i] >= 65 && (int) licenseNo[i] <= 69) deliveryNo.Add(0);
				else if((int) licenseNo[i] >= 70 && (int) licenseNo[i] <= 74) deliveryNo.Add(1);
				else if((int) licenseNo[i] >= 75 && (int) licenseNo[i] <= 79) deliveryNo.Add(0);
				else if((int) licenseNo[i] >= 80 && (int) licenseNo[i] <= 84) deliveryNo.Add(7);
				else if((int) licenseNo[i] >= 85 && (int) licenseNo[i] <= 89) deliveryNo.Add(5);
				else if(((int) licenseNo[i] >= 48 && (int) licenseNo[i] <= 52) || (int) licenseNo[i] == 90) deliveryNo.Add(4);
				else deliveryNo.Add(0);
			}
		}
	}
	
	bool CheckValidDeliveryNo()
	{
		foreach(int n in deliveryNo)
			if(deliveryNo.FindAll(x => x == n).Count() == 1) return false;

		return true;
	}

	String GetPortName(int port)
	{
		switch(port)
		{
			case 0: return "PS2";
			case 1: return "Serial";
			case 2: return "Parallel";
			case 3: return "AC Power";
			case 4: return "HDMI";
			case 5: return "VGA";
			case 6: return "USB";
			case 7: return "RJ-45";
			case 8: return "DVI-D";
			case 9: return "Stereo RCA";
			case 10: return "Component Video";
			case 11: return "Composite Video";
			case 12: return "PCMCIA";
		}

		return "";
	}

	String GetValues(char[] array)
	{
		string ret = "";
		foreach(char val in array)
			ret += val;
		return ret;
	}

	String GetValues(int[] array)
	{
		string ret = "";
		foreach(int val in array)
			ret += val;
		return ret;
	}
	
	String GetSectorName(int sector)
	{
		switch(sector)
		{
			case 1: return "USA #1";
			case 2: return "USA #2";
			case 3: return "Rest of America";
			case 4: return "Eurasia";
			case 5: return "Africa";
			case 6: return "Space";
		}

		return "";
	}

	String GetAreaName(int area)
	{
		switch(area)
		{
			case 100: return "Alabama";
			case 101: return "Alaska";
			case 102: return "Arizona";
			case 103: return "Arkansas";
			case 104: return "California";
			case 110: return "Colorado";
			case 111: return "Connecticut";
			case 112: return "Delaware";
			case 113: return "Florida";
			case 114: return "Georgia";
			case 120: return "Hawaii";
			case 121: return "Idaho";
			case 122: return "Illinois";
			case 123: return "Indiana";
			case 124: return "Iowa";
			case 130: return "Kansas";
			case 131: return "Kentucky";
			case 132: return "Louisiana";
			case 133: return "Maine";
			case 134: return "Maryland";
			case 140: return "Massachusetts";
			case 141: return "Michigan";
			case 142: return "Minnesota";
			case 143: return "Mississipi";
			case 144: return "Missouri";

			case 200: return "Montana";
			case 201: return "Nebraska";
			case 202: return "Nevada";
			case 203: return "New Hampshire";
			case 204: return "New Jersey";
			case 210: return "New Mexico";
			case 211: return "New York";
			case 212: return "North Carolina";
			case 213: return "North Dakota";
			case 214: return "Ohio";
			case 220: return "Oklahoma";
			case 221: return "Oregon";
			case 222: return "Pennsylvania";
			case 223: return "Rhode Island";
			case 224: return "South Carolina";
			case 230: return "South Dakota";
			case 231: return "Tennessee";
			case 232: return "Texas";
			case 233: return "Utah";
			case 234: return "Vermont";
			case 240: return "Virginia";
			case 241: return "Washington";
			case 242: return "West Virginia";
			case 243: return "Wisconsin";
			case 244: return "Wyoming";

			case 300: return "Canada";
			case 301: return "Mexico";
			case 302: return "Argentina";
			case 303: return "Brazil";
			case 304: return "Bolivia";
			case 310: return "Chile";
			case 311: return "Colombia";
			case 312: return "Ecuador";
			case 313: return "Guyana";
			case 314: return "Paraguay";
			case 320: return "Peru";
			case 321: return "Suriname";
			case 322: return "Uruguay";
			case 323: return "Venezuela";
			case 324: return "Belize";
			case 330: return "Costa Rica";
			case 331: return "El Salvador";
			case 332: return "Guatemala";
			case 333: return "Honduras";
			case 334: return "Nicaragua";
			case 340: return "Panama";
			case 341: return "Dominican Republic";
			case 342: return "Bahamas";
			case 343: return "Barbados";
			case 344: return "Haiti";

			case 400: return "China";
			case 401: return "Germany";
			case 402: return "India";
			case 403: return "France";
			case 404: return "Indonesia";
			case 410: return "Croatia";
			case 411: return "Pakistan";
			case 412: return "Spain";
			case 413: return "Bangladesh";
			case 414: return "Italy";
			case 420: return "Japan";
			case 421: return "UK";
			case 422: return "Philippines";
			case 423: return "Switzerland";
			case 424: return "Vietnam";
			case 430: return "Belgium";
			case 431: return "Iran";
			case 432: return "Greece";
			case 433: return "South Korea";
			case 434: return "Netherlands";
			case 440: return "Laos";
			case 441: return "Poland";
			case 442: return "Thailand";
			case 443: return "Sweden";
			case 444: return "Russia";

			case 500: return "South Africa";
			case 501: return "Nigeria";
			case 502: return "Morocco";
			case 503: return "Kenya";
			case 504: return "Senegal";
			case 510: return "Ghana";
			case 511: return "DRC";
			case 512: return "Ethiopia";
			case 513: return "Algeria";
			case 514: return "Tanzania";
			case 520: return "Tunisia";
			case 521: return "Cameroon";
			case 522: return "Uganda";
			case 523: return "Mali";
			case 524: return "Zimbabwe";
			case 530: return "Madagascar";
			case 531: return "Angola";
			case 532: return "Sudan";
			case 533: return "Namibia";
			case 534: return "Zambia";
			case 540: return "Somalia";
			case 541: return "Libya";
			case 542: return "Niger";
			case 543: return "Swaziland";
			case 544: return "Egypt";

			case 600: return "Mercury";
			case 601: return "Venus";
			case 602: return "Mars";
			case 603: return "Jupiter";
			case 604: return "Saturn";
			case 610: return "Uranus";
			case 611: return "Neptune";
			case 612: return "The Moon";
			case 613: return "Titan";
			case 614: return "Io";
			case 620: return "Europa";
			case 621: return "Triton";
			case 622: return "Callisto";
			case 623: return "Ganymede";
			case 624: return "Rhea";
			case 630: return "Umbriel";
			case 631: return "Oberon";
			case 632: return "Phoebe";
			case 633: return "The Sun";
			case 634: return "Asteroid Belt";
			case 640: return "Pluto";
			case 641: return "Kepler-1638b";
			case 642: return "The ISS";
			case 643: return "Kepler-1229b";
			case 644: return "Kepler-452b";
		}

		return "";
	}
}
