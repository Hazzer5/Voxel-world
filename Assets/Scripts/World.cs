using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Transform player;
    [HideInInspector()]
    public Vector3 spawnPostion;

    public Material material;
    public BlockData[] blockData;
    public WorldData worldSettings;
    public NoiseDataGroup noiseDataBatch;

    Dictionary<Vector2Int, Chunk> chunks;
    List<Vector2Int> activeChunks = new List<Vector2Int>();

    Vector2Int playerLastCunkCoord;
    Vector2Int playerChunkCoord;

    public void Start() {
        chunks = new Dictionary<Vector2Int, Chunk>();
        spawnPostion = new Vector3(worldSettings.worldSizeInVoxels /2f, 100, worldSettings.worldSizeInVoxels /2f); //place the spwan postion in the middle of the world
        player.gameObject.SetActive(false);
        StartCoroutine(GenerateWorld());

        playerChunkCoord = GetChunkCoordFromVector3(player.position); //find the chunk that the player is in
        playerLastCunkCoord = playerChunkCoord; //assign the player last chunk coord to the same chunk as they are curently in
    }

    public void Update() {

        playerChunkCoord = GetChunkCoordFromVector3(player.position);//find the chunk that the player is in
        if(!playerChunkCoord.Equals(playerLastCunkCoord)) { // if the chunk postion has changed
            CheckViewDist(); //update chunks in view dist
            playerLastCunkCoord = playerChunkCoord; //assign last chunk coord to curent coord
        }
    }

    IEnumerator GenerateWorld() {
        for (int x = -worldSettings.voxelDataViewDistInChunks; x <= worldSettings.voxelDataViewDistInChunks; x++)
        {
            for (int z = -worldSettings.voxelDataViewDistInChunks; z <= worldSettings.voxelDataViewDistInChunks; z++)
            { //iterate thorugh every chunk in the view distance from spawn
                Chunk chunk = CreateNewChunk(new Vector2Int(worldSettings.worldSizeInChunks/2  + x, worldSettings.worldSizeInChunks/2  + z));
                
            }
        }

        yield return new WaitForSeconds(2);

        for (int x = -worldSettings.meshViewDistInChunks; x <= worldSettings.meshViewDistInChunks; x++)
        {
            for (int z = -worldSettings.meshViewDistInChunks; z <= worldSettings.meshViewDistInChunks; z++)
            { //iterate thorugh every chunk in the view distance from spawn
                chunks[new Vector2Int(worldSettings.worldSizeInChunks/2  + x, worldSettings.worldSizeInChunks/2  + z)].isActive = true;                
            }
        }

        yield return new WaitForSeconds(2);

        for (int x = -worldSettings.colliderViewDistInChunks; x <= worldSettings.colliderViewDistInChunks; x++)
        {
            for (int z = -worldSettings.colliderViewDistInChunks; z <= worldSettings.colliderViewDistInChunks; z++)
            { //iterate thorugh every chunk in the view distance from spawn
                chunks[new Vector2Int(worldSettings.worldSizeInChunks/2  + x, worldSettings.worldSizeInChunks/2  + z)].activeCollider = true;                
            }

        }

        player.position = spawnPostion; //place the player at spawn
        player.gameObject.SetActive(true);
    }

    Vector2Int GetChunkCoordFromVector3(Vector3 pos) {
        int x = Mathf.FloorToInt(pos.x / worldSettings.chunkWidth);
        int z = Mathf.FloorToInt(pos.z / worldSettings.chunkWidth);
        return new Vector2Int(x, z);
    }

    void CheckViewDist() {
        Vector2Int coord = GetChunkCoordFromVector3(player.position); // get the cunk pos that the player is curently in
        List<Vector2Int> previouslyActiveChunks = new List<Vector2Int>(activeChunks); 

        for (int x = -worldSettings.voxelDataViewDistInChunks; x <= worldSettings.voxelDataViewDistInChunks; x++)
        {
            for (int z = -worldSettings.voxelDataViewDistInChunks; z <= worldSettings.voxelDataViewDistInChunks; z++)
            {
                Vector2Int curentCoord = new Vector2Int(coord.x + x, coord.y + z);
                if (IsChunkInWorld(curentCoord)) {
                    if (!chunks.ContainsKey(curentCoord)) {
                        Chunk chunk = CreateNewChunk(curentCoord);
                        if(Mathf.Abs(x) < worldSettings.meshViewDistInChunks && Mathf.Abs(z) < worldSettings.meshViewDistInChunks) {
                            chunk.isActive = true;
                        }                        
                        
                    }else if (Mathf.Abs(x) < worldSettings.meshViewDistInChunks && Mathf.Abs(z) < worldSettings.meshViewDistInChunks) { 
                        if(!chunks[curentCoord].isActive) {
                            chunks[curentCoord].isActive = true;
                            activeChunks.Add(curentCoord);
                        }

                        if(Mathf.Abs(x) < worldSettings.colliderViewDistInChunks && Mathf.Abs(z) < worldSettings.colliderViewDistInChunks) {
                            chunks[curentCoord].activeCollider = true;
                        } else {
                            chunks[curentCoord].activeCollider = false;
                        }

                        //remove chunk from previously active list if it is there
                        for (int i = previouslyActiveChunks.Count - 1; i >=0; i--) //have to start at the end of the list to stop indexing problems after some have been removed
                        {
                            if(previouslyActiveChunks[i].Equals(curentCoord)) {
                                previouslyActiveChunks.RemoveAt(i);
                            }
                        }
                    }

                    
                }
            }
        }
        foreach (Vector2Int c in previouslyActiveChunks)
        {
            chunks[c].isActive = false;
        }
    }

    public byte GetVoxel(Vector3 pos) {
        if (!IsVoxelInWorld(pos)) {
            return 0;
        }

        Vector2Int chunkCoord = new Vector2Int(Mathf.FloorToInt(pos.x/worldSettings.chunkWidth), Mathf.FloorToInt(pos.z/worldSettings.chunkWidth));
        Vector3Int blockCoord = new Vector3Int(Mathf.FloorToInt(pos.x)%worldSettings.chunkWidth, Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z)%worldSettings.chunkWidth);
        if(chunks.ContainsKey(chunkCoord)) {
            if(chunks[chunkCoord].hasVoxelMap){
                return chunks[chunkCoord].GetVoxel(blockCoord);
            } else {
                return 0;
            }
        }else {
            return 0;
        }
    }

    Chunk CreateNewChunk (Vector2Int coord) {
        chunks.Add(coord, new Chunk(coord, this, worldSettings, noiseDataBatch.main));
        activeChunks.Add(coord);
        chunks[coord].load();
        return chunks[coord];
    }

    bool IsChunkInWorld(Vector2Int coord) {
        return (coord.x >= 0 && coord.x < worldSettings.worldSizeInChunks &&
                coord.y >= 0 && coord.y < worldSettings.worldSizeInChunks);
                
    }

    bool IsVoxelInWorld (Vector3 pos) {
        return (pos.x >= 0 && pos.x < worldSettings.worldSizeInVoxels &&
                pos.y >= 0 && pos.y < worldSettings.chunkHeight &&
                pos.z >= 0 && pos.z < worldSettings.worldSizeInVoxels);
    }
}
[System.Serializable]
public class BlockData {
    public string name;
    public bool isSolid;

    [Header("Texture Values")]
    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    // Face order is back, front, top, bottom, left, right

    public int GetTextureID (int faceIndex) {
        switch(faceIndex) {
            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3: 
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.Log("Error getting face Id");
                return backFaceTexture;
        }
    }
}

[System.Serializable]
public class NoiseDataGroup {
    public NoiseData main;
    public NoiseData layerMix;
    public NoiseData biome;
    public NoiseData caves;
}