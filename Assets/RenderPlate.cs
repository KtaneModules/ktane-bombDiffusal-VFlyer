using KModkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderPlate : MonoBehaviour {

	public GameObject PS2, Serial, Parallel, ACPower, HDMI, VGA, USB, RJ45, DVID, StereoRCA, ComponentVideo, CompositeVideo, PCMCIA;

	// Use this for initialization
	void Start () {
		//RenderPort(-1);
	}
	public void RenderPort(int idx)
    {
		GameObject[] allPorts = new GameObject[] { PS2, Serial, Parallel, ACPower, HDMI, VGA, USB, RJ45, DVID, StereoRCA, ComponentVideo, CompositeVideo, PCMCIA };
        for (int x = 0; x < allPorts.Length; x++)
        {
			if (x == idx)
			{
				allPorts[x].SetActive(true);
				//Debug.LogFormat("{0}", x);
			}
			else
				allPorts[x].SetActive(false);
        }

    }


	// Update is called once per frame
	void Update () {

	}
}
