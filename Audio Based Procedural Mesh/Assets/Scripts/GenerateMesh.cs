using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Quarter-anchored NoteDuration.
public static class NoteDuration
{
    public static readonly float Whole = 4f;
    public static readonly float Half = 2f;
    public static readonly float Quarter = 1f; // Default
    public static readonly float Eighth = 0.5f;
    public static readonly float Sixteenth = 0.25f;

    public static float GetDurationSpan(float startTime, float endTime, float mpb)
    {
        float spanTime = (endTime - startTime);
        float rtn = 0;
        while (spanTime > mpb * Sixteenth)
        {
            if ((mpb * Whole) < spanTime)
            {
                spanTime -= mpb * Whole;
                rtn += Whole;
            }

            else if ((mpb * Half) < spanTime)
            {
                spanTime -= mpb * Half;
                rtn += Half;
            }

            else if ((mpb * Quarter) < spanTime)
            {
                spanTime -= mpb * Quarter;
                rtn += Quarter;
            }

            else if ((mpb * Eighth) < spanTime)
            {
                spanTime -= mpb * Eighth;
                rtn += Eighth;
            }

            else
            {
                spanTime -= mpb * Sixteenth;
                rtn += Sixteenth;
            }
        }
        rtn = (rtn == 0) ? Sixteenth : rtn;
        Debug.Log("This duration is: " + rtn + " beats.");
        return rtn;
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]


public class GenerateMesh : MonoBehaviour
{

    public bool RequireBeatmap;

    private float[] validRightAngles = new float[5];
    private float[] validLeftAngles = new float[5];

    // List of all the Mesh meta stuff
    public List<Vector3> Vertices; // Vert Array
    public List<Vector2> UVs; // UV Array
    public List<int> Triangles; // Triangle List
    public List<Vector3> Normals;
    public List<Color32> Colors;

    public List<Vector3> allPoints;

    private BeatmapMeta beatmapMeta;
    private float defaultStep = NoteDuration.Quarter;

    private float val = 0f;

    private float previousTime = 0f;
    private float currentTime = 0f;
    private float endTime;
    
    public int totalTubeLength = 400;
    private float depthChange = 140f; // Depth change based off of bpm. This i guess could literally be BPM

    // Colors
    private bool isRed = true;
    Color32 d_red = new Color(0.47f, 0.352f, 0.352f, 1.0f);
    Color32 d_blue = new Color(0.608f, 0.682f, 0.796f, 1.0f);
    Color32 d_gray = new Color(0.3f, 0.3f, 0.3f, 1.0f);

    // Constants
    private const float VAL_CHANGE = .1f;
    private const int tubeSegmentSize = 15; // Resolution of Tube.
    private float TUBE_RADIUS = 30f;
    private const float PI = Mathf.PI;

    void Start()
    {
        // Turn right/up
        validRightAngles[0] = 5f;
        validRightAngles[1] = 10f;
        validRightAngles[2] = 15f;
        validRightAngles[3] = 20f;
        validRightAngles[4] = 25f;

        // Turn left/down
        validLeftAngles[0] = 335f;
        validLeftAngles[1] = 340f;
        validLeftAngles[2] = 345f;
        validLeftAngles[3] = 350f;
        validLeftAngles[4] = 355f;
        // ugh zero case?
    }

    public void Generate()
    {
        float differenceMultiplier = 15f;
        float depthDivisor = 5f;
        float currentDistance = 0f;

        if (RequireBeatmap && beatmapMeta.BPM == 0) {
            Debug.Log("Hey bruh! Select a beatmap!");
            return;
        }

        // Load BeatmapMeta
        beatmapMeta = GameObject.Find("BeatmapMeta").GetComponent<BeatmapMeta>();
        if (beatmapMeta.hitObjects.Count > 0) {
            totalTubeLength = beatmapMeta.hitObjects.Count;
        }

        endTime = beatmapMeta.hitObjects[beatmapMeta.hitObjects.Count - 1].Time;

        for (int i = 0; i < totalTubeLength; i++)
        {
            val += VAL_CHANGE;
            //allPoints.Add(new Vector3(Mathf.Sin(val) * 30, Mathf.Cos(val) * 30, val * depthChange)); // Spiral Test.
            currentTime = beatmapMeta.hitObjects[i].Time;
            // currentDistance += (currentTime - previousTime) / depthDivisor;
            currentDistance += beatmapMeta.BPM * 0.5f * NoteDuration.GetDurationSpan(previousTime, currentTime, beatmapMeta.MPB);
            Debug.Log("Distance Delta: " + beatmapMeta.BPM * 2f * NoteDuration.GetDurationSpan(previousTime, currentTime, beatmapMeta.MPB));
            allPoints.Add(new Vector3(Random.Range(-1f, 1f) * differenceMultiplier, Random.Range(-1f, 1f) * differenceMultiplier, currentDistance /*+ ((currentTime - previousTime) / depthDivisor)*/));
            // Calculate point based off of timing;

            previousTime = currentTime;
        }

        /*
        // Based off of beatmap, load the anchor points of the mesh.
        int currentIndex = 0;
        int LeftRightAngleIndex = 0;
        int UpDownAngleIndex = 0;
        int maxIndexes = 0;
        while (currentTime < endTime || maxIndexes > 500)
        {
            val += VAL_CHANGE * NoteDuration.Quarter;

            // We haven't hit the start of the tunnel yet, keep going in the '0' direction.
            if (currentTime < beatmapMeta.hitObjects[0].Time)
            {
                allPoints.Add(new Vector3(0, 0, val * depthChange));
                previousTime = currentTime;
                continue;
            }

            // New hit circle detected, change tunnel direction.
            else if (currentTime > beatmapMeta.hitObjects[currentIndex].Time)
            {
                currentIndex++;
                LeftRightAngleIndex = Random.Range(-1 * (int)differenceMultiplier, 1 * (int)differenceMultiplier);
                UpDownAngleIndex = Random.Range(-1 * (int)differenceMultiplier, 1 * (int)differenceMultiplier);
                previousTime = currentTime;
            }

            // No new HitObject detected, keep going in the same direction.
            else
            {
                allPoints.Add(new Vector3(LeftRightAngleIndex, UpDownAngleIndex, val * depthChange));
            }

            maxIndexes++;
            currentTime += Mathf.RoundToInt(beatmapMeta.MPB * NoteDuration.Quarter);
        }*/

        CreateAllSegments(true);
        CreateAllSegments(false);

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = Vertices.ToArray();
        mesh.uv = UVs.ToArray();
        mesh.triangles = Triangles.ToArray();
        mesh.normals = Normals.ToArray();
        mesh.colors32 = Colors.ToArray();

        Renderer rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Vertex Colored");
        /*
        for (int i = 0; i < mesh.colors32.Length; i++)
        {
            Debug.Log("Mesh Colors!: " + mesh.colors32);
        }*/

        mesh.UploadMeshData(true);
        mesh.Optimize();
        GetComponent<MeshCollider>().sharedMesh = mesh;
        //mesh.RecalculateNormals();

    }

