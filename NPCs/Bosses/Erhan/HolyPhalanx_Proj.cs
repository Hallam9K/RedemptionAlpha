using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class HolyPhalanx_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Holy Phalanx");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjHoly[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public int Host
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public ref float AIRotation => ref Projectile.ai[1];

        public int AITimer
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        public int AIState;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)AIState);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AIState = reader.ReadByte();
        }

        public float rot;
        public float speed = 1;
        public float dist;
        public float dist2 = 0.2f;
        public bool s;
        public override void AI()
        {
            NPC host = Main.npc[Host];
            if (!host.active || (host.type != ModContent.NPCType<Erhan>() && host.type != ModContent.NPCType<ErhanSpirit>()))
                Projectile.Kill();
            switch (AIState)
            {
                case 0:
                    Projectile.rotation = -MathHelper.PiOver4;
                    Projectile.Center = host.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(AIRotation)) * 80;
                    if (!s)
                    {
                        RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120));
                        RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120), 0.13f, 0.83f, 0);
                        s = true;
                    }
                    if (AITimer++ >= 50)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.Slice2, Projectile.position);
                        rot = host.Center.ToRotation();
                        AITimer = 0;
                        AIState = 1;
                    }
                    break;
                case 1:
                    Projectile.Center = host.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(AIRotation + rot)) * (80 + dist);
                    if (AITimer++ >= 40)
                    {
                        Projectile.rotation = Projectile.DirectionTo(host.Center).ToRotation() - MathHelper.PiOver4;
                        rot += speed;
                        dist += dist2;
                        if (AITimer >= 160)
                            speed *= 0.99f;
                        else
                            speed *= 1.01f;
                        speed = MathHelper.Min(speed, 3);
                        if (AITimer >= 160)
                            dist2 *= 0.98f;
                        else
                            dist2 *= 1.03f;
                        dist2 = MathHelper.Min(dist2, 10);
                    }
                    else
                        Projectile.rotation.SlowRotation(Projectile.DirectionTo(host.Center).ToRotation() - MathHelper.PiOver4, MathHelper.Pi / 10);

                    if (AITimer >= 240)
                    {
                        Projectile.velocity *= 0;
                        AITimer = 0;
                        AIState = 2;
                    }
                    break;
                case 2:
                    Projectile.rotation.SlowRotation(Projectile.DirectionTo(host.Center).ToRotation() + MathHelper.PiOver4, MathHelper.Pi / 10);
                    if (AITimer++ == 40 && !Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Slice1, host.Center);
                    if (AITimer >= 40)
                        Projectile.velocity = Projectile.DirectionTo(host.Center) * 20;
                    if (Projectile.DistanceSQ(host.Center) <= 60 * 60)
                        Projectile.Kill();
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                Color color = new Color(255, 255, 120) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color * .5f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Dust dust2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), Vector2.Zero, Scale: 2);
            dust2.noGravity = true;
            Color dustColor = new(255, 255, 120) { A = 0 };
            dust2.color = dustColor;
            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                Main.dust[dust].noGravity = true;
            }
        }
    }
    public class HolyPhalanx_Proj2 : HolyPhalanx_Proj
    {
        public override string Texture => "Redemption/NPCs/Bosses/Erhan/HolyPhalanx_Proj";
        public override void SetStaticDefaults() => base.SetStaticDefaults();
        public override void SetDefaults() => base.SetDefaults();
        public override bool PreAI()
        {
            Projectile host = Main.projectile[Host];
            if (!host.active || host.type != ModContent.ProjectileType<Erhan_Bible>())
                Projectile.Kill();
            switch (AIState)
            {
                case 0:
                    Projectile.rotation = -MathHelper.PiOver4;
                    Projectile.Center = host.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(AIRotation)) * 80;
                    if (!s)
                    {
                        RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120));
                        RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120), 0.13f, 0.83f, 0);
                        s = true;
                    }
                    if (AITimer++ >= 50)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.Slice2, Projectile.position);
                        rot = host.Center.ToRotation();
                        AITimer = 0;
                        AIState = 1;
                    }
                    break;
                case 1:
                    Projectile.Center = host.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(AIRotation + rot)) * (80 + dist);
                    if (AITimer++ >= 40)
                    {
                        Projectile.rotation = Projectile.DirectionTo(host.Center).ToRotation() - MathHelper.PiOver4;
                        rot += speed;
                        dist += dist2;
                        if (AITimer >= 160)
                            speed *= 0.99f;
                        else
                            speed *= 1.01f;
                        speed = MathHelper.Min(speed, 3);
                        if (AITimer >= 160)
                            dist2 *= 0.98f;
                        else
                            dist2 *= 1.03f;
                        dist2 = MathHelper.Min(dist2, 10);
                    }
                    else
                        Projectile.rotation.SlowRotation(Projectile.DirectionTo(host.Center).ToRotation() - MathHelper.PiOver4, MathHelper.Pi / 10);
                    if (AITimer >= 240)
                    {
                        Projectile.velocity *= 0;
                        AITimer = 0;
                        AIState = 2;
                    }
                    break;
                case 2:
                    Projectile.rotation.SlowRotation(Projectile.DirectionTo(host.Center).ToRotation() + MathHelper.PiOver4, MathHelper.Pi / 10);
                    if (AITimer++ == 40 && !Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Slice1, host.Center);
                    if (AITimer >= 40)
                        Projectile.velocity = Projectile.DirectionTo(host.Center) * 20;
                    if (Projectile.DistanceSQ(host.Center) <= 60 * 60)
                        Projectile.Kill();
                    break;
            }
            return false;
        }
    }
}
