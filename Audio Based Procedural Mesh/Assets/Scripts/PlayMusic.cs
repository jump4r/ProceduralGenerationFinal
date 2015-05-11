using UnityEngine;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;

/// <summary>
/// Online found fix for translating mp3-to-OGG files to play in Unity.
/// </summary>
public class PlayMusic : MonoBehaviour {

    private string Pcommand;
    private bool isPlaying = false;
    [DllImport("winmm.dll")]
    private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, int bla);

    /// Stops currently playing audio file
    public void Close()
    {
        Pcommand = "close Song";
        mciSendString(Pcommand, null, 0, 0);
        isPlaying = false;
    }

    /// Opens audio file to play
    public void Open(string sFileName)
    {
        Pcommand = "open \"" + sFileName + "\" type mpegvideo alias Song";
        mciSendString(Pcommand, null, 0, 0);
    }

    /// Plays selected audio file
    public void Play()
    {
        Pcommand = "play Song";
        mciSendString(Pcommand, null, 0, 0);
        isPlaying = true;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnApplicationQuit()
    {
        Close();
        isPlaying = false;
    }
}
