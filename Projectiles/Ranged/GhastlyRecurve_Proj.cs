using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Effects;
using System;
using System.Transactions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Redemption.Projectiles.Ranged
{
    public class GhastlyRecurve_Proj : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_" + NPCID.DungeonSpirit;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ghastly Spirit");
            Main.projFrames[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 520;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            int d2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            Main.dust[d2].noGravity = true;
            Vector2 vector = new Vector2(Projectile.ai[0], Projectile.ai[1]) - Projectile.Center;
            if (vector.Length() < Projectile.velocity.Length())
            {
                Projectile.velocity *= 0f;
                Projectile.rotation = 0;
                Projectile.localAI[0] = 1;
            }
            else if (Projectile.localAI[0] == 0)
            {
                vector.Normalize();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, vector * 11.2f, 0.1f);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || !proj.arrow || !proj.friendly || proj.type == ModContent.ProjectileType<SpiritArrow_Proj>() || proj.type == ModContent.ProjectileType<SpiritArrow_Shard>())
                    continue;

                if (other != null && other.active && other.type == Type && Collision.CheckAABBvLineCollision(Projectile.position, Projectile.Size, Projectile.Center, other.Center))
                {

                }
                SoundEngine.PlaySound(SoundID.Zombie53 with { Volume = 0.6f }, Projectile.Center);
                for (int j = 0; j < 10; j++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                    Main.dust[d].velocity *= 3f;
                }
                proj.active = false;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), proj.position, proj.velocity, ModContent.ProjectileType<SpiritArrow_Proj>(), proj.damage, proj.knockBack, player.whoAmI);
            }
        }
        private readonly Projectile other;
        public void AdditiveCall(SpriteBatch sB, Vector2 screenPos)
        {
            if (other != null && other.active && other.type == Type)
                DrawTether(other, screenPos, Color.Red, Color.Red, 40, 1);
        }
        public void DrawTether(Projectile Target, Vector2 screenPos, Color color1, Color color2, float Size, float Strength)
        {
            Effect effect = ModContent.Request<Effect>("Redemption/Effects/Beam", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/GlowTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
            effect.Parameters["progress"].SetValue(Main.GlobalTimeWrappedHourly / 3);
            effect.Parameters["uColor"].SetValue(color1.ToVector4());
            effect.Parameters["uSecondaryColor"].SetValue(color2.ToVector4());
            Vector2 dist = Target.Center - Projectile.Center;
            TrianglePrimitive tri = new()
            {
                TipPosition = Projectile.Center - screenPos,
                Rotation = dist.ToRotation(),
                Height = Size + 20 + dist.Length() * 1.5f,
                Color = Color.White * Strength,
                Width = Size + ((Target.width + Target.height) / 2f)
            };
            PrimitiveRenderer.DrawPrimitiveShape(tri, effect);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * Projectile.Opacity;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 2);
                Main.dust[d].noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.NPCDeath39 with { Volume = 0.4f }, Projectile.position);
        }
    }
}