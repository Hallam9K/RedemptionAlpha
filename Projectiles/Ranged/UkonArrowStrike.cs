using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class UkonArrowStrike : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/ADD/UkkoStrike";
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
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.scale *= 2;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Redemption().ParryBlacklist = true;
        }
        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.frame >= 12 && Projectile.frame < 15 ? null : false;
        private int npcType;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 21)
                    Projectile.Kill();
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity, Projectile.Opacity, Projectile.Opacity);
            Projectile.localAI[0]++;
            NPC npc = Main.npc[0];
            if (Projectile.ai[1] == 0)
                npc = Main.npc[(int)Projectile.ai[0]];
            if (Projectile.localAI[0] == 1)
            {
                if (Projectile.ai[1] == 0)
                    npcType = npc.type;
                Projectile.position.Y -= 540;
                Projectile.alpha = 0;
            }
            if (Projectile.ai[1] == 0 && npc.active && npc.type == npcType && Projectile.localAI[0] < 36)
                Projectile.Center = npc.Center - new Vector2(0, 540);
            if (Projectile.localAI[0] == 36)
            {
                Player player = Main.player[Projectile.owner];
                player.GetModPlayer<ScreenPlayer>().Rumble(4, 10);
                SoundEngine.PlaySound(CustomSounds.Thunderstrike, Projectile.position);
                for (int i = 0; i < 30; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(Projectile.Center.X - 25, Projectile.Bottom.Y - 25), 50, 50, DustID.Electric, newColor: Color.Yellow, Scale: 1.2f);
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].velocity *= 2;
                }
                Rectangle boom = new((int)Projectile.Center.X - 25, (int)Projectile.Bottom.Y - 25, 50, 50);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (!target.active || target.friendly || target.dontTakeDamage)
                        continue;

                    if (target.immune[Projectile.whoAmI] > 0 || !target.Hitbox.Intersects(boom))
                        continue;

                    target.immune[Projectile.whoAmI] = 20;
                    int hitDirection = target.RightOfDir(Projectile);
                    BaseAI.DamageNPC(target, (int)(Projectile.damage * 1.2f), Projectile.knockBack, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                }
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
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, position, new Rectangle?(rect), Projectile.GetAlpha(Color.LightGoldenrodYellow), Projectile.rotation + MathHelper.PiOver2, origin, Projectile.scale, 0);

            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation + MathHelper.PiOver2, origin, Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 10;
            target.immune[Projectile.owner] = 0;

            target.AddBuff(BuffID.Electrified, target.HasBuff(BuffID.Wet) ? 320 : 160);
        }
    }
}