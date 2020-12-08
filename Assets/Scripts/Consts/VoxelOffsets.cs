using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelOffsets
{
    public static readonly Vector3[] voxelVerts = new Vector3[8] {
        new Vector3(0.0f, 0.0f, 0.0f), // back bottom left
        new Vector3(1.0f, 0.0f, 0.0f), // back bottom right
        new Vector3(1.0f, 1.0f, 0.0f), // back top right
        new Vector3(0.0f, 1.0f, 0.0f), // back top left
        new Vector3(0.0f, 0.0f, 1.0f), // front bottom left
        new Vector3(1.0f, 0.0f, 1.0f), // front bottom right
        new Vector3(1.0f, 1.0f, 1.0f), // front top right
        new Vector3(0.0f, 1.0f, 1.0f) // front top left
    };

    // Face order is back, front, top, bottom, left, right
    //where to go to check face if clear
    public static readonly Vector3[] faceChecks = new Vector3[6] {
        new Vector3(0.0f, 0.0f, -1.0f),//back face
        new Vector3(0.0f, 0.0f, 1.0f), //front face
        new Vector3(0.0f, 1.0f, 0.0f), // top face
        new Vector3(0.0f, -1.0f, 0.0f),//bottom face
        new Vector3(-1.0f, 0.0f, 0.0f),//left face
        new Vector3(1.0f, 0.0f, 0.0f)  // right face
    };
 
    // Face order is back, front, top, bottom, left, right
    //the vertices that make up that face  -- need duplicates between faces for corect normal calculation
    public static readonly int[,] voxelTries = new int[6,4] {
        {0, 3, 1, 2}, //back face
        {5, 6, 4, 7}, //front face
        {3, 7, 2, 6}, // top face
        {1, 5, 0, 4}, //bottom face
        {4, 7, 0, 3}, //left face
        {1, 2, 5, 6} // right face
    };
}
