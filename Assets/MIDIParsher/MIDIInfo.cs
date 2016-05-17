using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MIDIInfo : MonoBehaviour {

	public string songName;
	public int trackCount;
	public int TPQ;
	public List<Track> tracks;
	public float Tempo = 120;
	public List<float> TempoList;
	public List<int> metaDeltaT;
	public int startOffset;
	public int count = 0;

	// Use this for initialization
	void Start () {
	}
	public int tempCount = 0;
	// Update is called once per frame
	void Update () {
		if (metaDeltaT.Count > 0) {
			if (GetComponent<MIDIPlayer> ().SumTPQ > metaDeltaT [count]) {
				Tempo = TempoList [count];
				GetComponent<MIDIPlayer> ().deltaTPQ = Mathf.Round (TPQ * (Tempo) / 60 * Time.deltaTime); 
				if (count + 1 < metaDeltaT.Count) {
					count++;
				}
			} else if(GetComponent<MIDIPlayer> ().SumTPQ < metaDeltaT [count]&&Input.GetAxis("Horizontal")<0){
				if (count - 1 > 0) {
					count--;
				}
			}
		}
	}
}
