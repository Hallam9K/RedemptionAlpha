using System;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Effects.Trails.Tips;

namespace Redemption.Effects.Trails
{
    public class Ellipse
    {
        private const float DEFAULT_WIDTH = 16;
        
        private readonly Primitives _primitives;
        private readonly TrailWidthFunction _trailWidthFunction;
        private readonly TrailColorFunction _trailColorFunction;

        private Vector2 _anchorPosition;
        private Vector2[] _positions;

        private VertexPositionColorTexture[] _vertices;
        private short[] _indices;
        
        public Ellipse(Primitives primitives, TrailWidthFunction trailWidthFunction, TrailColorFunction trailColorFunction)
        {
            _primitives = primitives;
            _trailColorFunction = trailColorFunction;
            _trailWidthFunction = trailWidthFunction;
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

        public void SetPositions(Vector2[] positions, Vector2 anchorPosition)
        {
            ArgumentNullException.ThrowIfNull(positions);

            _positions = positions;
            _anchorPosition = anchorPosition;
            GenerateMesh(out _vertices, out _indices);
        }
        
        private void GenerateMesh(out VertexPositionColorTexture[] vertices, out short[] indices)
        {
            VertexPositionColorTexture[] verticesTemp = new VertexPositionColorTexture[_positions.Length * 2];
            short[] indicesTemp = new short[_positions.Length * 6 - 6];

            for (int k = 0; k < _positions.Length; k++)
            {
                float factorAlongTrail = (float)k / (_positions.Length - 1);
                // float width = _trailWidthFunction?.Invoke(factorAlongTrail) ?? DEFAULT_WIDTH; // is this intentionally not used?

                Vector2 a = _anchorPosition;
                Vector2 c = _positions[k];

                Vector2 texCoordA = new(factorAlongTrail, 0);
                Vector2 texCoordC = new(factorAlongTrail, 1);

                Color colorA = _trailColorFunction?.Invoke(texCoordA) ?? Color.White;
                Color colorC = _trailColorFunction?.Invoke(texCoordC) ?? Color.White;

                verticesTemp[k] = new VertexPositionColorTexture(a.Vec3(), colorA, texCoordA);
                verticesTemp[k + _positions.Length] = new VertexPositionColorTexture(c.Vec3(), colorC, texCoordC);
            }

            for (short k = 0; k < _positions.Length - 1; k++)
            {
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