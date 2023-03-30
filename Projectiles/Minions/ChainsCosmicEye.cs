using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class ChainsCosmicEye : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/CosmicEye";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Eye");
        }
        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 46;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc.active)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.velocity = Projectile.DirectionTo(npc.Center) * 10;
            }
            else if (Projectile.localAI[0] == 0)
                Projectile.localAI[0] = 1;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.alpha -= 10;
                if (Projectile.alpha <= 0)
                {
                    SoundEngine.PlaySound(SoundID.Item125, Projectile.position);
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<ChainsCosmicEye_Beam>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, Projectile.whoAmI);
                    Projectile.localAI[0] = 1;
                }
            }
            else
            {
                Projectile.localAI[0]++;
                if (Projectile.localAI[0] >= 30)
                {
                    Projectile.alpha += 12;
                    if (Projectile.alpha >= 255)
                        Projectile.Kill();
                }
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override bool ShouldUpdatePosition() => false;
    }
}