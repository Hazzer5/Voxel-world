using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    //create a meshdata object based on a voxel map
    public static MeshData CreateMesh(Chunk chunk) {
        World world = chunk.GetWorld();
        WorldData worldSettings = world.worldSettings;
        MeshData meshData = new MeshData(worldSettings, world);

        for (int x = 0; x < worldSettings.chunkWidth; x++)
        {
            for (int y = 0; y < worldSettings.chunkHeight; y++)
            {
                for (int z = 0; z < worldSettings.chunkWidth; z++)
                {
                    Vector3 pos = new Vector3(x,y,z);
                    byte blockId = chunk.GetVoxel(new Vector3Int(x, y, z));
                    if(world.blockData[blockId].isSolid) {
                        for (int f = 0; f < 6; f++)
                        {
                            if(!chunk.CheckVoxel(pos + VoxelOffsets.faceChecks[f])) {
                                
                                meshData.AddFace(pos, f, blockId);
                            }
                        }
                    }
                }
            }
        }

        return meshData;
    }
}

public class MeshData {
    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> uvs;

    int vertexIndex;
    WorldData worldSettings;
    World world;

    public MeshData(WorldData worldSettings, World world) {
        this.worldSettings = worldSettings;
        this.world = world;

        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
        vertexIndex = 0;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        return mesh;
    }

    public void AddFace(Vector3 pos, int f, byte blockId) {
                //add vertice postion to vertice
                vertices.Add(pos + VoxelOffsets.voxelVerts[VoxelOffsets.voxelTries[f, 0]]); //blockpos + vertex offset
                vertices.Add(pos + VoxelOffsets.voxelVerts[VoxelOffsets.voxelTries[f, 1]]);
                vertices.Add(pos + VoxelOffsets.voxelVerts[VoxelOffsets.voxelTries[f, 2]]);
                vertices.Add(pos + VoxelOffsets.voxelVerts[VoxelOffsets.voxelTries[f, 3]]);
                
                //add vertice uvs
                AddTexture(world.blockData[blockId].GetTextureID(f));

                //add triangle indexs
                triangles.Add(vertexIndex);    //triangle 1
                triangles.Add(vertexIndex + 1);//triangle 1
                triangles.Add(vertexIndex + 2);//triangle 1

                triangles.Add(vertexIndex + 2);//triangle 2
                triangles.Add(vertexIndex + 1);//triangle 2
                triangles.Add(vertexIndex + 3);//triangle 2

                //increment vertex index (num vertices added to mesh)
                vertexIndex += 4;
    }

    void AddTexture (int textureId) {
        //convert the texture id to x and y coords of the texture map
        float y = textureId / worldSettings.textureAtlessSizeInBlocks;
        float x = textureId - (y * worldSettings.textureAtlessSizeInBlocks);

        // convert the int x, y to the value between 0 and 1 for the texture map
        x *= worldSettings.NormilizedBlockSize;
        y *= worldSettings.NormilizedBlockSize;

        // done to get the texture map start with 0 at the top left rather than bottom left
        y = 1f - y - worldSettings.NormilizedBlockSize; 
    
        //add the texture coordinates to the uvs.
        uvs.Add(new Vector2(x, y)); //bottom left pixel
        uvs.Add(new Vector2(x, y + worldSettings.NormilizedBlockSize)); //top left pixel
        uvs.Add(new Vector2(x + worldSettings.NormilizedBlockSize, y)); //bottom right pixel
        uvs.Add(new Vector2(x + worldSettings.NormilizedBlockSize, y + worldSettings.NormilizedBlockSize)); //top right pixel
    }
}
