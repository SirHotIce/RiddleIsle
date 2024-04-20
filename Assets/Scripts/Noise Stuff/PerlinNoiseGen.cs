using System.Collections.Generic;
using UnityEngine;

namespace Noise_Stuff
{
    public class PerlinNoiseGen
    {
        private float baseScale;
        private float midScale;
        private float topScale;
        public PerlinNoiseGen()
        {
            baseScale = Random.Range(20, 50);
            midScale = Random.Range(10, 30);
            topScale = Random.Range(1, 10);
        }

        public List<float> GenerateNoise(int gridSize, float baseWeight, float midWeight, float topWeight, float blurStrength)
        {
            var baseLayer = BlurNoise(getNoiseArray(gridSize, baseScale), blurStrength);
            var midLayer = BlurNoise(getNoiseArray(gridSize, midScale), blurStrength);
            var topLayer = BlurNoise(getNoiseArray(gridSize, topScale), blurStrength);

            List<float> finalLayer = new List<float>();

            int finalListSize = gridSize * gridSize;
            for (int i = 0; i < finalListSize; i++)
            {
                float val = (baseLayer[i]*baseWeight) + (midLayer[i]*midWeight) + (topLayer[i]*topWeight);
                finalLayer.Add(val);
            }

            return finalLayer;
        }

        private List<float> BlurNoise(List<float> noiseMap, float blurStrength)
        {
            int gridSize = (int)Mathf.Sqrt(noiseMap.Count);
            float[,] blurKernel = {
                {1f, 1f, 1f},
                {1f, 1f, 1f},
                {1f, 1f, 1f}
            };
            int kernelSize = 3;
            float weight = 1.0f / 9.0f; // Normalize the kernel
            weight *= blurStrength; // Adjust the weight by the blur strength

            List<float> blurredMap = new List<float>(noiseMap);

            for (int y = 1; y < gridSize - 1; y++)
            {
                for (int x = 1; x < gridSize - 1; x++)
                {
                    float newValue = 0;
                    for (int i = 0; i < kernelSize; i++)
                    {
                        for (int j = 0; j < kernelSize; j++)
                        {
                            newValue += blurKernel[i, j] * noiseMap[(x + i - 1) + (y + j - 1) * gridSize];
                        }
                    }
                    blurredMap[x + y * gridSize] = newValue * weight;
                }
            }

            return blurredMap;
        }

        private List<float> getNoiseArray(int gridSize, float scale)
        {
            List<float> noiseMap = new List<float>();

            noiseMap.Clear();

            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    float xCoord = (float)x / gridSize * scale;
                    float yCoord = (float)y / gridSize * scale;

                    float perlinValue = Mathf.PerlinNoise(xCoord, yCoord);
                    noiseMap.Add(perlinValue);
                }
            }

            return noiseMap;
        }
    }
}
    