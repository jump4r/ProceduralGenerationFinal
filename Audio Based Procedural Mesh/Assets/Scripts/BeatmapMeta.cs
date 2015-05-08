using UnityEngine;
using System.Collections;
using BMAPI;
using BMAPI.v1;

public class BeatmapMeta : MonoBehaviour {

    public Beatmap beatmap;

    private TimingPoint initialTimingPoint;

    public float BPM;
    public float MPB;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Load in the beatmap
    /// </summary>
    /// <param name="m_beatmap"></param>
    public void LoadBeatmap(Beatmap m_beatmap)
    {
        beatmap = m_beatmap;

        initialTimingPoint = beatmap.TimingPoints[0];
        MPB = initialTimingPoint.BpmDelay;

        // Debug Test
        Debug.Log("Milleseconds Per Beat.: " + MPB);
    }
}
