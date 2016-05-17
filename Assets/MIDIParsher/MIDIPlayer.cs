using UnityEngine;
using System.Collections;


public class MIDIPlayer : MonoBehaviour {
	public enum playState {Play,Stop};
	public float SumTPQ = 0;
	public float deltaTPQ;
	public playState nowState = playState.Play;

	public Note nowNote;
	public bool makeFlag = true;
	// Use this for initialization
	void Start () {
		
	}
		
	public void clickStop(){
		nowState = playState.Stop;
	}
	public void clickPlay(){
		nowState = playState.Play;
	}
	public RaycastHit hit;
	// Update is called once per frame
	void Update () {
		if(nowState==playState.Play)
		SumTPQ += deltaTPQ;
		Camera.main.fieldOfView += Input.mouseScrollDelta.y*10;
		if(Input.GetMouseButtonDown(0)){
			var clickPosition = Input.mousePosition;
			clickPosition = new Vector3 (Mathf.Round(clickPosition.x),Mathf.Round(clickPosition.y),0);
			if(makeFlag){
				var newNote = Instantiate (GetComponent<MIDIReader>().NotePrefab,Vector3.zero,Quaternion.identity) as GameObject;
				nowNote = newNote.GetComponent<Note>();
				newNote.name = "On";
				Debug.Log (clickPosition.ToString());
				int t = (int)(clickPosition.y/128);
				int oc = (int)(clickPosition.y-128*t)%12;
				newNote.GetComponent<Note> ().offset = (int)clickPosition.x;
				newNote.GetComponent<Note>().key = (byte)((clickPosition.y-128*t) - oc*12);

				newNote.transform.parent = GameObject.Find ("MIDIParsher").GetComponent<MIDIInfo>().tracks[t-1].transform;
				newNote.transform.localPosition = clickPosition;
				makeFlag = false;
			}
			nowNote.transform.localScale = new Vector3(Mathf.Max(0,clickPosition.x-nowNote.GetComponent<Note>().offset),1,1);
		}
	}
}
