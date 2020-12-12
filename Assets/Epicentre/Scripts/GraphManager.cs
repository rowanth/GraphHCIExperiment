using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using SFB;
using System.Linq;

// UXF namespace
using UXF;

public class GraphManager : MonoBehaviour
{
    //BEGIN Load From File Helper Vars
    [System.Serializable]
    public class GraphInfo {
        //public int numNodes;
        //public int numEdges;
        public List<NodeJSON> nodes;
        public List<EdgeJSON> edges;
        //public int[] marginalsShape;
        //public List<string> marginalsValueNames;
        //public float[] marginals;
    }

    [System.Serializable]
    public class NodeJSON
    {
        public float x;
        public float y;
        public float z;

        public int id;
    }

    [System.Serializable]
    public class EdgeJSON
    {
        public int source;
        public int target;

        public int id;
    }

    Vector3[] vertices = new Vector3[0];
    List<int> lines = new List<int>();
    List<int> ids = new List<int>();
    Mesh mesh;
    int marginalTimesteps = 0;
    int marginalNodes = 0;
    int marginalValues = 0;

    float[,,] marginals = new float[0, 0, 0];
    List<string> marginalsValueNames = new List<string>();
    int prevMargTimestep = -1;

    public Transform graphParent;
    public Transform nodesParent;
    public Material edgeMaterial;

    public GameObject nodeTemplate;
    //END Load From File Helper Vars

    public Node nodePrefab;
    public Button confirmButton;

    int numSequence;

    public void Awake()
    {

    }
    public void CreateGraphAndPlay(Trial trial)
    {
        StopAllCoroutines();
        // run in a coroutines to take advantage of delays
        StartCoroutine(CreateGraphAndPlaySequence(trial));
    }

    public IEnumerator CreateGraphAndPlaySequence(Trial trial)
    {
        foreach (Transform child in transform) Destroy(child.gameObject);

        // after a delay, create the graph
        yield return new WaitForSeconds(0.25f);

        string fileName = (string)trial.settings.GetObject("filename");
        CreateGraph(fileName);

        // show confirm button
        confirmButton.gameObject.SetActive(true);
    }

    public void Confirm()
    {
        // if somehow the button is clicked when we are not in a trial, do nothing
        if (!Session.instance.InTrial) return;

        // hide confirm button
        confirmButton.gameObject.SetActive(false);

        // end trial
        Session.instance.CurrentTrial.End();
    }

    #region GRAPH_CREATION
    private void CreateMesh(int numVerts)
    {
        Debug.Log("Creating mesh with " + numVerts + " nodes");
        GameObject meshObj = new GameObject();
        meshObj.transform.SetParent(graphParent, false);
        meshObj.name = "mesh";

        MeshRenderer meshRenderer = meshObj.AddComponent<MeshRenderer>();
        meshRenderer.material = edgeMaterial;

        MeshFilter meshFilter = meshObj.AddComponent<MeshFilter>();

        if (mesh) {
            mesh.Clear();
        }
        else {
            mesh = new Mesh();
        }

        vertices = new Vector3[numVerts];
        // place nodes on a unit circle by default
        for (int i = 0; i < vertices.Length; i++)
        {
            float deg = (i * 360.0f) / numVerts;
            vertices[i] = new Vector3(Mathf.Cos(deg * Mathf.Deg2Rad), Mathf.Sin(deg * Mathf.Deg2Rad), 0);
        }
        mesh.vertices = vertices;

        // no edges yet
        lines = new List<int>();
        mesh.SetIndices(lines, MeshTopology.Lines, 0);

        meshFilter.mesh = mesh;
    }

    void CreateGraph(String filename)
    {
        Debug.Log("Loading cache from " + filename);

        GraphInfo gi = JsonUtility.FromJson<GraphInfo>(File.ReadAllText(filename));

        CreateMesh(gi.nodes.Count);

        vertices = gi.nodes.OrderBy(t => t.id-1).Select(t => new Vector3(t.x, t.y, t.z)).ToArray();
        lines = gi.edges.Select(t => new List<int>() { t.source-1, t.target-1 }).SelectMany(item => item).ToList();

        UpdateMesh();
        CreateNodes();
        //marginals = To3DArray(gi.marginalsShape[0], gi.marginalsShape[1], gi.marginalsShape[2], gi.marginals);
        //marginalsValueNames = gi.marginalsValueNames;

        //statusText.gameObject.SetActive(false);
        //SetDropdownOptions(marginalsValueNames);
    }

    void UpdateMesh()
    {
        mesh.vertices = vertices;
        mesh.SetIndices(lines, MeshTopology.Lines, 0);
    }

    private void CreateNodes()
    {
        // remove any already there
        foreach (Transform child in nodesParent)
        {
            Destroy(child.gameObject);
        }

        int index = 0;
        foreach (Vector3 vert in vertices)
        {
            GameObject obj = GameObject.Instantiate(nodeTemplate, nodesParent);
            obj.transform.localPosition = vert;
            obj.name = string.Format("{0}", index);
            obj.SetActive(true);
            index++;
        }
    }
    #endregion
}