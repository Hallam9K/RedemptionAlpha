using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class VesselStaff_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadesoul");
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.Redemption().Unparryable = true;
        }
        public override bool? CanCutTiles() => false;
        public Vector2 MoveVector2;
        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 8)
                    Projectile.frame = 0;
            }
            Vector2 Pos = new(Main.rand.Next(-120, 120), Main.rand.Next(-120, 120));
            Projectile.rotation = projOwner.channel ? 0 : Projectile.velocity.ToRotation() + 1.57f;
            if (Projectile.ai[1] == 0)
            {
                MoveVector2 = Pos;
                Projectile.ai[1] = 1;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (projOwner.channel && Projectile.ai[0] == 0)
                    Projectile.Move(projOwner.Center + MoveVector2, 10, 30);
                else
                {
                    if (Projectile.ai[0] == 0)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath52.WithVolume(0.5f), Projectile.position);
                        Projectile.timeLeft = 60;
                        Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 20;
                        Projectile.ai[0] = 1;
                    }
                }
            }
        }
        public override Color? GetAlpha(Color lightColor) => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.GhostWhite, Color.Black, Color.GhostWhite);
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, Scale: 2f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 8;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
    public class VesselStaff_Proj2 : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shade Ring");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead || !player.active || player.RedemptionPlayerBuff().shadowBinderCharge <= 0)
                Projectile.Kill();

            Projectile.velocity *= 0f;
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.Center = player.Center;
                if (Projectile.ai[0] == 0)
                    Projectile.ai[0] = 100;
                else
                {
                    Projectile.ai[0]++;
                    if (Projectile.ai[0] % 10 == 0)
                        player.RedemptionPlayerBuff().shadowBinderCharge--;
                }
            }
            for (int k = 0; k < 3; k++)
            {
                Vector2 vector;
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                vector.X = (float)(Math.Sin(angle) * Projectile.ai[0]);
                vector.Y = (float)(Math.Cos(angle) * Projectile.ai[0]);
                Dust dust2 = Main.dust[Dust.NewDust(player.Center + vector, 2, 2, ModContent.DustType<VoidFlame>(), Scale: 3f)];
                dust2.noGravity = true;
                dust2.velocity = -player.DirectionTo(dust2.position) * 4f;
            }
            for (int p = 0; p < Main.maxNPCs; p++)
            {
                NPC npc = Main.npc[p];
                if (!npc.active || npc.immortal || npc.dontTakeDamage || npc.friendly || Projectile.DistanceSQ(npc.Center) >= Projectile.ai[0] * Projectile.ai[0])
                    continue;

                if (Projectile.ai[0] % 5 == 0)
                    BaseAI.DamageNPC(npc, 300, 0, Projectile, true);
            }
        }
    }
}