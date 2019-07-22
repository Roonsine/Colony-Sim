
using System.Collections.Generic;
using UnityEngine;
using SimplexNoise;

public class WorldGeneration
{
    Block[,,] grid;

    WorldChunkStats s;
    MeshData meshData;
    public volatile bool jobDone;

    public delegate void WorldGenerationCallback(Block[,,] grid, MeshData d);

    WorldGenerationCallback finishedCallback;


    public WorldGeneration(WorldChunkStats stats, WorldGenerationCallback callback) {
        s = stats;
        finishedCallback = callback;
    }

    public void StartCreatingWorld() {



        meshData= CreateChunk();

        jobDone = true;
    }

    public void NotifyComplete() {
        finishedCallback(grid, meshData);
    }

     int GetNoise(int x, int y, int z, float scale, int max) {
        return Mathf.FloorToInt((Noise.Generate(x * scale, y * scale, z * scale) + 1) * (max / 2f));
    }

    MeshData CreateChunk () 
    {
        grid = new Block[s.maxX,s.elevation,s.maxZ];

        List<Block> blocks = new List<Block>();

        for (int x = 0; x < s.maxX; x++)
        {
            for (int z = 0; z < s.maxZ; z++)
            {
                Block b = new Block();
                b.x = x;
                b.z = z;

                b.isSolid = true;

                Vector3 targetPosition = s.origin;
                targetPosition.x += x * 1;
                targetPosition.z += z * 1;
                float height = 0;
                
                height += GetNoise(x, 0, z, s.frequency, s.elevation);

                targetPosition.y += height;
                b.worldPosition = targetPosition;
                b.y = Mathf.FloorToInt(height);

                grid[x,b.y,z] = b;
                blocks.Add(b);

            //    b.LoadBlock(d, this, targetPosition);
            }            
        }
        
        MeshData d = new MeshData();
        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].LoadBlock(d, this);
        }
        return d;
    }
        public Block GetBlock(int x, int y, int z) {
        if (x < 0 || y < 0 || z < 0 || x > s.maxX - 1 || z > s.maxZ - 1 || y > s.elevation -1)
        {
            return null;
        }
        return grid[x,y,z];
    }
}
