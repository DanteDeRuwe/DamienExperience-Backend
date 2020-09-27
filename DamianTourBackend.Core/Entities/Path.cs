using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Core.Entities {
    public class Path {
        public ICollection<CoordinateTuple> Coordinates { get; set; }

        public Path() {
            Coordinates = new List<CoordinateTuple>();
        }


    }
}
