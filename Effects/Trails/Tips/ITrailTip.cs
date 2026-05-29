using Microsoft.Xna.Framework.Graphics;

namespace Redemption.Effects.Trails.Tips
{
    public delegate float TrailWidthFunction(float factorAlongTrail);

    public delegate Color TrailColorFunction(Vector2 textureCoordinates);
    
    public interface ITrailTip
    {
        int ExtraVertices { get; }

        int ExtraIndices { get; }

        void GenerateMesh(Vector2 trailTipPosition, Vector2 trailTipNormal, int startFromIndex, out VertexPositionColorTexture[] vertices, out short[] indices, TrailWidthFunction trailWidthFunction, TrailColorFunction trailColorFunction);
    }
}