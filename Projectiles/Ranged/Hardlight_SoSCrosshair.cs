using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Cooldowns;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class Hardlight_SoSCrosshair : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crosshair");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Ranged;
        }
        NPC target;
        NPC locked;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Projectile.rotation += 0.05f;
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || Projectile.localAI[0] >= 5)
                Projectile.Kill();

            switch (Projectile.ai[1])
            {
                case 0:
                    if (RedeHelper.ClosestNPC(ref target, 1400, Projectile.Center, true, player.MinionAttackTargetNPC))
                    {
                        locked = Main.npc[target.whoAmI];
                        Projectile.ai[1] = 1;
                    }
                    else
                    {
                        CombatText.NewText(player.getRect(), Color.Cyan, "No targets found!");
                        player.ClearBuff(ModContent.BuffType<HardlightCooldown>());
                        Projectile.Kill();
                    }
                    break;
                case 1:
                    if (!locked.active)
                        Projectile.Kill();

                    Projectile.Center = locked.Center;
                    Projectile.alpha = 0;

                    Projectile.localAI[1]++;
                    if (Projectile.localAI[1] >= 30 && Projectile.localAI[1] % 5 == 0 && Projectile.localAI[1] < 60 && Projectile.owner == Main.myPlayer)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.MissileFire1, player.position);

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(player.Center.X + Main.rand.Next(-200, 201), player.Center.Y - 800), RedeHelper.PolarVector(14, (locked.Center - Projectile.Center).ToRotation()), ModContent.ProjectileType<Hardlight_SoSMissile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI, 1);
                    }
                    if (Projectile.localAI[1] >= 60 && !RedeHelper.AnyProjectiles(ModContent.ProjectileType<Hardlight_SoSMissile>()))
                        Projectile.Kill();
                    break;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}