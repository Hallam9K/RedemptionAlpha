using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Redemption.Effects.Trails
{
    public class Primitives : IDisposable
    {
        private const int RESIZE_MULTIPLIER = 2;
        private readonly GraphicsDevice _graphicsDevice;

        private DynamicVertexBuffer _vertexBuffer;
        private DynamicIndexBuffer _indexBuffer;
        
        public Primitives(GraphicsDevice graphicsDevice, int maxVertices, int maxIndices)
        {
            _graphicsDevice = graphicsDevice;
            
            Main.QueueMainThreadAction(() =>
            {
                _vertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), maxVertices, BufferUsage.None);
                _indexBuffer = new DynamicIndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, maxIndices, BufferUsage.None);
            });
        }

        public void Render(Effect effect, int vertexCount, int primitiveCount)
        {
            if (_vertexBuffer is null || _indexBuffer is null)
            {
                return;
            }

            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, primitiveCount);
            }
        }

        public void SetVertices(VertexPositionColorTexture[] vertices)
        {
            // Check if a resize is necessary
            if (vertices.Length > _vertexBuffer.VertexCount)
            {
                int newSize = _vertexBuffer.VertexCount * RESIZE_MULTIPLIER;
                
                Redemption.Instance.Logger.Info("Resizing primitives vertex buffer to " + newSize);
                DynamicVertexBuffer vertexBuffer = new(_graphicsDevice,  typeof(VertexPositionColorTexture), newSize, BufferUsage.None);
                
                _vertexBuffer.Dispose();
                _vertexBuffer = vertexBuffer;
            }
            
            _vertexBuffer?.SetData(0, vertices, 0, vertices.Length, VertexPositionColorTexture.VertexDeclaration.VertexStride, SetDataOptions.None);
        }

        public void SetIndices(short[] indices)
        {
            // Check if a resize is necessary
            if (indices.Length > _indexBuffer.IndexCount)
            {
                int newSize = _indexBuffer.IndexCount * RESIZE_MULTIPLIER;

                Redemption.Instance.Logger.Info("Resizing primitives index buffer to " + newSize);
                DynamicIndexBuffer indexBuffer = new(_graphicsDevice,  typeof(short), newSize, BufferUsage.None);
                
                _indexBuffer.Dispose();
                _indexBuffer = indexBuffer;
            }
            
            _indexBuffer?.SetData(0, indices, 0, indices.Length, SetDataOptions.None);
        }

        public void Dispose()
        {
            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();
            GC.SuppressFinalize(this);
        }
        
        ~Primitives()
        {
            Redemption.Instance.Logger.Warn("A graphics resource of type " + GetType().Name + " was not disposed! We'll dispose it this time, but we should fix this.");
            Main.QueueMainThreadAction(Dispose);
        }
    }
}