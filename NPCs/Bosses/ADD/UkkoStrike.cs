using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class UkkoStrike : ModProjectile
    {
        private static Asset<Texture2D> warning;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            warning = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/ADD/LightningWarning");
        }
        public override void Unload()
        {
            warning = null;
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ukko's Lightning");
            Main.projFrames[Projectile.type] = 24;
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 540;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale *= 2;
            Projectile.alpha = 255;
            Projectile.Redemption().ParryBlacklist = true;
        }
        public int warningFrames;
        public int frameCounters;
        public override void AI()
        {
            frameCounters++;
            if (frameCounters >= 3)
            {
                warningFrames++;
                frameCounters = 0;
            }
            if (warningFrames >= 2)
                warningFrames = 0;
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 21)
                    Projectile.Kill();
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity, Projectile.Opacity, Projectile.Opacity);
            Projectile.localAI[0]++;
            if (Projectile.frame >= 12 && Projectile.frame < 15)
                Projectile.hostile = true;
            else
                Projectile.hostile = false;

            if (Projectile.localAI[0] == 1)
            {
                Projectile.position.Y -= 540;
                Projectile.alpha = 0;
            }
            if (Projectile.localAI[0] == 36)
            {
                Player player = Main.player[Projectile.owner];
                Main.NewLightning();
                player.GetModPlayer<ScreenPlayer>().Rumble(10, 10);
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Thunderstrike, Projectile.position);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X, Projectile.Bottom.Y), Projectile.velocity, ModContent.ProjectileType<UkkoStrikeZap>(), (int)(Projectile.damage * 1.2f), Projectile.knockBack, Projectile.owner);
            }
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 24;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, position, new Rectangle?(rect), Projectile.GetAlpha(Color.LightGoldenrodYellow), Projectile.rotation + MathHelper.PiOver2, origin, Projectile.scale, 0);

            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation + MathHelper.PiOver2, origin, Projectile.scale, 0, 0);

            int height2 = warning.Value.Height / 2;
            int y2 = height2 * warningFrames;
            Rectangle rect2 = new(0, y2, warning.Value.Width, height2);
            Vector2 origin2 = new(warning.Value.Width / 2f, height2 / 2f);

            if (Projectile.frame < 12)
                Main.EntitySpriteDraw(warning.Value, position, new Rectangle?(rect2), Projectile.GetAlpha(Color.White) * 0.8f, Projectile.rotation + MathHelper.PiOver2, origin2, Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Electrified, target.HasBuff(BuffID.Wet) ? 320 : 160);
        }
    }
    public class UkkoStrikeZap : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ukko's Lightning");
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
            Projectile.Redemption().ParryBlacklist = true;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, newColor: Color.Yellow, Scale: 1.2f);
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].velocity *= 2;
                }
                Projectile.localAI[0] = 1;
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Electrified, target.HasBuff(BuffID.Wet) ? 320 : 160);
        }
    }
}