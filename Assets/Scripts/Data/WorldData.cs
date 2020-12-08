using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WorldData : ScriptableObject
{
    //consts for the game

    public int chunkWidth = 5;
    public int chunkHeight = 5;
    public int worldSizeInChunks = 100;
    public int SeaLevel = 50;

    public int worldSizeInVoxels {
        get { return worldSizeInChunks * chunkWidth;}
    }

    public int textureAtlessSizeInBlocks = 4; //number of blocks seen in one row/column of the texture

    public int meshViewDistInChunks = 5;
    public int voxelDataViewDistInChunks = 6;
    public int colliderViewDistInChunks = 1;
    
    public float NormilizedBlockSize { //the float size used to go from one block to the next in the texture
        get {
            return 1f / (float) textureAtlessSizeInBlocks;
        }
    }
}
