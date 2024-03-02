using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity.Intruder
{
    public class IntruderScarfPhysChain : IPhysChain
    {
        public int NumberOfSegments => 5;
        public Vector2 AnchorOffset => new(-10, -12);
        public int MaxFrames => 1;
        public int FrameCounterMax => 0;
        public bool Glow => false;
        public bool HasGlowmask => false;
        public int Shader => 0;
        public int GlowmaskShader => 0;

        public Texture2D GetTexture(Mod mod)
        {
            return ModContent.Request<Texture2D>("Redemption/Items/Armor/Vanity/Intruder/IntruderScarfPhysChain").Value;
        }
        public Texture2D GetGlowmaskTexture(Mod mod) => null;

        public Vector2 Force(Player player, int index, int dir, float gravDir, float time, NPC npc = null, Projectile proj = null)
        {
            Vector2 force = new(
                -dir * 0.5f,
                Player.defaultGravity * (0.5f + NumberOfSegments * NumberOfSegments * 0.5f / (1 + index))
                );

            if (!Main.gameMenu)
            {
                float windPower = 0.3f * dir * -10;

                // Wave in the wind
                force.X += windPower;
                force.Y += MathF.Sin(time * 2f * windPower - index * MathF.Sign(force.X)) * 0.3f * windPower * gravDir;
            }
            return force;
        }

        public Color GetColor(PlayerDrawSet drawInfo, Color baseColour)
        {
            return baseColour;
        }

        public Rectangle GetSourceRect(Texture2D texture, int index)
        {
            return texture.Frame(NumberOfSegments, 1, NumberOfSegments - 1 - index, 0);
        }

        public int Length(int index) => 10;

        public Vector2 OriginOffset(int index)
        {
            return index switch
            {
                0 => new Vector2(-2, 0),
                1 => new Vector2(-4, 0),
                2 => new Vector2(-6, 0),
                3 => new Vector2(-8, 0),
                _ => new Vector2(-10, 0),
            };
        }
    }
}