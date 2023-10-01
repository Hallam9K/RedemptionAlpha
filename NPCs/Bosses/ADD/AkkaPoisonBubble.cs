using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class AkkaPoisonBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Poison Bubble");
            ElementID.ProjWater[Type] = true;
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ToxicBubble);
            AIType = ProjectileID.ToxicBubble;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
        }
        private float teleAlpha;
        public override void PostAI()
        {
            if (Projectile.timeLeft < 60)
                teleAlpha += 0.016f;
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.hostile = true;
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
            RedeHelper.PlayerRadiusDamage(120, Projectile, NPCHelper.HostileProjDamageInc(Projectile.damage), Projectile.knockBack);

            for (int i = 0; i < 20; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, Scale: 2);
                Main.dust[dustIndex].velocity *= 8f;
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => target.AddBuff(BuffID.Venom, 120);
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D telegraph = ModContent.Request<Texture2D>("Redemption/Textures/RadialTelegraph3").Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = new(telegraph.Width / 2f, telegraph.Height / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(telegraph, position, null, Color.MediumPurple * teleAlpha, Projectile.rotation, origin, Projectile.scale * 0.6f, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return true;
        }
    }
}