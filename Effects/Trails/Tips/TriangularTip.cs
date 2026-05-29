using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Redemption.Effects.Trails.Tips
{
    public class TriangularTip : ITrailTip
    {
        public int ExtraVertices => 3;

        public int ExtraIndices => 3;

        private readonly float length;

        public TriangularTip(float length)
        {
            this.length = length;
        }

        public void GenerateMesh(Vector2 trailTipPosition, Vector2 trailTipNormal, int startFromIndex, out VertexPositionColorTexture[] vertices, out short[] indices, TrailWidthFunction trailWidthFunction, TrailColorFunction trailColorFunction)
        {
            /*     C
             *    / \
             *   /   \
             *  /     \
             * A-------B
             * 
             * This tip is arranged as the above shows.
             * Consists of a single triangle with indices (0, 1, 2) offset by the next available index.
             */

            Vector2 normalPerp = trailTipNormal.RotatedBy(MathHelper.PiOver2);

            float width = trailWidthFunction?.Invoke(1) ?? 1;
            Vector2 a = trailTipPosition + (normalPerp * width);
            Vector2 b = trailTipPosition - (normalPerp * width);
            Vector2 c = trailTipPosition + (trailTipNormal * length);

            Vector2 texCoordA = Vector2.UnitX;
            Vector2 texCoordB = Vector2.One;
            Vector2 texCoordC = new(1, 0.5f);//this fixes the texture being skewed off to the side

            Color colorA = trailColorFunction?.Invoke(texCoordA) ?? Color.White;
            Color colorB = trailColorFunction?.Invoke(texCoordB) ?? Color.White;
            Color colorC = trailColorFunction?.Invoke(texCoordC) ?? Color.White;

            vertices = new VertexPositionColorTexture[]
            {
                new VertexPositionColorTexture(a.Vec3(), colorA, texCoordA),
                new VertexPositionColorTexture(b.Vec3(), colorB, texCoordB),
                new VertexPositionColorTexture(c.Vec3(), colorC, texCoordC)
            };

            indices = new short[]
            {
                (short)startFromIndex,
                (short)(startFromIndex + 1),
                (short)(startFromIndex + 2)
            };
        }
    }
}