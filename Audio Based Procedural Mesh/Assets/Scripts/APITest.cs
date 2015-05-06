    using UnityEngine;
using System.Collections;
using BMAPI;
using BMAPI.v1;
public class APITest : MonoBehaviour
{

    BMAPI.v1.Beatmap beatmap = new BMAPI.v1.Beatmap("D:\\osu\\Songs\\207806 Tujamo & Plastik Funk feat Sneakbo - Dr Who! (Smooth Remix)\\Tujamo & Plastik Funk feat. Sneakbo - Dr. Who! (Smooth Remix) (Natteke) [Extra].osu");

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
