    using UnityEngine;
using System.Collections;
using BMAPI;
using BMAPI.v1;
public class APITest : MonoBehaviour
{

    BMAPI.v1.Beatmap beatmap = new BMAPI.v1.Beatmap("D:\\Documents HD\\Classes\\Game Development\\Procedural Mesh Final\\Audio Based Procedural Mesh\\Beatmaps\\155118 Drop - Granat\\Drop - Granat (Lan wings) [Extra].osu");
    
    // Use this for initialization
    void Start()
    {
        Debug.Log(beatmap.ApproachRate);
        Debug.Log(beatmap.Artist);
        Debug.Log(beatmap.OverallDifficulty);
        Debug.Log(beatmap.Filename);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
