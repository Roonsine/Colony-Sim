using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public int x;
    public int y;
    public int z;

    public Vector3 worldPosition;
    public bool isSolid;

    public virtual void LoadBlock(MeshData d, WorldGeneration w)
    {
        Vector3 o = worldPosition;

        MeshUtilities.FaceUp(d, o);

        Block n = w.GetBlock(x, y, z + 1);
        if (n == null || !n.isSolid)
        {
            MeshUtilities.FaceNorth(d, o);
        }

        Block s = w.GetBlock(x, y, z - 1);
        if (s == null || !s.isSolid)
        {
            MeshUtilities.FaceSouth(d, o);
        }

        Block we = w.GetBlock(x - 1, y, z);
        if (we == null || !we.isSolid)
        {
            MeshUtilities.FaceWest(d, o);
        }

        Block e = w.GetBlock(x + 1, y, z);
        if (e == null || !e.isSolid)
        {
            MeshUtilities.FaceEast(d, o);
        }
        
    }

}
