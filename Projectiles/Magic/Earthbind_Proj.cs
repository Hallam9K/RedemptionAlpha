using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class Earthbind_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Earthbind");
            Main.projFrames[Projectile.type] = 21;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.whoAmI == Main.myPlayer)
            {
                if (!player.channel || !player.active || player.dead)
                    Projectile.Kill();

                player.itemTime = 20;
                player.itemAnimation = 20;
                Projectile.MoveToVector2(Main.MouseWorld, 3);
            }
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 21)
                {
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        NPC npc = Main.npc[n];
                        if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.knockBackResist <= 0)
                            continue;

                        if (!Projectile.Hitbox.Intersects(npc.Hitbox))
                            continue;

                        for (int i = 0; i < 3; i++)
                        {
                            int dust = Dust.NewDust(npc.Center + npc.velocity - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 1.5f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].velocity *= 0;
                            Color dustColor = new(255, 244, 163) { A = 0 };
                            Main.dust[dust].color = dustColor;
                        }
                        npc.AddBuff(ModContent.BuffType<StunnedDebuff>(), (int)(300 * npc.knockBackResist));
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<Earthbind_Effect>(), 0, 0, Main.myPlayer, npc.whoAmI);
                    }
                    Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
                    Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 4;

                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.position);
                    for (int i = 0; i < 15; i++)
                    {
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sandnado, Scale: .6f);
                        int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, Scale: 1);
                        Main.dust[d].velocity.X = 0;
                        Main.dust[d].velocity.Y = -Main.rand.NextFloat(3, 6);
                    }
                    Projectile.Kill();
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 21;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White) * .5f, Projectile.rotation, origin, Projectile.scale + .2f, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }
    }
    public class Earthbind_Effect : ModProjectile
    {
        public override string Texture => "Redemption/Textures/EarthbindEffect";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Earthbind");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc.active && npc.RedemptionNPCBuff().stunned)
                Projectile.timeLeft = 2;
            else
            {
                int dust = Dust.NewDust(npc.Center + npc.velocity - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0;
                Color dustColor = new(255, 244, 163) { A = 0 };
                Main.dust[dust].color = dustColor;

                return;
            }

            Projectile.Center = npc.Center;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.2f, 0.4f, 0.2f);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White) * .5f, Projectile.rotation, origin, Projectile.scale + scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}