using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using Terraria.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.GameContent;

namespace Redemption.NPCs.Bosses.ADD
{
    public class UkkoRainCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rain Cloud");
            Main.projFrames[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 354;
            Projectile.height = 144;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 3)
                Projectile.frame = 1;
            else if (Projectile.localAI[0] == 4)
                Projectile.frame = 2;
            else 
                Projectile.frame = 0;

            if (Projectile.alpha > 0 && Projectile.timeLeft >= 60 && Projectile.alpha > 100)
                Projectile.alpha -= 5;
            if (Projectile.timeLeft < 60)
                Projectile.alpha += 5;
            if (Projectile.alpha <= 100)
            {
                var list = Main.projectile.Where(x => x.Hitbox.Intersects(Projectile.Hitbox));
                foreach (var proj in list)
                {
                    if (Projectile != proj && !proj.friendly && !proj.minion && Projectile.localAI[0] == 0)
                    {
                        if (proj.type == ModContent.ProjectileType<UkkoBlizzard>())
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy, Scale: 1.5f);
                                dust.velocity = -Projectile.DirectionTo(dust.position);
                            }
                            Projectile.localAI[0] = 1;
                        }
                        else if (proj.type == ModContent.ProjectileType<DualcastBall>() || proj.type == ModContent.ProjectileType<UkkoLightning>() || proj.type == ModContent.ProjectileType<UkkoThunderwave>() || proj.type == ModContent.ProjectileType<UkkoStrike>())
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Zap1 with { Volume = .3f }, Projectile.position);
                            for (int i = 0; i < 20; i++)
                            {
                                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, Scale: 1.5f);
                                dust.velocity = -Projectile.DirectionTo(dust.position);
                            }
                            Projectile.localAI[0] = 2;
                        }
                        else if (proj.type == ModContent.ProjectileType<AkkaPoisonBubble>())
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, Scale: 1.5f);
                                dust.velocity = -Projectile.DirectionTo(dust.position);
                            }
                            Projectile.localAI[0] = 3;
                        }
                        else if (proj.type == ModContent.ProjectileType<AkkaHealingSpirit>())
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Water_Jungle, Scale: 1.5f);
                                dust.velocity = -Projectile.DirectionTo(dust.position);
                            }
                            Projectile.localAI[0] = 4;
                        }
                    }
                }
                switch (Projectile.localAI[0])
                {
                    case 0:
                        if (Main.rand.NextBool(5))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X + Main.rand.Next(-150, 150), Projectile.Center.Y + Main.rand.Next(4, 8)), new Vector2(0f, 0f), ModContent.ProjectileType<UkkoRain>(), 0, 0, Projectile.owner, 0, 0);
                        }
                        break;
                    case 1:
                        if (Main.rand.NextBool(5))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X + Main.rand.Next(-150, 150), Projectile.Center.Y + Main.rand.Next(4, 8)), new Vector2(0f, 0f), ModContent.ProjectileType<UkkoHail>(), 0, 0, Projectile.owner, 0, 0);
                        }
                        if (++Projectile.localAI[1] >= 180)
                        {
                            Projectile.localAI[1] = 0;
                            Projectile.localAI[0] = 0;
                        }
                        break;
                    case 2:
                        if (Main.rand.NextBool(3))
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Zap2 with { Volume = .2f, PitchVariance = .1f }, Projectile.position);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X + Main.rand.Next(-150, 151), Projectile.Center.Y + Main.rand.Next(50, 55)), new Vector2(0f, 2f), ModContent.ProjectileType<UkkoCloud_Thunder>(), NPCHelper.HostileProjDamage(85), 0, Projectile.owner, 0, 0);
                        }
                        if (++Projectile.localAI[1] >= 50)
                        {
                            Projectile.localAI[1] = 0;
                            Projectile.localAI[0] = 0;
                        }
                        break;
                    case 3:
                        if (Main.rand.NextBool(5))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X + Main.rand.Next(-150, 150), Projectile.Center.Y + Main.rand.Next(4, 8)), new Vector2(0f, 0f), ModContent.ProjectileType<UkkoRainPoison>(), 0, 0, Projectile.owner, 0, 0);
                        }
                        if (++Projectile.localAI[1] >= 180)
                        {
                            Projectile.localAI[1] = 0;
                            Projectile.localAI[0] = 0;
                        }
                        break;
                    case 4:
                        if (Main.rand.NextBool(5))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X + Main.rand.Next(-150, 150), Projectile.Center.Y + Main.rand.Next(4, 8)), new Vector2(0f, 0f), ModContent.ProjectileType<UkkoRainHealing>(), 0, 0, Projectile.owner, 0, 0);
                        }
                        if (++Projectile.localAI[1] >= 180)
                        {
                            Projectile.localAI[1] = 0;
                            Projectile.localAI[0] = 0;
                        }
                        break;
                }
            }
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 3;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center - Main.screenPosition, new Rectangle?(rect), lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}