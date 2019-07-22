using UnityEngine;
using System.Threading;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    MeshFilter filter;

    public int worldX = 8;
    public int worldZ = 8;

    public int chunkX = 64;
    public int chunkZ = 64;

    public float baseNoise = 0.02f;
    public float baseNoiseHeight = 4;
    public int elevation = 15;
    public float frequency = 0.005f;

    Block [,,] grid;

    public int MaxJobs = 4;
    List<WorldGeneration> toDoJobs = new List<WorldGeneration>();
    List<WorldGeneration> currentJobs = new List<WorldGeneration>();

    void Start()
    {
        CreateWorld();
    }

    void CreateWorld() {
        chunkX = Mathf.Clamp(chunkX, 0, worldX);
        chunkZ = Mathf.Clamp(chunkZ, 0, worldZ);
        for (int x = 0; x < worldX; x++)
        {
            for (int z = 0; z < worldZ; z++)
            {
                Vector3 p = Vector3.zero;
                p.x = x * chunkX;
                p.z = z * chunkZ;
            }
        }
    }

    void Update()
    {
        int i = 0;
        while(i < currentJobs.Count) {
            if (currentJobs[i].jobDone)
            {
                currentJobs[i].NotifyComplete();
                currentJobs.RemoveAt(i);
            } else {
                i++;
            }
        }
        if (toDoJobs.Count > 0 && currentJobs.Count < MaxJobs)
        {
            WorldGeneration job = toDoJobs[0];
            toDoJobs.RemoveAt(0);
            currentJobs.Add(job);

            Thread jobThread = new Thread(job.StartCreatingWorld);
            jobThread.Start();
        }
    }

    public void LoadMeshData(Block[,,] createdGrid, MeshData data) {
        grid = createdGrid;
        filter = GetComponent<MeshFilter>();

        Mesh mesh = new Mesh() {
            vertices = data.vertices.ToArray(),
            uv = data.uv.ToArray(),
            triangles = data.triangles.ToArray()
        };
        mesh.RecalculateNormals();

        filter.mesh = mesh;
    }

    public Block GetBlock(int x, int y, int z) {
        if (x < 0 || y < 0 || z < 0 || x > chunkX - 1 || z > chunkZ - 1 || y > elevation -1)
        {
            return null;
        }
        return grid[x,y,z];
    }

    public void RequestWorldGeneration(Vector3 o) {

       WorldChunkStats s = new WorldChunkStats {
           maxX = chunkX,
           maxZ = chunkZ,
           baseNoise = baseNoise,
           baseNoiseHeight = baseNoiseHeight,
           elevation = elevation,
           frequency = frequency,
           origin = o
       };
        WorldGeneration wg = new WorldGeneration(s, LoadMeshData);
        toDoJobs.Add(wg);
    }
}