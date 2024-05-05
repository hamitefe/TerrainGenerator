using System.Collections.Generic;
using UnityEngine;
using System;
public class PerlinTerrainGenerator : MonoBehaviour{
    
    private MeshFilter filter;

    [Range(10, 500)]
    public int width, height;
    [Range(0.1f, 10)]
    public float cellSize;
    public float speed;
    public float detail = 8;
    public float heightMultiplier = 10;
    public bool animate = true;

    private void Awake() {
        filter = GetComponent<MeshFilter>();
        InitializeFlatTerrain();
    }
    private void InitializeFlatTerrain() {
        List<Vector3> vertices = new();
        List<Vector2> uvs = new();
        List<int> indices = new();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Vector3 pos = new Vector3(
                    x * cellSize,
                    0,
                    y * cellSize
                );
                vertices.Add(pos);
                uvs.Add(new Vector2(0, 0));
            }
        }

        for (int x = 0; x < width-1; x++) {
            for (int y = 0; y < height-1; y++) {
                /*
                 *we use width-1 and height-1 because its the begining of the 
                 *triangle it will be out of bounds if we dont use -1
                 */
                indices.Add(To1DIndex(x, y));
                indices.Add(To1DIndex(x + 1, y));
                indices.Add(To1DIndex(x, y+1));

                indices.Add(To1DIndex(x, y+1));
                indices.Add(To1DIndex(x+1, y));
                indices.Add(To1DIndex(x+1, y + 1));
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.name = "Perlin Terrain";
        filter.mesh = mesh;
    }

    private void LateUpdate() {
        AssignHeights();
    }

    private void AssignHeights() {
        /*
         * use ".sharedMesh" instead of ".mesh" because mesh returns a copy
         * and we are running this function in every late update
         */
        Vector3[] vertices = filter.sharedMesh.vertices;
        Vector2[] uvs = filter.sharedMesh.uv;
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int index = To1DIndex(x,y);
                float animationTimelapse = Time.time * Convert.ToInt16(animate) * speed;
                float noise = Mathf.PerlinNoise(
                    (x +animationTimelapse ) / detail,
                    (y +animationTimelapse) / detail
                );

                
                vertices[index].y = noise * heightMultiplier;
                uvs[index] = new Vector2(noise, 0);
                /*
                 * the position of our color at the texture
                 * 0.0 being first pixel of the texture and
                 * 1.0 being the last pixel of the texture
                 */
                
            }
        }
        filter.sharedMesh.vertices = vertices;
        filter.sharedMesh.uv = uvs;
        filter.sharedMesh.RecalculateBounds();
    }

    private int To1DIndex(int x, int y) {
        return x + (y * width);
    }
}
