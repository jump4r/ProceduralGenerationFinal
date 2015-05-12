using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Navigate : MonoBehaviour {

    public List<Vector3> vertexPoints;
    public GenerateMesh meshScript;
    public BeatmapMeta beatmap;
    public PlayMusic music;

    private bool canNavigate = false;
    private int currentIndex = 0;

    private float moveSpeed ;
    private float rotationSpeed = 2f;

    private float xOffset = 5f;
    private float yOffset = 5f;

	// Use this for initialization
	void Start () {
        meshScript = GameObject.Find("Mesh").GetComponent<GenerateMesh>();
        music = GameObject.Find("BeatmapMeta").GetComponent<PlayMusic>();
        beatmap = GameObject.Find("BeatmapMeta").GetComponent<BeatmapMeta>();
	}
	
	// Update is called once per frame
	void Update () {
        if (canNavigate)
        {
            Vector3 lookDireciton;
            Debug.Log("Begin Navigation of the tunnel!");
            if (currentIndex < (vertexPoints.Count - 2))
            {
                if (Vector3.Distance(transform.position, vertexPoints[currentIndex + 1]) < 40f)
                {
                    currentIndex++;
                    // Magic numbers for the offset I guess
                    vertexPoints[currentIndex + 1] = new Vector3(vertexPoints[currentIndex + 1].x + xOffset, vertexPoints[currentIndex + 1].y + yOffset, vertexPoints[currentIndex + 1].z);
                    Debug.Log("Hit Vertex, Move on!");
                }

                Debug.Log("Distance between Player and next hit point is: " + Vector3.Distance(transform.position, vertexPoints[currentIndex + 1]));
                // TODO: Match movespeed with bpm so curves match up.

                transform.Translate(Vector3.forward * moveSpeed, Space.Self);
                lookDireciton = (vertexPoints[currentIndex + 1] - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(lookDireciton);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            }

            else
            {
                canNavigate = false;
            }
        }
	}

    public void EnableNavigation()
    {
        music.Play();
        canNavigate = true;
        vertexPoints = meshScript.allPoints;
        Debug.Log("Vertex Points Size: " + vertexPoints.Count);
        transform.position = vertexPoints[currentIndex];
        moveSpeed = beatmap.BPM * 0.02183410822f; // This was calculated I promise.
    }
}
