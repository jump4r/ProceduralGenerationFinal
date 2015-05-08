using UnityEngine;
using System.Collections;
using UnityEditor;
using BMAPI.v1;

public class OpenFileDialog : MonoBehaviour {

    private BMAPI.v1.Beatmap m_beatmap;

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
            string newPath = path.Replace('/', '\\');
            Debug.Log(newPath + " Beatmap Loaded");
            m_beatmap = new BMAPI.v1.Beatmap(newPath);
            gameObject.GetComponent<BeatmapMeta>().LoadBeatmap(m_beatmap);
        }
    }
}
