using System;
using Microsoft.SPOT;

namespace IxDLib {
    /// <summary>
    /// Utility class for Arduino ports etc.
    /// v0.1 by Lars Toft Jacobsen, ITU, IxDLab
    /// CC-BY-SA
    /// </summary>
    public static class IdXUtility {

        /// <summary>
        /// Arduino constrain function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x">Value to constrain</param>
        /// <param name="a">Lower bound</param>
        /// <param name="b">Upper bound</param>
        /// <returns>Constrained value</returns>
        public static int Constrain(int x, int a, int b) {
            if (x < a) return a;
            if (x > b) return b;
            return x;
        }

        public static double Constrain(double x, double a, double b) {
            if (x < a) return a;
            if (x > b) return b;
            return x;
        }

        /// <summary>
        /// Arduino map function
        /// </summary>
        /// <param name="x">mapped value</param>
        /// <param name="in_min">in range min</param>
        /// <param name="in_max">in range max</param>
        /// <param name="out_min">out range min</param>
        /// <param name="out_max">out range max</param>
        /// <returns></returns>
        public static int RangeMap(int x, int in_min, int in_max, int out_min, int out_max) {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
            
    }
}
