using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Friendly.SpiritSummons
{
    public class MossyGoliath_SS_ToxicBreath : ModProjectile
    {
        public override string Texture => "Redemption/Textures/IceMist";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Toxic Breath");
            ElementID.ProjWind[Type] = true;
            ElementID.ProjPoison[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.timeLeft = 140;
            Projectile.scale = Main.rand.NextFloat(0.2f, 0.4f);
            Projectile.rotation = RedeHelper.RandomRotation();
        }

        public override void AI()
        {
            Projectile.velocity *= .98f;
            if (Projectile.localAI[0] == 0)
                Projectile.localAI[0] = Main.rand.Next(1, 3);

            if (Projectile.localAI[0] == 1)
                Projectile.rotation -= 0.003f;
            else if (Projectile.localAI[0] == 2)
                Projectile.rotation += 0.003f;

            if (Projectile.timeLeft < 80)
            {
                Projectile.alpha += 20;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.Purple), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.Purple), -Projectile.rotation / 2, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.Purple), -Projectile.rotation, drawOrigin, Projectile.scale * 0.1f, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.Purple), Projectile.rotation / 2, drawOrigin, Projectile.scale * 1.1f, effects, 0);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 30);
        }
    }
}