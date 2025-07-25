using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Utilities;
using Terraria.GameContent;

namespace Redemption.NPCs.Lab.Janitor
{
    public class JanitorMop_Proj : ModProjectile
    {
        public Vector2[] oldPos = new Vector2[8];
        public float[] oldRot = new float[8];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            switch (Projectile.ai[0])
            {
                case 0:
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    Projectile.velocity.Y += 0.02f;
                    break;
                case 1:
                    if (Projectile.velocity.X > 0)
                        Projectile.rotation += 0.5f;
                    else
                        Projectile.rotation -= 0.5f;
                    break;
            }
            if (Projectile.localAI[0]++ >= 30)
                Projectile.friendly = true;

            for (int k = oldPos.Length - 1; k > 0; k--)
            {
                oldPos[k] = oldPos[k - 1];
                oldRot[k] = oldRot[k - 1];
            }
            oldPos[0] = Projectile.Center;
            oldRot[0] = Projectile.rotation;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = texture.Size() / 2;
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int k = 0; k < oldPos.Length; k++)
            {
                Vector2 drawPos = oldPos[k] - Main.screenPosition;
                Color color = Color.White * ((oldPos.Length - k) / (float)oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color.WithAlpha(0) * .5f, oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WoodFurniture, 0f, 0f, 100, default, 2.0f);
                    Main.dust[dustIndex].velocity *= 2f;
                }
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
                Projectile.velocity *= 0.95f;
                return false;
            }
            else
                return true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.type == ModContent.NPCType<JanitorBot>() && target.ai[0] != 4 && target.ai[0] != 5)
            {
                target.RedemptionGuard().GuardPoints = 0;
                target.ai[0] = 4;
                target.ai[1] = 0;
                target.ai[2] = 0;
                target.RedemptionGuard().GuardBreakCheck(target, DustID.Electric, CustomSounds.GuardBreak, 10, 1, 1000);
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            for (int i = 0; i < 8; i++)
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.WoodFurniture, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f);

            if (Main.netMode == NetmodeID.Server)
                return;

            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, Find<ModGore>("Redemption/JanitorMopGore1").Type, 1);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(6, 22), Projectile.velocity, Find<ModGore>("Redemption/JanitorMopGore2").Type, 1);
        }
    }
}
