using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillChunkData : MonoBehaviour
{
    public static byte[,,] PopulateVoxelMap(WorldData worldSettings, Vector2 sampleCenter, NoiseDataGroup noiseData) {

        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(worldSettings.chunkWidth, worldSettings.chunkWidth, sampleCenter, noiseData.main);
        byte[,,] blocks = new byte[worldSettings.chunkWidth, worldSettings.chunkHeight, worldSettings.chunkWidth];
        NoiseGenerator layerBlur = new NoiseGenerator(noiseData.layerMix, sampleCenter);
        NoiseGenerator caveGenerator = new NoiseGenerator(noiseData.caves, sampleCenter);

        for (int x = 0; x < worldSettings.chunkWidth; x++)
        {
            for (int z = 0; z < worldSettings.chunkWidth; z++)
            {
                int gorundLevel = Mathf.FloorToInt(Mathf.Lerp(worldSettings.SeaLevel, worldSettings.chunkHeight, heightMap.values[x, z]));
                int dirtThickness = Mathf.FloorToInt(Mathf.Lerp(2, 7, layerBlur.GenerateNosieValue(x, z)));
                for (int y = 0; y < worldSettings.chunkHeight; y++)
                {
                    if(y==0) {
                        blocks[x,y,z] = 1; //bedrock
                    }else if (y > gorundLevel) {
                        blocks[x,y,z] = 0; //air
                    }else {
                        if (caveGenerator.GenerateNosieValue(x, y, z) > .55) {
                            blocks[x,y,z] = 0; //air
                        }else if(y < gorundLevel - dirtThickness) {
                            blocks[x,y,z] = 2; //stone
                        }else if (y == gorundLevel) {
                            blocks[x,y,z] = 3; //grass
                        }else {
                            blocks[x,y,z] = 4; //dirt
                        }
                    }
                        
                }
            }
        }




        return blocks;
    }
}
