using System;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Effects.Trails.Tips;
using Terraria;

namespace Redemption.Effects.Trails
{
    public class DanTrail
    {
        private const float DEFAULT_WIDTH = 16;

        private readonly Primitives _primitives;
        private readonly ITrailTip _tip;
        private readonly TrailWidthFunction _trailWidthFunction;
        private readonly TrailColorFunction _trailColorFunction;
        
        private Vector2 _nextPosition;
        private Vector2[] _positions;

        private VertexPositionColorTexture[] _vertices;
        private short[] _indices;
        
        public DanTrail(Primitives primitives, ITrailTip tip, TrailWidthFunction trailWidthFunction, TrailColorFunction trailColorFunction)
        {
            _primitives = primitives;
            _tip = tip ?? new NoTip();
            _trailWidthFunction = trailWidthFunction;
            _trailColorFunction = trailColorFunction;

            /* A---B---C
             * |  /|  /|
             * D / E / F
             * |/  |/  |
             * G---H---I
             * 
             * Let D, E, F, etc. be the set of n points that define the trail.
             * Since each point generates 2 vertices, there are 2n vertices, plus the tip's count.
             * 
             * As for indices - in the region between 2 defining points there are 2 triangles.
             * The amount of regions in the whole trail are given by n - 1, so there are 2(n - 1) triangles for n points.
             * Finally, since each triangle is defined by 3 indices, there are 6(n - 1) indices, plus the tip's count.
             */
        }

        public void Render(Effect effect)
        {
            if (_positions == null || _primitives == null)
            {
                return;
            }
            
            _primitives.SetVertices(_vertices);
            _primitives.SetIndices(_indices);
            _primitives.Render(effect, _vertices.Length, _indices.Length / 3);
        }
        
        public void SetPositions(Vector2[] positions, Vector2 nextPosition)
        {
            ArgumentNullException.ThrowIfNull(positions);

            _positions = positions;
            _nextPosition = nextPosition;
            
            Vector2 toNext = (_nextPosition - _positions[^1]).SafeNormalize(Vector2.Zero);
            GenerateMesh(out _vertices, out _indices);
            _tip.GenerateMesh(_positions[^1], toNext, _vertices.Length, out VertexPositionColorTexture[] tipVertices, out short[] tipIndices, _trailWidthFunction, _trailColorFunction);

            _vertices = _vertices.FastUnion(tipVertices);
            _indices = _indices.FastUnion(tipIndices);
        }
        
        private void GenerateMesh(out VertexPositionColorTexture[] vertices, out short[] indices)
        {
            VertexPositionColorTexture[] verticesTemp = new VertexPositionColorTexture[_positions.Length * 2];
            short[] indicesTemp = new short[_positions.Length * 6 - 6];

            // k = 0 indicates starting at the end of the trail (furthest from the origin of it).
            for (int k = 0; k < _positions.Length; k++)
            {
                // 1 at k = Positions.Length - 1 (start) and 0 at k = 0 (end).
                float factorAlongTrail = (float)k / (_positions.Length - 1);

                // Uses the trail width function to decide the width of the trail at this point (if no function, use 
                float width = _trailWidthFunction?.Invoke(factorAlongTrail) ?? DEFAULT_WIDTH;

                Vector2 current = _positions[k];
                Vector2 next = (k == _positions.Length - 1 ? _positions[^1] + (_positions[^1] - _positions[^2]) : _positions[k + 1]);

                Vector2 normalToNext = (next - current).SafeNormalize(Vector2.Zero);
                Vector2 normalPerp = normalToNext.RotatedBy(MathHelper.PiOver2);

                /* A
                 * |
                 * B---D
                 * |
                 * C
                 * 
                 * Let B be the current point and D be the next one.
                 * A and C are calculated based on the perpendicular vector to the normal from B to D, scaled by the desired width calculated earlier.
                 */

                Vector2 a = current + (normalPerp * width);
                Vector2 c = current - (normalPerp * width);

                /* Texture coordinates are calculated such that the top-left is (0, 0) and the bottom-right is (1, 1).
                 * To achieve this, we consider the Y-coordinate of A to be 0 and that of C to be 1, while the X-coordinate is just the factor along the trail.
                 * This results in the point last in the trail having an X-coordinate of 0, and the first one having a Y-coordinate of 1.
                 */
                Vector2 texCoordA = new(factorAlongTrail, 0);
                Vector2 texCoordC = new(factorAlongTrail, 1);

                // Calculates the color for each vertex based on its texture coordinates. This acts like a very simple shader (for more complex effects you can use the actual shader).
                Color colorA = _trailColorFunction?.Invoke(texCoordA) ?? Color.White;
                Color colorC = _trailColorFunction?.Invoke(texCoordC) ?? Color.White;

                /* 0---1---2
                 * |  /|  /|
                 * A / B / C
                 * |/  |/  |
                 * 3---4---5
                 * 
                 * Assuming we want vertices to be indexed in this format, where A, B, C, etc. are defining points and numbers are indices of mesh points:
                 * For a given point that is k positions along the chain, we want to find its indices.
                 * These indices are given by k for the above point and k + n for the below point.
                 */

                verticesTemp[k] = new VertexPositionColorTexture(a.Vec3(), colorA, texCoordA);
                verticesTemp[k + _positions.Length] = new VertexPositionColorTexture(c.Vec3(), colorC, texCoordC);
            }

            /* Now, we have to loop through the indices to generate triangles.
             * Looping to maxPointCount - 1 brings us halfway to the end; it covers the top row (excluding the last point on the top row).
             */
            for (short k = 0; k < _positions.Length - 1; k++)
            {
                /* 0---1
                 * |  /|
                 * A / B
                 * |/  |
                 * 2---3
                 * 
                 * This illustration is the most basic set of points (where n = 2).
                 * In this, we want to make triangles (2, 3, 1) and (1, 0, 2).
                 * Generalising this, if we consider A to be k = 0 and B to be k = 1, then the indices we want are going to be (k + n, k + n + 1, k + 1) and (k + 1, k, k + n)
                 */

                indicesTemp[k * 6] = (short)(k + _positions.Length);
                indicesTemp[k * 6 + 1] = (short)(k + _positions.Length + 1);
                indicesTemp[k * 6 + 2] = (short)(k + 1);
                indicesTemp[k * 6 + 3] = (short)(k + 1);
                indicesTemp[k * 6 + 4] = k;
                indicesTemp[k * 6 + 5] = (short)(k + _positions.Length);
            }
            
            vertices = verticesTemp;
            indices = indicesTemp;
        }
    }
}
