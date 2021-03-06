﻿using System;
using TerrainQuest.Generator.Helpers;

namespace TerrainQuest.Generator.Blending
{
    /// <summary>
    /// A darkening <see cref="IBlendMode"/> where the lowest value is copies to the result
    /// </summary>
    public class DarkenBlend : IBlendMode
    {
        /// <summary>
        /// Perform the blending.
        /// </summary>
        public HeightMap Blend(HeightMap left, HeightMap right)
        {
            var result = left.Clone();

            result.ForEach((r, c) => {
                var a = result[r, c];
                var b = right.IsInRange(r, c) ? right[r, c] : 1d;
                result[r, c] = Math.Min(a, b);
            });

            return result;
        }
    }
}
