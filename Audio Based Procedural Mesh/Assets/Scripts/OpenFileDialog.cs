using UnityEngine;
using System.Collections;
using UnityEditor;
using BMAPI.v1;

public class OpenFileDialog : MonoBehaviour {

    private Beatmap beatmap;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SelectFile()
    {
        string path = EditorUtility.OpenFilePanel("Select .osu Beatmap File", "./Beatmaps", "osu");
        if (path.Length != 0)
        {
            // var www = WWW("file:///" + path);
            Debug.Log("Beatmap Loaded");
            beatmap = new Beatmap(path);
        }
    }
}
