using System.Collections.Generic;

namespace Noise_Stuff
{
    public class MergeAlgorithms
    {
        private List<float> _perlin, _cellular;

        public MergeAlgorithms(List<float> perlin, List<float> cellular)
        {
            _perlin = perlin;
            _cellular = cellular;
        }

        public List<float> GetFinal()
        {
            int finalListSize = _perlin.Count;
            List<float> final = new List<float>();
            for (int i = 0; i < finalListSize; i++)
            {
                float val = (_perlin[i] * 0.8f) * _cellular[i];
                final.Add(val);
            }

            return final;
        }
    }
}