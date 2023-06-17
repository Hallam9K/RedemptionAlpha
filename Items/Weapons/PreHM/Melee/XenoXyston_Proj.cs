using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class XenoXyston_Proj : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Xeno Xyston");
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetSafeDefaults()
        {
            Projectile.width = 122;
            Projectile.height = 122;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
        }

        private float rotSpeed = 0.05f;
        private float SwingSpeed;
        public override void AI()
        {
            Projectile.soundDelay--;
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item15 with { Pitch = rotSpeed - 0.5f }, Projectile.Center);
                Projectile.soundDelay = (int)(360 / ((rotSpeed * (SwingSpeed + 1) + 1) * MathHelper.TwoPi));
            }

            Player player = Main.player[Projectile.owner];
            if (Main.myPlayer == Projectile.owner)
            {
                if (!player.channel || player.noItems || player.CCed)
                    Projectile.Kill();
            }

            SwingSpeed = SetSwingSpeed(1);
            rotSpeed *= 1.01f;
            rotSpeed = MathHelper.Clamp(rotSpeed, 0.1f, 1.5f);

            Lighting.AddLight(Projectile.Center, 1f, 0.6f, 0f);
            Projectile.Center = player.MountedCenter;
            Projectile.position.X += player.width / 2 * player.direction;
            Projectile.spriteDirection = player.direction;
            Projectile.rotation += rotSpeed * SwingSpeed * player.direction;

            if (Projectile.rotation > MathHelper.TwoPi)
                Projectile.rotation -= MathHelper.TwoPi;
            else if (Projectile.rotation < 0)
                Projectile.rotation += MathHelper.TwoPi;

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = Projectile.rotation;
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = (int)(120 / ((rotSpeed * (SwingSpeed + 1) + 1) * MathHelper.TwoPi));
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float increase = rotSpeed * 10;
            increase = MathHelper.Clamp(increase, 0, 10);
            if (target.knockBackResist > 0)
                modifiers.Knockback.Flat += increase;

            Player player = Main.player[Projectile.owner];
            modifiers.HitDirectionOverride = target.RightOfDir(player);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}