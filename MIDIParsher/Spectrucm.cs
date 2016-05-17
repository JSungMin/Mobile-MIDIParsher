using UnityEngine;
using System.Collections;

public class Spectrucm : MonoBehaviour {

	public float radius = 50;
	public GameObject cube;
	public GameObject[] cubes;
	// Use this for initialization
	void Start () {
		for(int i = 0; i<20;i++){
			float angle = i * Mathf.PI * 2 / 20;
			Vector3 pos = new Vector3 (Mathf.Cos(angle),0,Mathf.Sin(angle))*radius+Vector3.forward*40;
			var newCube = Instantiate (cube,pos,Quaternion.identity) as GameObject;
		}
		for(int i = 0; i<10;i++){
			float angle = i * Mathf.PI * 2 / 10;
			Vector3 pos = new Vector3 (Mathf.Cos(angle),0,Mathf.Sin(angle))*radius*0.5f+Vector3.forward*40;
			var newCube = Instantiate (cube,pos,Quaternion.identity) as GameObject;
		}
		cubes = GameObject.FindGameObjectsWithTag ("Spectrucm");
	}
	
	// Update is called once per frame
	void Update () {
		float[] spectrum = AudioListener.GetSpectrumData (1024,0,FFTWindow.Hamming);
		for(int i = 0;i<20;i++){
			Vector3 previousScale = cubes [i].transform.localScale;
			previousScale.y = spectrum [i] * 50;
			cubes [i].transform.localScale = previousScale;
		}
		for(int i = 20;i<30;i++){
			Vector3 previousScale = cubes [i].transform.localScale;
			previousScale.y = spectrum [i] * 100;
			cubes [i].transform.localScale = previousScale;
		}
	}
}
