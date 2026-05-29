using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Redemption.Effects.Trails.Tips
{
    // Note: Every vertex in this tip is drawn twice, but the performance impact from this would be very little
    public class RoundedTip : ITrailTip
    {
        // The edge vextex count is count * 2 + 1, but one extra is added for the center, and there is one extra hidden vertex.
        public int ExtraVertices => (triCount * 2) + 3;

        public int ExtraIndices => ((triCount * 2) * 3) + 5;

        // TriCount is the amount of tris the curve should have, higher means a better circle approximation. (Keep in mind each tri is drawn twice)
        private readonly int triCount;

        public RoundedTip(int triCount = 2)//amount of tris
        {
            this.triCount = triCount;

            if (triCount < 2)
            {
                throw new ArgumentException($"Parameter {nameof(triCount)} cannot be less than 2.");
            }
        }

        public void GenerateMesh(Vector2 trailTipPosition, Vector2 trailTipNormal, int startFromIndex, out VertexPositionColorTexture[] vertices, out short[] indices, TrailWidthFunction trailWidthFunction, TrailColorFunction trailColorFunction)
        {
            /*   C---D
             *  / \ / \
             * B---A---E (first layer)
             * 
             *   H---G
             *  / \ / \
             * I---A---F (second layer)
             * 
             * This tip attempts to approximate a semicircle as shown.
             * Consists of a fan of triangles which share a common center (A).
             * The higher the tri count, the more points there are.
             * Point E and F are ontop of eachother to prevent a visual seam.
             */

            /// We want an array of vertices the size of the accuracy amount plus the center.
            vertices = new VertexPositionColorTexture[ExtraVertices];

            Vector2 fanCenterTexCoord = new(1, 0.5f);

            vertices[0] = new VertexPositionColorTexture(trailTipPosition.Vec3(), (trailColorFunction?.Invoke(fanCenterTexCoord) ?? Color.White) * 0.75f, fanCenterTexCoord);

            List<short> indicesTemp = new();

            for (int k = 0; k <= triCount; k++)
            {
                // Referring to the illustration: 0 is point B, 1 is point E, any other value represent the rotation factor of points in between.
                float rotationFactor = k / (float)(triCount);

                // Rotates by pi/2 - (factor * pi) so that when the factor is 0 we get B and when it is 1 we get E.
                float angle = MathHelper.PiOver2 - (rotationFactor * MathHelper.Pi);


                Vector2 circlePoint = trailTipPosition + (trailTipNormal.RotatedBy(angle) * (trailWidthFunction?.Invoke(1) ?? 1));

                // Handily, the rotation factor can also be used as a texture coordinate because it is a measure of how far around the tip a point is.
                Vector2 circleTexCoord = new(rotationFactor, 1);

                // The transparency must be changed a bit so it looks right when overlapped
                Color circlePointColor = (trailColorFunction?.Invoke(new Vector2(1, 0)) ?? Color.White) * rotationFactor * 0.85f;

                vertices[k + 1] = new VertexPositionColorTexture(circlePoint.Vec3(), circlePointColor, circleTexCoord);

                //if (k == triCount)//leftover and not needed
                //{
                //    continue;
                //}

                short[] tri = new short[]
                {
                    /* Because this is a fan, we want all triangles to share a common point. This is represented by index 0 offset to the next available index.
                     * The other indices are just pairs of points around the fan. The vertex k points along the circle is just index k + 1, followed by k + 2 at the next one along.
                     * The reason these are offset by 1 is because index 0 is taken by the fan center.
                     */

                    //before the fix, I believe these being in the wrong order was what prevented it from drawing
                    (short)startFromIndex,
                    (short)(startFromIndex + k + 2),
                    (short)(startFromIndex + k + 1)
                };

                indicesTemp.AddRange(tri);
            }

            // These 2 forloops overlap so that 2 points share the same location, this hidden point hides a tri that acts as a transition from one UV to another
            for (int k = triCount + 1; k <= triCount * 2 + 1; k++)
            {
                // Referring to the illustration: triCount + 1 is point F, 1 is point I, any other value represent the rotation factor of points in between.
                float rotationFactor = ((k - 1) / (float)(triCount)) - 1;

                // Rotates by pi/2 - (factor * pi) so that when the factor is 0 we get B and when it is 1 we get E.
                float angle = MathHelper.PiOver2 - (rotationFactor * MathHelper.Pi);

                Vector2 circlePoint = trailTipPosition + (trailTipNormal.RotatedBy(-angle) * (trailWidthFunction?.Invoke(1) ?? 1));

                // Handily, the rotation factor can also be used as a texture coordinate because it is a measure of how far around the tip a point is.
                Vector2 circleTexCoord = new(rotationFactor, 0);

                // The transparency must be changed a bit so it looks right when overlapped
                Color circlePointColor = ((trailColorFunction?.Invoke(new Vector2(1, 0)) ?? Color.White) * rotationFactor * 0.75f);

                vertices[k + 1] = new VertexPositionColorTexture(circlePoint.Vec3(), circlePointColor, circleTexCoord);

                // Skip last point, since there is no point to pair with it.
                if (k == triCount * 2 + 1)
                {
                    continue;
                }

                short[] tri = new short[]
                {
                    /* Because this is a fan, we want all triangles to share a common point. This is represented by index 0 offset to the next available index.
                     * The other indices are just pairs of points around the fan. The vertex k points along the circle is just index k + 1, followed by k + 2 at the next one along.
                     * The reason these are offset by 1 is because index 0 is taken by the fan center.
                     */

                    //The order of the indices is reversed since the direction is backwards
                    (short)startFromIndex,
                    (short)(startFromIndex + k + 1),
                    (short)(startFromIndex + k + 2)
                };

                indicesTemp.AddRange(tri);
            }

            indices = indicesTemp.ToArray();
        }
    }
}