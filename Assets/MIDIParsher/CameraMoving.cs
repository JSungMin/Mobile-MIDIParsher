using UnityEngine;
using System.Collections;

public class CameraMoving : MonoBehaviour {
	public MIDIPlayer MidiPlayer;
	public float y = 175;

	public float verticalSpeed = 300;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = new Vector3 (MidiPlayer.SumTPQ*0.1f, y, -10);
		y += Input.GetAxis ("Vertical")*verticalSpeed*Time.deltaTime;
		if(Input.GetAxis("Horizontal")!=0){
			MidiPlayer.SumTPQ += Input.GetAxis ("Horizontal") * 50;
		}
	}
}
