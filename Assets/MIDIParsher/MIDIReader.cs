using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class MIDIReader : MonoBehaviour {

	public string path = "Assets/Scorpion_Fire_(Full._piano)";

	public GameObject TrackPrefab;
	public GameObject NotePrefab;
	public GameObject ParsherPrefab;
	/* The list of Midi Events */
	public const int EventNoteOff = 0x80;
	public const int EventNoteOn = 0x90;
	public const int EventKeyPressure = 0xA0;
	public const int EventControlChange = 0xB0;
	public const int EventProgramChange = 0xC0;
	public const int EventChannelPressure = 0xD0;
	public const int EventPitchBend = 0xE0;
	public const int SysexEvent1 = 0xF0;
	public const int SysexEvent2 = 0xF7;
	public const int MetaEvent = 0xFF;

	/* The list of Meta Events */
	public const int MetaEventSequence = 0x0;
	public const int MetaEventText = 0x1;
	public const int MetaEventCopyright = 0x2;
	public const int MetaEventSequenceName = 0x3;
	public const int MetaEventInstrument = 0x4;
	public const int MetaEventLyric = 0x5;
	public const int MetaEventMarker = 0x6;
	public const int MetaEventEndOfTrack = 0x2F;
	public const int MetaEventTempo = 0x51;
	public const int MetaEventSMPTEOffset = 0x54;
	public const int MetaEventTimeSignature = 0x58;
	public const int MetaEventKeySignature = 0x59;

	public BinaryReader MidiFile;
	public FileInfo fileInfo;
	public byte[] MidiBytes;

	public int offset = 0;

	public string ReadChars(int size){
		string s;
		char[] read = new char[size];
		for(int i = 0;i<size;i++){
			read [i] = (char)MidiBytes [i + offset];	
		}
		offset += size;
		s = new string (read, 0, read.Length);
		return s;
	}
	public int ReadShort() {
		int x = ( ( (int)(MidiBytes[offset]) << 8) | (int)(MidiBytes[offset + 1])  );
		offset += 2;
		return x;
	}

	public int ReadInt() {
		int x = ( (int)((MidiBytes[offset]) << 24) | (int)((MidiBytes[offset+1]) << 16) |
			(int)((MidiBytes[offset+2]) << 8) | (int)(MidiBytes[offset+3]) );
		offset += 4;
		return x;
	}
	public byte ReadByte() {
		byte x = MidiBytes[offset];
		offset++;
		return x;
	}
	public byte[] ReadBytes(int amount) {
		byte[] chunk = new byte[amount];
		for (int i = 0; i < amount; i++) {
			chunk[i] = MidiBytes[i + offset];
		}

		offset += amount;
		return chunk;
	}

	int getHighNibble(int data){
		return (data >> 4) & 0xF;
	}
	int getLowNibble(int data){
		return data & 0xF;
	}

	public int ReadVarlen() {
		int result = 0;
		byte b;

		b = ReadByte();
		result = (int)(b & 0x7F);

		for (int i = 0; i < 3; i++) {
			if ((b & 0x80) != 0) {
				b = ReadByte();
				result = (int)((result << 7) + (b & 0x7F));
			}
			else {
				break;
			}
		}
		return (int)result;
	}
	public byte Pop(){
		return MidiBytes[offset];
	}


	public string header;
	public int format;
	public int currentTrackNumber = 0;


	public void FileRead(){
		header = ReadChars (4);
		if (header == "MThd") {
			Debug.Log ("MThd is founded");
		}

		long goodInt = ReadInt ();
		if (goodInt == 6)
			Debug.Log ("This's a good Midi file");
		format = ReadShort ();
		if(format>=0&&format<=2)
			Debug.Log ("Format : " + format);
		GetComponent<MIDIInfo>().trackCount = ReadShort ();
		Debug.Log ("Track : " + GetComponent<MIDIInfo>().trackCount);
		GetComponent<MIDIInfo>().TPQ = ReadShort ();
		Debug.Log ("TPQ : " + GetComponent<MIDIInfo>().TPQ);
	}
	public Track SetTrack(int index){
		int eventType = 0;
		int totalDeltaT = 0;
			var newTrack = Instantiate (TrackPrefab,Vector3.zero,Quaternion.identity) as GameObject;
			newTrack.name = "Track" + index;
		newTrack.transform.parent = GameObject.Find ("TrackPool").transform;
			newTrack.GetComponent<Track> ().trackLen = ReadInt ();
			newTrack.GetComponent<Track> ().trackEnd = newTrack.GetComponent<Track> ().trackLen + offset;
		while(offset<newTrack.GetComponent<Track> ().trackEnd){
			int deltaT = ReadVarlen ();
			totalDeltaT += deltaT;

			int drawEvent = (int)Pop ();
			if(drawEvent>=EventNoteOff){
				eventType = (int)ReadByte ();
			}
			if (eventType >= EventNoteOff && eventType < EventNoteOff + 16) {
				var newNote = Instantiate (NotePrefab,Vector3.zero,Quaternion.identity) as GameObject;
				newNote.transform.parent = newTrack.transform;
				newNote.name = "Off";
				newNote.GetComponent<Note> ().flag = false;
				if (getLowNibble (eventType) < 9) {
					newNote.GetComponent<Note> ().instrument = "piano";
				} else {
					newNote.GetComponent<Note>().instrument="percussion";
				}
				newNote.GetComponent<Note> ().offset = totalDeltaT;
				newNote.GetComponent<Note> ().duration = deltaT;
				newNote.GetComponent<Note> ().key = ReadByte ();
				newNote.GetComponent<Note> ().velocity = ReadByte ();
				newTrack.GetComponent<Track> ().notes.Add (newNote.GetComponent<Note>());
			} else if (eventType >= EventNoteOn && eventType < EventNoteOn + 16) {
				var newNote = Instantiate (NotePrefab,Vector3.zero,Quaternion.identity) as GameObject;
				newNote.transform.parent = newTrack.transform;
				newNote.name = "On";
				if (getLowNibble (eventType) < 9) {
					newNote.GetComponent<Note> ().instrument = "piano";
				} else {
					newNote.GetComponent<Note>().instrument="percussion";
				}
				newNote.GetComponent<Note> ().offset = totalDeltaT;
				newNote.GetComponent<Note> ().duration = deltaT;
				newNote.GetComponent<Note> ().key = ReadByte ();
				newNote.GetComponent<Note> ().velocity = ReadByte ();
				newNote.transform.localPosition = new Vector3 ((totalDeltaT+deltaT*0.5f)*0.1f,index,0);
				if(deltaT*0.1f>0.3f)
				newNote.transform.localScale = new Vector3 (deltaT*0.1f,1,1);
				else
					newNote.transform.localScale = new Vector3 (0.3f,1,1);	
				if(newNote.GetComponent<Note>().velocity==0){
					newNote.name = "Off";
					newNote.SetActive (false);
				}
				newTrack.GetComponent<Track> ().notes.Add (newNote.GetComponent<Note>());
			} else if (eventType >= EventKeyPressure && eventType < EventKeyPressure + 16) {
				ReadByte ();
				ReadByte ();
			} else if (eventType >= EventControlChange && eventType < EventControlChange + 16) {
				ReadByte ();
				ReadByte ();
			} else if (eventType >= EventProgramChange && eventType < EventProgramChange + 16) {
				var value = ReadByte ();
				var lowNibble = getLowNibble (eventType);

			} else if (eventType >= EventChannelPressure && eventType < EventChannelPressure + 16) {
				ReadByte ();
			} else if (eventType == SysexEvent1) {
				int length = ReadVarlen ();
				ReadBytes(length);
			} else if (eventType == SysexEvent2) {
				int length = ReadVarlen ();
				ReadBytes(length);
			} else if (eventType == MetaEvent) {
				int metaEvent = ReadByte (); 
				int length = ReadVarlen ();
				byte[] metaValue = ReadBytes(length);
				if (metaEvent == MetaEventTimeSignature) {
					if (length != 4) {
					} else {
						//event.setDenominator(event.getMetaValue()[0]);
						//event.setNumerator((int) Math.pow(2, event.getMetaValue()[1]));
					}
				} else if (metaEvent == MetaEventKeySignature) {
					//System.out.println("Key signature check successed!");
				} else if (metaEvent == MetaEventTempo) {
					if (length != 3) {
					//	System.out.println("Error occurred while reading tempo data.");
					} else {
						GetComponent<MIDIInfo>().Tempo = 60000000 / ((int) ((metaValue[0] & 0xff) << 16) | (int) ((metaValue[1] & 0xff) << 8) | (int) (metaValue[2] & 0xff));
						GetComponent<MIDIInfo> ().TempoList.Add (GetComponent<MIDIInfo> ().Tempo);
						GetComponent<MIDIInfo> ().metaDeltaT.Add (totalDeltaT);
					}
				} else if (metaEvent == MetaEventEndOfTrack) {
				//	System.out.println("<Track End>");
					//break;
				}
			}
		//	Debug.Log (eventType);
		}
		return newTrack.GetComponent<Track>();
	}
	public void FindEvent(){
		while (currentTrackNumber < GetComponent<MIDIInfo> ().trackCount) {
			string trackChunk = ReadChars (4);
			if (trackChunk == "MTrk") {
				Debug.Log ("MTrk is founded");
				GetComponent<MIDIInfo> ().tracks.Add (SetTrack (currentTrackNumber));
				currentTrackNumber++;
			}
			Debug.Log ("in While");
		}
		Debug.Log ("out While");
	}

	private void Go(){
		MidiFile = new BinaryReader (File.OpenRead (path));
		fileInfo = new FileInfo (path);
		MidiBytes = new byte[(int)fileInfo.Length];

		for(int i = 0;i<(int)fileInfo.Length;i++){
			MidiBytes [i] = MidiFile.ReadByte ();
		}
		FileRead ();
		FindEvent ();
		GetComponent<MIDIInfo> ().metaDeltaT.Sort ();
		GetComponent<MIDIPlayer>().deltaTPQ = Mathf.Round(GetComponent<MIDIInfo> ().TPQ * (GetComponent<MIDIInfo> ().Tempo)/60 * Time.deltaTime);
	}

	void Start(){
		inputPath = GameObject.Find	("Path").GetComponent<Text>();
		if(inputPath.text!="")
		path = "Assets/" + inputPath.text + ".mid";
		Go ();
		Debug.Log (GetComponent<MIDIPlayer>().deltaTPQ );
	}
	void Reset(){
		path = "Assets/" + inputPath.text + ".mid";
		for(int i = 0;i<GameObject.Find("TrackPool").transform.childCount;i++){
			Destroy (GameObject.Find("TrackPool").transform.GetChild(i).gameObject);
			Debug.Log ("Destroy");
		}
		offset = 0;
		currentTrackNumber = 0;
		GetComponent<MIDIInfo> ().TPQ = 0;
		GetComponent<MIDIPlayer> ().SumTPQ = 0;
		GetComponent<MIDIInfo> ().startOffset = 0;
		GetComponent<MIDIInfo> ().Tempo = 120;
		GetComponent<MIDIInfo> ().count = 0;
		//for(int i = 0; i<GetComponent<MIDIInfo>().tracks.Count;i++){
		GetComponent<MIDIInfo> ().tracks = new List<Track>(0);
	//	}
			GetComponent<MIDIInfo> ().TempoList = new List<float>(0);
			Debug.Log ("Removeat");
		//for(int i = 0; i<GetComponent<MIDIInfo>().metaDeltaT.Count;i++){
		GetComponent<MIDIInfo> ().metaDeltaT = new List<int>(0);
	//	}

	}
	public Text inputPath;

	public void Recall(){
		if (GetComponent<MIDIPlayer> ().SumTPQ != 0) {
			Reset ();
			Go ();
			Time.timeScale = 1;
		}
	}

	// Update is called once per frame
	void Update () {
		/*if (path != "Assets/" + inputPath.text + ".mid" && inputPath.text.Length != 0) {
			if (GetComponent<MIDIPlayer> ().SumTPQ != 0)
			{
				Reset ();
			Go ();
			}
		}*/
	}
}
