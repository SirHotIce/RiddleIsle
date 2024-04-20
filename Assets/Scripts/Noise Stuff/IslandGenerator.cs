using System.Collections.Generic;
using UnityEngine;

namespace Noise_Stuff
{
    public class IslandGenerator
    {
        private int _gridSize;
        private int _iterations;
        private int _islandSize;
        private float _blurStrength;
        private float _difficulty;

        public IslandGenerator(int gridSize, int iterations, float blurStrength, float difficulty)
        {
            this._gridSize = gridSize;
            this._difficulty = difficulty;
            this._iterations = iterations;
            int max = (int)(((225-150) * _difficulty)+150);
            int min = (int)(((150 - 50) * _difficulty) + 50);
            this._islandSize = Random.Range(min, max);
            this._blurStrength = blurStrength;
            
        }

        public List<float> GenerateIsland()
        {
            List<float> islandData = new List<float>();

            // Initialize grid with water (0.0f) and land (1.0f)
            for (int i = 0; i < _gridSize * _gridSize; i++)
            {
                islandData.Add(0.0f); // Start with water
            }

            // Calculate the center of the island
            int centerX = _gridSize / 2;
            int centerY = _gridSize / 2;

            // Adjust center based on island size
            int halfIslandSize = _islandSize / 2;
            centerX -= halfIslandSize;
            centerY -= halfIslandSize;

            // Randomly generate island shape within the specified bounds
            for (int y = 0; y < _islandSize; y++)
            {
                for (int x = 0; x < _islandSize; x++)
                {
                    float distanceToCenter = Mathf.Sqrt((x - halfIslandSize) * (x - halfIslandSize) + (y - halfIslandSize) * (y - halfIslandSize));
                    float perlinValue = Mathf.PerlinNoise((float)(x + Random.Range(-10, 10)) / _islandSize * 3f, (float)(y + Random.Range(-10, 10)) / _islandSize * 3f); // Use Perlin noise for varied shapes with additional randomness
                    float threshold = Mathf.Lerp(1f, 0f, distanceToCenter / (_islandSize / 2f)); // Gradually decrease land placement probability towards the edges
                    if (Random.value < perlinValue * threshold * Random.Range(0.7f, 1.0f)) // Introduce randomness in land placement
                    {
                        islandData[(x + centerX) + (y + centerY) * _gridSize] = 1.0f; // Set cell to land
                    }
                }
            }

            // Apply cellular automaton to refine island shape
            for (int i = 0; i < _iterations; i++)
            {
                islandData = CellularAutomaton(islandData);
            }

            // Smooth transition between land and water
            SmoothTransition(ref islandData);

            return islandData;
        }

        private List<float> CellularAutomaton(List<float> data)
        {
            List<float> newData = new List<float>();

            for (int y = 0; y < _gridSize; y++)
            {
                for (int x = 0; x < _gridSize; x++)
                {
                    int landNeighbours = CountLandNeighbours(data, x, y);

                    if (landNeighbours >= 4) // Adjust the threshold to maintain a solid island
                    {
                        newData.Add(1.0f); // Set cell to land
                    }
                    else
                    {
                        newData.Add(0.0f); // Set cell to water
                    }
                }
            }

            return newData;
        }

        private int CountLandNeighbours(List<float> data, int x, int y)
        {
            int count = 0;

            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i >= 0 && i < _gridSize && j >= 0 && j < _gridSize && !(i == x && j == y))
                    {
                        if (data[i + j * _gridSize] > 0.0f) // Check for land cell
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        private void SmoothTransition(ref List<float> islandData)
        {
            // Apply blur filter to smooth transition between land and water
            float[,] blurKernel = {
                {1f, 1f, 1f},
                {1f, 1f, 1f},
                {1f, 1f, 1f}
            };

            int kernelSize = 3;
            float weight = 1.0f / 9.0f; // Normalize the kernel
            weight *= _blurStrength; // Adjust the weight by the blur strength

            List<float> blurredData = new List<float>(islandData);

            for (int y = 1; y < _gridSize - 1; y++)
            {
                for (int x = 1; x < _gridSize - 1; x++)
                {
                    float newValue = 0;
                    for (int i = 0; i < kernelSize; i++)
                    {
                        for (int j = 0; j < kernelSize; j++)
                        {
                            newValue += blurKernel[i, j] * islandData[(x + i - 1) + (y + j - 1) * _gridSize];
                        }
                    }
                    blurredData[x + y * _gridSize] = newValue * weight;
                }
            }

            islandData = blurredData;
        }
    }
}
