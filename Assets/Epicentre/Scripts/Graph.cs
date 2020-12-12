using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    public class Node
    {
        public Vector3 position;
        public float size;
        public Vector3 colour;
    }

    public class Edge
    {
        public int node1Index;
        public int node2Index;
    }

    List<Node> nodes;
    List<Edge> edges;
}