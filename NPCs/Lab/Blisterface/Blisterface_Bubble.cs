using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.Blisterface
{
    public class Blisterface_Bubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Toxic Bubble");
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
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ToxicBubble, Scale: 2f);
                Main.dust[dustIndex].velocity *= 2f;
            }
            if (skrongle == 2)
                RedeHelper.SpawnNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<BlisteredFish>());
        }
        private int skrongle;
        public override void PostAI()
        {
            if (Projectile.ai[1] == 1)
            {
                if (skrongle == 0)
                {
                    if (Main.rand.NextBool(5))
                        skrongle = 2;
                    else
                        skrongle = 1;
                }
                Projectile.tileCollide = true;
                if (Main.rand.NextBool(3))
                {
                    int dust1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ToxicBubble, Scale: 2f);
                    Main.dust[dust1].noGravity = true;
                }
                Projectile.velocity.Y += 0.3f;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            }
            else
            {
                if (skrongle == 0)
                {
                    if (Main.rand.NextBool(10))
                        skrongle = 2;
                    else
                        skrongle = 1;
                }
                if (Projectile.timeLeft < 40)
                    Projectile.tileCollide = true;
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<BileDebuff>(), 180);
            Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D fish = ModContent.Request<Texture2D>("Redemption/NPCs/Lab/Blisterface/BlisteredFish").Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            int height = fish.Height / 6;
            Rectangle fishRect = new(0, 0, fish.Width, height);
            Vector2 fishDrawOrigin = new(fish.Width / 2, height / 2);
            float scale = 0;

            if (skrongle == 2)
            {
                Main.EntitySpriteDraw(fish, Projectile.Center - Main.screenPosition, new Rectangle?(fishRect), lightColor, Projectile.rotation, fishDrawOrigin, 1, SpriteEffects.None, 0);
                scale = 0.5f;
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale + scale, SpriteEffects.None, 0);
            return false;
        }
    }
}