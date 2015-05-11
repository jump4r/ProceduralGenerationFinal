using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BMAPI;
using BMAPI.v1;

public class HitObjectMeta {
    public Vector2 Position;
    public int Time;
    public HitObjectType Type;

    public HitObjectMeta() { }
    public HitObjectMeta(Vector2 position, int time, HitObjectType type) {
        this.Position = position;
        this.Time = time;
        this.Type = type;
    }
}

public class BeatmapMeta : MonoBehaviour {

    public Beatmap beatmap;

    private TimingPoint initialTimingPoint;

    public float BPM;
    public float MPB;

    // Path Variables
    private string audioPath;
    private string beatmapPath;

    // Getters;
    private AudioSource audioSource;
    private PlayMusic musicPlayer;

    // API Getters
    public List<HitObjectMeta> hitObjects = new List<HitObjectMeta>();

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        musicPlayer = GetComponent<PlayMusic>();
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
        BPM = Mathf.RoundToInt(60000f / MPB);

        // Debug Test
        Debug.Log("Milleseconds Per Beat.: " + MPB);
        Debug.Log("BPM: " + BPM);
        audioPath = GetAudioPath(beatmap.Filename);
        Debug.Log("Audio Path: " + audioPath);

        for (int i = 0; i < beatmap.HitObjects.Count; i++)
        {
            // Why are these floats Smoogipoo what are you doing.
            float _x = beatmap.HitObjects[i].Location.X;
            float _y = beatmap.HitObjects[i].Location.Y;
            Vector2 _position = new Vector2((int)_x, (int)_y);

            // This might mess up millesecond stuff.
            float _time = beatmap.HitObjects[i].StartTime;
            HitObjectType _type = beatmap.HitObjects[i].Type;
            hitObjects.Add(new HitObjectMeta(_position, Mathf.RoundToInt(_time), _type));
        }

        musicPlayer.Open(audioPath);
        // musicPlayer.Play();
        // PLEASE NOTE: EVEN IF YOU CLOSE THE APPLICATION, THE MUSIC DOESN'T STOP.
        // #FIX IT BABY
    }

    private string GetAudioPath(string path)
    {
        string[] splitPath = path.Split('\\');
        string rtn = "";
        for (int i = 0; i < splitPath.Length - 1; i++)
        {
            rtn += splitPath[i];
            rtn += "\\";
        }
        rtn += beatmap.AudioFilename;
        return rtn;
    }

    private void LoadMP3(string mp3FilePath)
    {
        WWW www = new WWW(mp3FilePath);
        audioSource.clip = www.GetAudioClip(false);
    }

    private void PlayMP3()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
