using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Track : MonoBehaviour {

	public List<Note> notes;
	public int offset;
	public string Instrument = "piano";

	public int trackLen;
	public int trackEnd;

	public void setTrackLen(int value){
		trackLen = (int)value;
	}
	public void setTrackEnd(int nowOffset){
		trackEnd = trackLen + (int)nowOffset;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
