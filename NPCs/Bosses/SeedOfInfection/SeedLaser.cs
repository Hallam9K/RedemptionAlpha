using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Redemption.Globals;
using Redemption.Buffs.Debuffs;

namespace Redemption.NPCs.Bosses.SeedOfInfection
{
    public class SeedLaser : ModProjectile // Thanks to Dan Yami for the laser code
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Xenomite Beam");
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 3600;
            Projectile.tileCollide = false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<BileDebuff>(), 120);
        internal const float charge = 40f;
        public float LaserLength { get { return Projectile.localAI[1]; } set { Projectile.localAI[1] = value; } }
        public const float LaserLengthMax = 2000f;
        int multiplier = 1;

        public override bool ShouldUpdatePosition() => false;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active)
                Projectile.Kill();

            Projectile.Center = npc.Center;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.ai[1] = 5;
            }
            else if (Projectile.localAI[0] >= 20)
                Projectile.ai[1] += 5f * multiplier;
            Projectile.localAI[0]++;
            if (Projectile.ai[1] == charge)
            {
                Projectile.hostile = true;
            }
            if (Projectile.ai[1] >= charge + 30f && multiplier == 1)
            {
                multiplier = -1;
            }
            if (multiplier == -1 && Projectile.ai[1] <= 0)
                Projectile.Kill();

            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57079637f;
            Projectile.velocity = Vector2.Normalize(Projectile.velocity);

            float[] sampleArray = new float[2];
            Collision.LaserScan(Projectile.Center, Projectile.velocity, 0, LaserLengthMax, sampleArray);
            float sampledLength = 0f;
            for (int i = 0; i < sampleArray.Length; i++)
            {
                sampledLength += sampleArray[i];
            }
            sampledLength /= sampleArray.Length;
            float amount = 0.75f; // last prism is 0.75 rather than 0.5?
            LaserLength = MathHelper.Lerp(LaserLength, sampledLength, amount);
            LaserLength = LaserLengthMax;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, projHitbox.Width, ref collisionPoint);
        }
        public override bool? CanCutTiles()
        {
            DelegateMethods.tilecut_0 = Terraria.Enums.TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, Projectile.width * Projectile.scale * 2, new Utils.TileActionAttempt(CutTilesAndBreakWalls));
            return true;
        }

        private bool CutTilesAndBreakWalls(int x, int y)
        {
            return DelegateMethods.CutTiles(x, y);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
                return false;

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D texture2D19 = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D texture2D20 = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/SeedOfInfection/SeedLaser_Beam").Value;
            Texture2D texture2D21 = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/SeedOfInfection/SeedLaser_End").Value;
            float num228 = LaserLength;
            Color color44 = Color.White * 0.8f;
            Texture2D arg_AF99_1 = texture2D19;
            Vector2 arg_AF99_2 = Projectile.Center + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition;
            Rectangle? sourceRectangle2 = null;
            Main.spriteBatch.Draw(arg_AF99_1, arg_AF99_2, sourceRectangle2, color44, Projectile.rotation, texture2D19.Size() / 2f, new Vector2(Math.Min(Projectile.ai[1], charge) / charge, 1f), SpriteEffects.None, 0f);
            num228 -= (texture2D19.Height / 2 + texture2D21.Height) * Projectile.scale;
            Vector2 value20 = Projectile.Center + new Vector2(0, Projectile.gfxOffY);
            value20 += Projectile.velocity * Projectile.scale * texture2D19.Height / 2f;
            if (num228 > 0f)
            {
                float num229 = 0f;
                Rectangle rectangle7 = new(0, 16 * (Projectile.timeLeft / 3 % 5), texture2D20.Width, 16);
                while (num229 + 1f < num228)
                {
                    if (num228 - num229 < rectangle7.Height)
                    {
                        rectangle7.Height = (int)(num228 - num229);
                    }
                    Main.spriteBatch.Draw(texture2D20, value20 - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle7), color44, Projectile.rotation, new Vector2(rectangle7.Width / 2, 0f), new Vector2(Math.Min(Projectile.ai[1], charge) / charge, 1f), SpriteEffects.None, 0f);
                    num229 += rectangle7.Height * Projectile.scale;
                    value20 += Projectile.velocity * rectangle7.Height * Projectile.scale;
                    rectangle7.Y += 16;
                    if (rectangle7.Y + rectangle7.Height > texture2D20.Height)
                    {
                        rectangle7.Y = 0;
                    }
                }
            }
            SpriteBatch arg_B1FF_0 = Main.spriteBatch;
            Texture2D arg_B1FF_1 = texture2D21;
            Vector2 arg_B1FF_2 = value20 - Main.screenPosition;
            sourceRectangle2 = null;
            arg_B1FF_0.Draw(arg_B1FF_1, arg_B1FF_2, sourceRectangle2, color44, Projectile.rotation, texture2D21.Frame(1, 1, 0, 0).Top(), new Vector2(Math.Min(Projectile.ai[1], charge) / charge, 1f), SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}