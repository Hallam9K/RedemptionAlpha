using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Projectiles.Magic;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.BaseExtension;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class PureIronStaff_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Magic/PureIronStaff";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pure-Iron Staff");
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 44;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ownerHitCheck = true;
            Projectile.ignoreWater = true;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        public bool glow;
        public float glowTimer;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float num = MathHelper.ToRadians(0f);
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Projectile.spriteDirection == -1)
                num = MathHelper.ToRadians(90f);

            if (!player.channel)
                Projectile.Kill();

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

            if (glow)
            {
                glowTimer++;
                if (glowTimer > 60)
                {
                    glow = false;
                    glowTimer = 0;
                }
            }

            Projectile.position = player.RotatedRelativePoint(player.MountedCenter + RedeHelper.PolarVector(30, Projectile.velocity.ToRotation()), true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + num + MathHelper.PiOver4;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            if (Projectile.localAI[0]++ == 0 && Projectile.owner == Main.myPlayer)
            {
                Projectile.alpha = 0;
                SoundEngine.PlaySound(SoundID.Item30, player.position);
                glow = true;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center + Vector2.Normalize(Projectile.velocity) * 35f, Projectile.velocity, ModContent.ProjectileType<IceBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Projectile.whoAmI);
            }
        }
        private float Opacity { get => glowTimer; set => glowTimer = value; }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = ModContent.Request<Texture2D>("Redemption/Textures/Star").Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 position = Projectile.Center - Main.screenPosition + RedeHelper.PolarVector(15, Projectile.velocity.ToRotation()) + Vector2.UnitY * Projectile.gfxOffY;
            Color colour = Color.Lerp(Color.LightBlue, Color.LightCyan, 1f / Opacity * 10f) * (1f / Opacity * 10f);
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (glow)
            {
                Main.EntitySpriteDraw(texture, position, null, colour, Projectile.rotation, origin, 0.8f, spriteEffects, 0);
                Main.EntitySpriteDraw(texture, position, null, colour * 0.4f, Projectile.rotation, origin, 1, spriteEffects, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
