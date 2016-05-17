using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour {
	public byte key;
	public int offset;
	public byte velocity;
	public int duration;
	public bool flag = true;
	private int octave;
	public string keyName;
	public string instrument;
	public string fullInstrumentName;
	private string[] keyTable = {"C","C#","D","D#","E","F","F#","G","G#","A","A#","B"}; 
	public AudioClip clip;

	private float nowVol;

	// Use this for initialization
	void Start () {
		octave = (int)(key-24)/12;
		keyName = instrument +"_"+ keyTable [key % 12] + octave.ToString ();
		fullInstrumentName = instrument + "_src/" + keyName;
		clip = Resources.Load (fullInstrumentName) as AudioClip;
		GetComponent<AudioSource> ().clip = clip;
		GetComponent<AudioSource> ().volume = (float)velocity / 127f;
		nowVol = (float)velocity / 127f;
		transform.localPosition += new Vector3 (0,transform.localPosition.y*128,0);
		transform.localPosition += Vector3.up * (octave*12 + key%12);
	}
	
	// Update is called once per frame
	void Update () {
		if (GameObject.Find ("MIDIParsher").GetComponent<MIDIPlayer> ().SumTPQ >= offset && flag) {
			GetComponent<AudioSource> ().Play ();
			flag = false;
		}
		else if (!flag&&GetComponent<AudioSource>().time>duration) {
			GetComponent<AudioSource> ().volume -= velocity*Time.deltaTime*0.01f;
		}
		if(GameObject.Find ("MIDIParsher").GetComponent<MIDIPlayer> ().SumTPQ < offset&&name=="On"){
			flag = true;
		}
	}
}