    void CreateAllSegments(bool interior)
    {
        Vector3 previousPosition = allPoints[0]; // Initial Position

        int uvy = 0;

        for (int i = 1; i < allPoints.Count; i++)
        {
            // Get position of next vert.
            Vector3 position = allPoints[i];
            Vector3 dif = (position - previousPosition).normalized;
           
            Vector3 right = Vector3.Cross(Vector3.up, dif).normalized;
            Vector3 forward = Vector3.Cross(dif, right).normalized;

            // Determine tube facing. 
            if (interior)
            {
                GenerateLinearSegment(previousPosition, forward, right, TUBE_RADIUS * .9f, (float)uvy);
            }
            else
            {
                //GenerateLinearSegment(previousPosition, position, right, forward, TUBE_RADIUS, (float)uvy);
                GenerateLinearSegment(previousPosition, right, forward, TUBE_RADIUS, (float)uvy); //Flip these to invert faces
            }

            Debug.DrawLine(previousPosition, position, Color.red, 60f);
            previousPosition = position;

            uvy = ~uvy;
        }

        RemoveLastPart();
    }

    void RemoveLastPart()
    {
        Triangles.RemoveRange(Triangles.Count - tubeSegmentSize * 6, tubeSegmentSize * 6);
    }


    void GenerateLinearSegment(Vector3 vertexPosition, Vector3 forward, Vector3 right, float TUBE_RADIUS, float uvy)
    { 
        float stepAmount = (PI * 2f) / tubeSegmentSize;
        int segmentStartIndex = Vertices.Count;
        float val = 180f;

        float uv = 0;
        float uvStep = 1f / tubeSegmentSize; 

        int i = 0;
        for (i = 0; i < tubeSegmentSize - 1; i++)
        {
            Vector3 normal = (forward * Mathf.Sin(val) + right * Mathf.Cos(val));
            Vector3 newVert = vertexPosition + normal * TUBE_RADIUS;

            Vertices.Add(newVert);
            Normals.Add(normal);
            Color32 Color2Add = (isRed) ? d_red : d_gray;
            Colors.Add(Color2Add);

            UVs.Add(new Vector2(uv, uvy));

            Triangles.Add(segmentStartIndex + i);
            Triangles.Add(segmentStartIndex + i + tubeSegmentSize);
            Triangles.Add(segmentStartIndex + i + tubeSegmentSize + 1);

            Triangles.Add(segmentStartIndex + i);
            Triangles.Add(segmentStartIndex + i + 1 + tubeSegmentSize);
            Triangles.Add(segmentStartIndex + i + 1);

            /*
            for (int j = 0; j < 6; j++)
            {
                Color32 Color2Add = (isRed) ? d_red : d_gray;
                Colors.Add(Color2Add);
            }
             * */

            val += stepAmount;
            uv += uvStep;
        }

        val += stepAmount;
        //Last triangle to close the loop
        
        Vector3 tri1 = (forward * Mathf.Sin(val) + right * Mathf.Cos(val));
        Vector3 endVert = vertexPosition + tri1 * TUBE_RADIUS;

        val += stepAmount;

        Vector3 tri2 = (forward * Mathf.Sin(val) + right * Mathf.Cos(val)); 
        Vector3 nextVert = vertexPosition + tri2 * TUBE_RADIUS;

        Color32 Color2Add2 = (isRed) ? d_red : d_gray;
        Colors.Add(Color2Add2);
        Colors.Add(Color2Add2);
        Vertices.Add(endVert);
        Vertices.Add(nextVert);

        Normals.Add(tri1);
        Normals.Add(tri2);

        UVs.Add(new Vector2(uv, uvy));
        UVs.Add(new Vector2(uv + uvStep, uvy));

        Triangles.Add(segmentStartIndex + i);
        Triangles.Add(segmentStartIndex + i + tubeSegmentSize);
        Triangles.Add(segmentStartIndex + i + tubeSegmentSize + 1);

        Triangles.Add(segmentStartIndex + i + 1);
        Triangles.Add(segmentStartIndex + i + tubeSegmentSize + 2);
        Triangles.Add(segmentStartIndex + i + 2);
        
        /*
        for (int j = 0; j < 6; j++)
        {
            Color32 Color2Add = (isRed) ? d_red : d_gray;
            Colors.Add(Color2Add);
        }
         */

        isRed = !isRed;
    }

    // TODO if I can figure it out
    void GenerateBezeirSegment()
    {

    }
}
