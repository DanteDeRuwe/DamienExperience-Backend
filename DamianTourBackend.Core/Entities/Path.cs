using System.Collections.Generic;

namespace DamianTourBackend.Core.Entities
{
    public class Path
    {
        public string LineColor { get; set; }
        public List<double[]> Coordinates { get; set; }

        public Path()
        {
            Coordinates = new List<double[]>();
        }
    }
}