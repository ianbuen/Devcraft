using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum Block { Air, Grass, Rock }

public class Chunk : MonoBehaviour {

    private enum Side { Top, North, East, West, South, Bottom }

    private float textureSize = 0.083f;

    // Grass
    private Vector2 grassTop = new Vector2(1, 11);
    private Vector2 grassSide = new Vector2(0, 10);

    // Rock
    private Vector2 rock = new Vector2(7, 8);

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> UVs = new List<Vector2>();

    private Mesh mesh;
    private MeshCollider meshCollider;

    public int ChunkSize { get; set; }
    public int ChunkX { get; set; }
    public int ChunkY { get; set; }
    public int ChunkZ { get; set; }
    public GameObject WorldObject { get; set; }
    private World world;

    private int faceCount;

    void Awake() {

    }

    // Use this for initialization
    void Start () {
        world = WorldObject.GetComponent("World") as World;
        ChunkSize = world.ChunkSize;

        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();

        GenerateMesh();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void GenerateMesh() {
        for (int x = 0; x < ChunkSize; x++) {
            for (int y = 0; y < ChunkSize; y++) {
                for (int z = 0; z < ChunkSize; z++) {
                    if (GetBlockAt(x, y, z) != (byte) Block.Air.GetHashCode()) {
                        // if block above is AIR
                        if (GetBlockAt(x, y + 1, z) == (byte)Block.Air.GetHashCode())
                            CubeFace(x, y, z, GetBlockAt(x, y, z), Side.Top);

                        // if block below is AIR
                        if (GetBlockAt(x, y - 1, z) == (byte)Block.Air.GetHashCode())
                            CubeFace(x, y, z, GetBlockAt(x, y, z), Side.Bottom);

                        // if block north is AIR
                        if (GetBlockAt(x, y, z + 1) == (byte)Block.Air.GetHashCode())
                            CubeFace(x, y, z, GetBlockAt(x, y, z), Side.North);

                        // if block south is AIR
                        if (GetBlockAt(x, y, z - 1) == (byte)Block.Air.GetHashCode())
                            CubeFace(x, y, z, GetBlockAt(x, y, z), Side.South);

                        // if block east is AIR
                        if (GetBlockAt(x + 1, y, z) == (byte)Block.Air.GetHashCode())
                            CubeFace(x, y, z, GetBlockAt(x, y, z), Side.East);

                        // if block west is AIR
                        if (GetBlockAt(x - 1, y, z) == (byte)Block.Air.GetHashCode())
                            CubeFace(x, y, z, GetBlockAt(x, y, z), Side.West);
                    }
                }
            }
        }

        UpdateMesh();
    }

    byte GetBlockAt(int x, int y, int z) {
        return world.GetBlockAt(x + ChunkX, y + ChunkY, z + ChunkZ);
    }

    void UpdateMesh() {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = UVs.ToArray();
        mesh.RecalculateNormals();

        //meshCollider.sharedMesh = null;
        //meshCollider.sharedMesh = mesh;

        vertices.Clear();
        UVs.Clear();
        triangles.Clear();
        faceCount = 0;
    }

    void CubeFace(int x, int y, int z, byte blockType, Side side) {
        // IMPORTANT NOTE: ARRANGEMENT OF VERTICES IN TRIANGLES MUST BE IN CLOCKWISE WINDING ORDER.
        // MESH WON'T SHOW OTHERWISE

        byte block = blockType;

        switch (side) {
            case Side.Top:
                // point out the four vertices of the Top of the cube
                vertices.Add(new Vector3(x, y, z + 1)); // V0
                vertices.Add(new Vector3(x + 1, y, z + 1)); // V1
                vertices.Add(new Vector3(x + 1, y, z)); // V2
                vertices.Add(new Vector3(x, y, z)); // V3
                break;
            case Side.North:
                // point out the four vertices of the North of the cube
                vertices.Add(new Vector3(x + 1, y - 1, z + 1)); // V0
                vertices.Add(new Vector3(x + 1, y, z + 1)); // V1
                vertices.Add(new Vector3(x, y, z + 1)); // V2
                vertices.Add(new Vector3(x, y - 1, z + 1)); // V3
                break;
            case Side.East:
                // point out the four vertices of the East of the cube
                vertices.Add(new Vector3(x, y - 1, z + 1)); // V0
                vertices.Add(new Vector3(x, y, z + 1)); // V1
                vertices.Add(new Vector3(x, y, z)); // V2
                vertices.Add(new Vector3(x, y - 1, z)); // V3
                break;
            case Side.West:
                // point out the four vertices of the West of the cube
                vertices.Add(new Vector3(x + 1, y - 1, z)); // V0
                vertices.Add(new Vector3(x + 1, y, z)); // V1
                vertices.Add(new Vector3(x + 1, y, z + 1)); // V2
                vertices.Add(new Vector3(x + 1, y - 1, z + 1)); // V3
                break;
            case Side.South:
                // point out the four vertices of the South of the cube
                vertices.Add(new Vector3(x, y - 1, z)); // V0
                vertices.Add(new Vector3(x, y, z)); // V1
                vertices.Add(new Vector3(x + 1, y, z)); // V2
                vertices.Add(new Vector3(x + 1, y - 1, z)); // V3
                break;
            case Side.Bottom:
                // point out the four vertices of the Bottom of the cube
                vertices.Add(new Vector3(x, y - 1, z)); // V0
                vertices.Add(new Vector3(x + 1, y - 1, z)); // V1
                vertices.Add(new Vector3(x + 1, y - 1, z + 1)); // V2
                vertices.Add(new Vector3(x, y - 1, z + 1)); // V3
                break;

        }

        // form two triangles for the square
        // triangle 1: vertices 0-1-2
        triangles.Add(faceCount * 4); // V0
        triangles.Add(faceCount * 4 + 1); // V1
        triangles.Add(faceCount * 4 + 2); // V2
        // triangle 2: vertices 0-2-3
        triangles.Add(faceCount * 4); // V0
        triangles.Add(faceCount * 4 + 2); // V2
        triangles.Add(faceCount * 4 + 3); // V3

        Vector2 t_pos = rock; // TEST

        // get the texture map positions
        UVs.Add(new Vector2(textureSize * t_pos.x, textureSize * t_pos.y));
        UVs.Add(new Vector2(textureSize * t_pos.x + textureSize, textureSize * t_pos.y));
        UVs.Add(new Vector2(textureSize * t_pos.x, textureSize * t_pos.y + textureSize));
        UVs.Add(new Vector2(textureSize * t_pos.x + textureSize, textureSize * t_pos.y + textureSize));

        faceCount++;
    }
}
