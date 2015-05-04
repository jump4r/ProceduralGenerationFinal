    using UnityEngine;
using System.Collections;
using BMAPI;
using BMAPI.v1;
public class APITest : MonoBehaviour
{

    BMAPI.v1.Beatmap beatmap = new BMAPI.v1.Beatmap("C:\\Program Files (x86)\\osu!\\Songs\\65853 Blue Stahli - Shotgun Senorita (Zardonic Remix)\\Blue Stahli - Shotgun Senorita (Zardonic Remix) (Aleks719) [AUTO].osu");

    // Use this for initialization
    void Start()
    {
        Debug.Log(beatmap.ApproachRate);
        Debug.Log(beatmap.Artist);
        Debug.Log(beatmap.OverallDifficulty);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
