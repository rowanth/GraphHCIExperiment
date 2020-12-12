using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UXF namespace
using UXF;

public class Node : MonoBehaviour
{
    public GraphManager graph;
    public int id = 0;
    public bool clickable = false;

    Animator anim;
    AudioSource audioSrc;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        anim = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();
    }

    public void LightUp()
    {
        anim.SetTrigger("LightUp");
        audioSrc.Play();
    }

    /// <summary>
    /// OnMouseDown is called when the user has pressed the mouse button while
    /// over the GUIElement or Collider.
    /// </summary>
    void OnMouseDown()
    {
    }
}