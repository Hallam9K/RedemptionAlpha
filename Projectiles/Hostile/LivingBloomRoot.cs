using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.NPCs.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Hostile
{
    public class LivingBloomRoot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Living Root");
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
            ElementID.ProjNature[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 28;
            Projectile.hostile = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.Redemption().friendlyHostile = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.velocity.Length() != 0 && target.type != ModContent.NPCType<LivingBloom>() && target.type != ModContent.NPCType<ForestNymph>() ? null : false;
        }
        public override bool CanHitPlayer(Player target)
        {
            return Projectile.velocity.Length() != 0;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= 4;
        public override void AI()
        {
            if (Main.rand.NextBool(2) && Projectile.localAI[0] < 30)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, 2, DustID.DryadsWard, 0, 0);
                Main.dust[dust].velocity.Y -= 4f;
                Main.dust[dust].velocity.X *= 0f;
                Main.dust[dust].noGravity = true;
            }
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] < 30)
                Projectile.alpha -= 10;
            else if (Projectile.localAI[0] == 30)
            {
                SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
                Projectile.velocity.Y -= 3;
            }
            else if (Projectile.localAI[0] == 40)
                Projectile.velocity.Y = 0;
            else if (Projectile.localAI[0] > 45)
            {
                Projectile.alpha += 10;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
        }
    }
}