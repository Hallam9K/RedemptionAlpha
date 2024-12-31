using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class Bacteria_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bacteria");
            Main.projFrames[Projectile.type] = 2;
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = 4;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = Main.rand.Next(40, 71);
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }
        NPC target;
        public override void AI()
        {
            if (Projectile.ai[0] >= 5)
            {
                Projectile.penetrate = 1;
                if (RedeHelper.ClosestNPC(ref target, 3000, Projectile.Center))
                {
                    float speed = Projectile.rotation * 10;
                    Projectile.Move(target.Center, speed, 30);
                }
                else
                    Projectile.timeLeft--;
            }
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 2)
                    Projectile.frame = 0;
            }
            Projectile.velocity *= .98f;
            Projectile.rotation += 0.02f;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = .4f }, Projectile.position);
            for (int i = 0; i < 6; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Slime, Scale: .5f);
            if (Projectile.ai[0] < 3)
            {
                for (int i = 0; i < 2; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.Spread(7), ProjectileType<Bacteria_Proj>(), Projectile.damage, 0, Projectile.owner, Main.rand.Next(3, 5));
            }
            else if (Projectile.ai[0] == 3 || Projectile.ai[0] == 4)
            {
                for (int i = 0; i < 2; i++)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.Spread(7), ProjectileType<Bacteria_Proj>(), Projectile.damage, 0, Projectile.owner, Main.rand.Next(5, 7));
                    Main.projectile[p].timeLeft = Main.rand.Next(40, 181);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 2;
            int width = texture.Width / 7;
            int y = height * Projectile.frame;
            int x = width * (int)Projectile.ai[0];
            Rectangle rect = new(x, y, width, height);
            Vector2 drawOrigin = new(width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(RedeColor.FadeColour1) * 2, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffType<BInfectionDebuff>(), 1000);
        }
    }
}