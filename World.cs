using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    [SerializeField] private GameObject chunk;
    [SerializeField] private int chunkSize = 16;

    [SerializeField] private int worldX = 16;
    [SerializeField] private int worldY = 16;
    [SerializeField] private int worldZ = 16;

    public byte[,,] worldData;
    private Chunk[,,] chunks;

    public int ChunkSize { get { return chunkSize; } }

    // Use this for initialization
    void Awake () {
        worldData = new byte[worldX, worldY, worldZ];

        for (int x = 0; x < worldX; x++) {
            for (int y = 0; y < worldY; y++) {
                for (int z = 0; z < worldZ; z++) {
                    if (y <= 4)
                        worldData[x, y, z] = (byte)Block.Rock.GetHashCode();
                }
            }
        }

        chunks = new Chunk[Mathf.FloorToInt(worldX / ChunkSize), Mathf.FloorToInt(worldY / ChunkSize), Mathf.FloorToInt(worldZ / ChunkSize)];

        for (int x = 0; x < ChunkSize; x++) {
            for (int y = 0; y < ChunkSize; y++) {
                for (int z = 0; z < ChunkSize; z++) {
                    GameObject newChunk = Instantiate(chunk, new Vector3(x * ChunkSize, y * ChunkSize, z * ChunkSize), new Quaternion(0, 0, 0, 0)) as GameObject;
                    chunks[x, y, z] = newChunk.GetComponent("Chunk") as Chunk;
                    chunks[x, y, z].WorldObject = gameObject;
                    chunks[x, y, z].ChunkSize = ChunkSize;
                    chunks[x, y, z].ChunkX = x * ChunkSize;
                    chunks[x, y, z].ChunkY = y * ChunkSize;
                    chunks[x, y, z].ChunkZ = z * ChunkSize;
                }
            }
        }
    }

    public byte GetBlockAt(int x, int y, int z) {
        if (x < 0 || x >= worldX || y < 0 || y >= worldY || z < 0 || z >= worldZ)
            return (byte) Block.Rock.GetHashCode();

        return worldData[x, y, z];
    }

    // Update is called once per frame
    void Update () {
		
	}
}
