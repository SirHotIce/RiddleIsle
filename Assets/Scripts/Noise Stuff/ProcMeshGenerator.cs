using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using noiseLib = Noise_Stuff.PerlinNoiseGen;
using cellLib = Noise_Stuff.IslandGenerator;
using mergeLib = Noise_Stuff.MergeAlgorithms;

namespace Noise_Stuff
{
    public class ProcMeshGenerator : MonoBehaviour
    {
        [SerializeField] private int overallGridSize;
        [Header("For Perlin Noise")] 
        [SerializeField] [Range(0, 1)] private float baseWeight;
        [SerializeField] [Range(0, 1)] private float midWeight;
        [SerializeField] [Range(0, 1)] private float topWeight;
        [SerializeField] [Range(0.1f, 1)] private float perlinBlurStrength;
        [Header("For Cellular Automata")] 
        [SerializeField] private int iterations;
        [SerializeField] [Range(0.1f, 1)] private float blurStrength;
        [Header("For Mesh")] 
        [SerializeField] private float maximumHeight;
        [SerializeField] private float minimumHeight;

        // private void Start()
        // {
        //     
        //
        //     StartCoroutine(Gen());
        //    print("Generated");
        //    
        // }

        public IEnumerator Gen(float diffScale)
        {
            var noiseGen = new noiseLib();
            var isleGen = new cellLib(overallGridSize, iterations, blurStrength, diffScale);
            var noise = noiseGen.GenerateNoise(overallGridSize, baseWeight, midWeight, topWeight, perlinBlurStrength);
            var isle = isleGen.GenerateIsland();
            var mergeLib = new mergeLib(noise, isle);
            var merged = mergeLib.GetFinal();
            print("Generating...");
            yield return new WaitUntil( ()=>GenerateMeshFromValues(overallGridSize, merged, maximumHeight, minimumHeight));
        }
        private bool GenerateMeshFromValues(int gridSize, List<float> values, float maxHeight, float minHeight)
        {
            
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            Mesh mesh = new Mesh();

            // Vertices
            Stack<float> valuesStack = new Stack<float>(values);
            List<Vector3> verticesToMake = new List<Vector3>();
            
            float stepSize = 1.0f; // Adjust this value as needed
            for (float x = -gridSize / 2f; x < gridSize / 2f; x += stepSize)
            {
                for (float z = -gridSize / 2f; z < gridSize / 2f; z += stepSize)
                {
                    verticesToMake.Add(new Vector3(x, 0, z));
                    //print(((((gridSize/2f)+x)*gridSize/2)+((gridSize/2f)+z))+$" {(gridSize/2f)+x},{z + (gridSize/2f)}");
                }
            }

            mesh.vertices = verticesToMake.ToArray();

            // Triangles
            int width = Mathf.FloorToInt(Mathf.Sqrt(verticesToMake.Count));
            int length = Mathf.FloorToInt(verticesToMake.Count / width);

            List<int> triangles = new List<int>();
            for (int z = 0; z < length - 1; z++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    int currentIndex = x + z * width;

                    triangles.Add(currentIndex);
                    triangles.Add(currentIndex + 1);
                    triangles.Add(currentIndex + width + 1);

                    triangles.Add(currentIndex);
                    triangles.Add(currentIndex + width + 1);
                    triangles.Add(currentIndex + width);
                }
            }

            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshFilter.mesh = mesh;
            
            
            // GeneratePlane(gridSize);
            
            
            Vector3[] vertices = mesh.vertices;
            
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].y += minHeight + (values[i] * (maxHeight - minHeight));
            }
            
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.Optimize();
            mesh.OptimizeIndexBuffers();

            GetComponent<MeshCollider>().sharedMesh = mesh;
            return true;
        }

        // private void GeneratePlane(int gridSize)
        // {
        //     MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        //     Mesh mesh = new Mesh();
        //
        //     // Vertices
        //     List<Vector3> verticesToMake = new List<Vector3>();
        //     float stepSize = 1.0f; // Adjust this value as needed
        //     for (float x = -gridSize / 2f; x < gridSize / 2f; x += stepSize)
        //     {
        //         for (float z = -gridSize / 2f; z < gridSize / 2f; z += stepSize)
        //         {
        //             verticesToMake.Add(new Vector3(x, 0f, z));
        //         }
        //     }
        //
        //     mesh.vertices = verticesToMake.ToArray();
        //
        //     // Triangles
        //     int width = Mathf.FloorToInt(Mathf.Sqrt(verticesToMake.Count));
        //     int length = Mathf.FloorToInt(verticesToMake.Count / width);
        //
        //     List<int> triangles = new List<int>();
        //     for (int z = 0; z < length - 1; z++)
        //     {
        //         for (int x = 0; x < width - 1; x++)
        //         {
        //             int currentIndex = x + z * width;
        //
        //             triangles.Add(currentIndex);
        //             triangles.Add(currentIndex + 1);
        //             triangles.Add(currentIndex + width + 1);
        //
        //             triangles.Add(currentIndex);
        //             triangles.Add(currentIndex + width + 1);
        //             triangles.Add(currentIndex + width);
        //         }
        //     }
        //
        //     mesh.triangles = triangles.ToArray();
        //
        //     mesh.RecalculateNormals();
        //     mesh.RecalculateBounds();
        //
        //     meshFilter.mesh = mesh;
        // }


    }
}
