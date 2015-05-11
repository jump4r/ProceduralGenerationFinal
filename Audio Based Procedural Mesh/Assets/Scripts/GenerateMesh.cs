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

    public List<Vector3> allPoints;

    private BeatmapMeta beatmapMeta;
    private float defaultStep = NoteDuration.Quarter;

    private float val = 0f;

    private float previousTime = 0f;
    private float currentTime;
    
    public int totalTubeLength = 400;
    private float depthChange = 140f; // Depth change based off of bpm. This i guess could literally be BPM

    // Constants
    private const float VAL_CHANGE = .1f;
    private const int tubeSegmentSize = 15; // Resolution of Tube.
    private float TUBE_RADIUS = 15f;
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
        float differenceMultiplier = 4f;

        if (RequireBeatmap && beatmapMeta.BPM == 0) {
            Debug.Log("Hey bruh! Select a beatmap!");
            return;
        }

        // Load BeatmapMeta
        beatmapMeta = GameObject.Find("BeatmapMeta").GetComponent<BeatmapMeta>();
        if (beatmapMeta.hitObjects.Count > 0) {
            totalTubeLength = beatmapMeta.hitObjects.Count;
        }

        for (int i = 0; i < totalTubeLength; i++)
        {
            val += VAL_CHANGE;
            //allPoints.Add(new Vector3(Mathf.Sin(val) * 30, Mathf.Cos(val) * 30, val * depthChange)); // Spiral Test.
            allPoints.Add(new Vector3(Random.Range(-1f, 1f) * differenceMultiplier, Random.Range(-1f, 1f) * differenceMultiplier, val * depthChange));
            currentTime = beatmapMeta.hitObjects[i].Time;
            // Calculate point based off of timing;
            Debug.Log("Time Difference for index: " + i + " is " + (currentTime - previousTime));
            previousTime = currentTime;
        }

        CreateAllSegments(true);
        CreateAllSegments(false);

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = Vertices.ToArray();
        mesh.uv = UVs.ToArray();
        mesh.triangles = Triangles.ToArray();
        mesh.normals = Normals.ToArray();

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
            Vector3 right = (dif != Vector3.up) ? Vector3.Cross(Vector3.up, dif).normalized : Vector3.Cross(dif, Vector3.right).normalized;
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

            UVs.Add(new Vector2(uv, uvy));

            Triangles.Add(segmentStartIndex + i);
            Triangles.Add(segmentStartIndex + i + tubeSegmentSize);
            Triangles.Add(segmentStartIndex + i + tubeSegmentSize + 1);

            Triangles.Add(segmentStartIndex + i);
            Triangles.Add(segmentStartIndex + i + 1 + tubeSegmentSize);
            Triangles.Add(segmentStartIndex + i + 1);

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
    }

    // TODO if I can figure it out
    void GenerateBezeirSegment()
    {

    }
}
