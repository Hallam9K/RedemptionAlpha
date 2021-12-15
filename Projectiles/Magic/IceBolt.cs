using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.NPCBuffs;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Globals.Player;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class IceBolt : ModProjectile, ITrailProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 4;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.GetGlobalProjectile<RedeProjectile>().Unparryable = true;
        }

        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(Color.LightBlue, new Color(200, 223, 230)), new RoundCap(), new DefaultTrailPosition(), 30f, 100f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_3", AssetRequestMode.ImmediateLoad).Value, 0.1f, 1f, 1f));
        }

        private float glowRot;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            glowRot += 0.04f;

            if (player.channel && Projectile.ai[0] == 0)
            {
                float num = MathHelper.ToRadians(0f);
                Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
                if (Projectile.spriteDirection == -1)
                    num = MathHelper.ToRadians(90f);

                if (Main.myPlayer == Projectile.owner)
                {
                    float scaleFactor6 = 1f;
                    if (player.inventory[player.selectedItem].shoot == Projectile.type)
                    {
                        scaleFactor6 = player.inventory[player.selectedItem].shootSpeed * Projectile.scale;
                    }
                    Vector2 vector13 = Main.MouseWorld - vector;
                    vector13.Normalize();
                    if (vector13.HasNaNs())
                    {
                        vector13 = Vector2.UnitX * player.direction;
                    }
                    vector13 *= scaleFactor6;
                    if (vector13.X != Projectile.velocity.X || vector13.Y != Projectile.velocity.Y)
                        Projectile.netUpdate = true;

                    Projectile.velocity = vector13;
                    if (player.noItems || player.CCed || player.dead || !player.active)
                    {
                        Projectile.Kill();
                    }
                    Projectile.netUpdate = true;
                }

                Projectile.position = player.RotatedRelativePoint(player.MountedCenter + RedeHelper.PolarVector(35, Projectile.velocity.ToRotation()), true) - Projectile.Size / 2f;
                Projectile.rotation = Projectile.velocity.ToRotation() + num;
                Projectile.spriteDirection = Projectile.direction;

                Projectile.timeLeft = 180;
                Projectile.scale += 0.015f;
                Projectile.scale = MathHelper.Clamp(Projectile.scale, 1, 2.5f);
            }
            else if (Projectile.ai[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item43, player.position);
                DustHelper.DrawCircle(Projectile.Center, DustID.IceTorch, 2, 2, 2, 1, 3, nogravity: true);
                Projectile.tileCollide = true;
                Projectile.friendly = true;
                Projectile.velocity = RedeHelper.PolarVector(12, Projectile.velocity.ToRotation());
                Projectile.ai[0] = 1;
            }
            else
            {
                Projectile.LookByVelocity();
                Projectile.rotation += Projectile.velocity.Length() / 50 * Projectile.spriteDirection;
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item50, Projectile.position);
            DustHelper.DrawCircle(Projectile.Center, DustID.IceTorch, 2 * Projectile.scale, 1, 1, 1, 3, nogravity: true);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("Redemption/Textures/Star").Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Rectangle rectGlow = new(0, 0, glowTex.Width, glowTex.Height);
            Vector2 drawOriginGlow = new(glowTex.Width / 2, glowTex.Height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale * 0.5f, effects, 0);

            if (Projectile.ai[0] == 0)
            {
                Main.EntitySpriteDraw(glowTex, Projectile.Center - Main.screenPosition, new Rectangle?(rectGlow), Projectile.GetAlpha(Color.LightBlue) * 0.6f, glowRot, drawOriginGlow, Projectile.scale * 0.7f, effects, 0);
                Main.EntitySpriteDraw(glowTex, Projectile.Center - Main.screenPosition, new Rectangle?(rectGlow), Projectile.GetAlpha(Color.LightBlue) * 0.4f, -glowRot, drawOriginGlow, Projectile.scale * 0.7f, effects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];

            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.Frostburn, 240);

            if (player.GetModPlayer<BuffPlayer>().pureIronBonus)
                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.Frostburn, 240);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => damage = (int)(damage * Projectile.scale);
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit) => damage = (int)(damage * Projectile.scale);
    }
}