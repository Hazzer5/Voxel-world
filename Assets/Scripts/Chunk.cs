using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{

    GameObject chunkObject; //what will be seen in the game
    MeshRenderer meshRenderer; // mesh renderer of the chunk object
    MeshFilter meshFilter; // mesh filter of the chunk object
    MeshCollider collider;

    Mesh mesh;

    HeightMap heightMap;
    public bool hasVoxelMap;
    bool needsMesh;
    bool hasMesh;

    byte[,,] voxelMap; // a map of blocks in chunk using their block IDs

    World world; 
    Vector2Int coord;


    Vector3 postion;
    bool hasColliderMesh;

    // Start is called before the first frame update
    public Chunk(Vector2Int coord, World world, WorldData worldSettings, NoiseData noiseData) {
        this.world = world; // assign the world object
        this.coord = coord; // assign the coord object

        voxelMap = new byte[worldSettings.chunkWidth, worldSettings.chunkHeight, worldSettings.chunkWidth];

        chunkObject = new GameObject(); // create the chunk game object (where the mesh will be rendered)
        meshFilter = chunkObject.AddComponent<MeshFilter>(); // give chunk object a mesh filter
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();// give chunk object a mesh renderer
        collider = chunkObject.AddComponent<MeshCollider>(); // gives the chunk object a collider

        meshRenderer.material = world.material; // assign the world material as the mesh renderer materila
        chunkObject.transform.SetParent(world.transform); // set the game object parent to the world
        postion = new Vector3(coord.x * worldSettings.chunkWidth, 0, coord.y * worldSettings.chunkWidth); //postion gameobject based on coord
        chunkObject.transform.position = postion;
        chunkObject.name = "Chunk " + coord.x + ", " + coord.y; //rename gameobject based on coords
        hasVoxelMap = false;
        hasMesh = false;
        needsMesh = false;
        isActive = false;
        hasColliderMesh = false;
        collider.enabled = false;

    }
    
    //called to start the data loading / generating
    public void load() {
        Vector2 sampleCenter = new Vector2(postion.x, postion.z);
        ThreadedDataRequester.RequestData(() => FillChunkData.PopulateVoxelMap(world.worldSettings, sampleCenter, world.noiseDataBatch), 
        OnReciveVoxelMap);
    }


    //called once it creates the voxel map, will start creating the mesh
    void OnReciveVoxelMap(object voxelMapObject) {
        voxelMap = (byte[,,])voxelMapObject;
        hasVoxelMap = true;

        if (needsMesh) {
            RequestMesh();
        }
    }

    void RequestMesh() {
        ThreadedDataRequester.RequestData(() =>MeshGenerator.CreateMesh(this), OnReciveMeshData);
    }

    //called once mesh data is recived, assigns the mesh to the object
    void OnReciveMeshData(object meshDataObject) {
        mesh = ((MeshData)meshDataObject).CreateMesh();
        meshFilter.mesh = mesh;
        hasMesh = true;
    }

    //checks if the coordinates fall within the chunk
    bool InChunk(int x, int y, int z) {

        return !(x < 0 || x >= world.worldSettings.chunkWidth ||
                y < 0 || y >= world.worldSettings.chunkHeight ||
                z < 0 || z >= world.worldSettings.chunkWidth);
    }



    // check if the given pos is a solid block
    public bool CheckVoxel(Vector3 pos) {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!InChunk(x, y, z)) { // check if in chunk
                return world.blockData[world.GetVoxel(pos + postion)].isSolid;
        }
        
        return world.blockData[voxelMap[x,y,z]].isSolid; //return isSolid
    }

    //checks if the chunk is acitve in the scene
    public bool isActive {
        get { return chunkObject.activeSelf;}
        set { 
                chunkObject.SetActive(value);
                if (!hasMesh && value) {
                    needsMesh = true;
                    if(hasVoxelMap) {
                        RequestMesh();
                    }
                }
            }
    }

    //get the block id of the voxel at pos coordinates
    public byte GetVoxel(Vector3Int pos) {
        return voxelMap[pos.x, pos.y, pos.z];
    }

    //get the world object this is assigned to
    public World GetWorld() {
        return world;
    }

    public bool activeCollider {
        set {
            if(hasColliderMesh) {
                collider.enabled = value;
            }else if(value) {
                collider.sharedMesh = mesh;
                collider.enabled = value;
            }
        }
        get {return collider.enabled;}
    }
}
