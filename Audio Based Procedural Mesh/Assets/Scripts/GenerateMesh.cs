using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
 
public class GenerateMesh : MonoBehaviour
{
    public List<Vector3> newVertices;
    public List<Vector2> newUV;
    public List<int> newTriangles;
    public List<Vector3> newNormals;

    public List<Vector3> allPoints;
    private float val = 0f;

    void Start()
    {
        //	Generate();
    }

    public float towerRadius = 20f;
    public float heightChange = 1f;
    public int totalTowerHeight = 400;
    public float valChange = .005f;
    public void Generate()
    {
        newVertices.Clear();
        newTriangles.Clear();
        newNormals.Clear();
        newUV.Clear();

        float differenceMultiplier = 6f;

        for (int i = 0; i < totalTowerHeight; i++)
        {
            val += valChange;
            float tempRadius = towerRadius * Mathf.Sin(val / 2f);
            // allPoints.Add(new Vector3(Mathf.Sin(val) * 10, Mathf.Cos(val) * 10, val * heightChange)); // Spiral Test.
            allPoints.Add(new Vector3(Random.Range(-1f, 1f) * differenceMultiplier, Random.Range(-1f, 1f) * differenceMultiplier, val * heightChange));
        }

        CreateLoop(true);
        CreateLoop(false);

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = newVertices.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.normals = newNormals.ToArray();

        GetComponent<MeshCollider>().sharedMesh = mesh;
        //mesh.RecalculateNormals();

    }

    void CreateLoop(bool interior)
    {

        float newVal = 0f;
        float tempRadius = towerRadius * Mathf.Sin(newVal / 2f);
        Vector3 previousPosition = new Vector3(Mathf.Sin(newVal) * tempRadius, newVal * heightChange, Mathf.Cos(newVal) * tempRadius);

        int uvy = 0;

        for (int i = 0; i < allPoints.Count; i++)
        {
            newVal += valChange;

            tempRadius = towerRadius * Mathf.Sin(newVal / 2f);

            print(tempRadius);
            // Vector3 position = new Vector3(Mathf.Sin(newVal) * tempRadius, newVal * heightChange, Mathf.Cos(newVal) * tempRadius);
            Vector3 position = allPoints[i];
            Vector3 dif = position - previousPosition;
            dif.Normalize();

            Vector3 right = Vector3.Cross(Vector3.up, dif).normalized;
            Vector3 forward = Vector3.Cross(dif, right).normalized;

            if (dif == Vector3.up)
            {
                right = Vector3.Cross(dif, Vector3.right).normalized;
                forward = Vector3.Cross(dif, right).normalized;
            }

            // float realTubeRadius = Mathf.Sin(newVal / .1f) * tubeRadius * .1f + tubeRadius;
            float realTubeRadius = tubeRadius;

            //interior 
            if (interior)
            {
                GeneratePart(previousPosition, position, forward, right, realTubeRadius * .8f, (float)uvy);
            }
            else
            {
                GeneratePart(previousPosition, position, right, forward, realTubeRadius, (float)uvy);//Flip these to invert faces
            }

            Debug.DrawLine(previousPosition, position, Color.red, 60f);
            previousPosition = position;
            uvy = ~uvy;
        }

        RemoveLastPart();
    }

    void RemoveLastPart()
    {
        newTriangles.RemoveRange(newTriangles.Count - tubeResolution * 6, tubeResolution * 6);
    }

    public float tubeRadius = 1f;
    public int tubeResolution = 5;

    void GeneratePart(Vector3 start, Vector3 end, Vector3 forward, Vector3 right, float tubeRadius, float uvy)
    {
        int segmentStartIndex = newVertices.Count;

        float stepAmount = (Mathf.PI * 2f) / tubeResolution;
        float val = 180f;

        float uv = 0;
        float uvStep = 1f / tubeResolution;

        int i = 0;
        for (i = 0; i < tubeResolution - 1; i++)
        {
            Vector3 normal = (forward * Mathf.Sin(val) + right * Mathf.Cos(val));
            Vector3 newVert = start + normal * tubeRadius;

            newVertices.Add(newVert);
            newNormals.Add(normal);

            newUV.Add(new Vector2(uv, uvy));

            newTriangles.Add(segmentStartIndex + i);
            newTriangles.Add(segmentStartIndex + i + tubeResolution);
            newTriangles.Add(segmentStartIndex + i + tubeResolution + 1);

            newTriangles.Add(segmentStartIndex + i);
            newTriangles.Add(segmentStartIndex + i + 1 + tubeResolution);
            newTriangles.Add(segmentStartIndex + i + 1);

            val += stepAmount;
            uv += uvStep;
        }

        val += stepAmount;
        //Last triangle to close the loop
        Vector3 n1 = (forward * Mathf.Sin(val) + right * Mathf.Cos(val));
        Vector3 endVert = start + n1 * tubeRadius;

        val += stepAmount;

        Vector3 n2 = (forward * Mathf.Sin(val) + right * Mathf.Cos(val));
        Vector3 nextVert = start + n2 * tubeRadius;

        newVertices.Add(endVert);
        newVertices.Add(nextVert);

        newNormals.Add(n1);
        newNormals.Add(n2);

        newUV.Add(new Vector2(uv, uvy));
        newUV.Add(new Vector2(uv + uvStep, uvy));

        newTriangles.Add(segmentStartIndex + i);
        newTriangles.Add(segmentStartIndex + i + tubeResolution);
        newTriangles.Add(segmentStartIndex + i + tubeResolution + 1);

        newTriangles.Add(segmentStartIndex + i + 1);
        newTriangles.Add(segmentStartIndex + i + 1 + tubeResolution + 1);
        newTriangles.Add(segmentStartIndex + i + 1 + 1);

    }
}
