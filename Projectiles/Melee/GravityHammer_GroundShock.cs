using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Projectiles.Melee
{
    public class GravityHammer_GroundShock : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Lab/MACE/MACE_GroundShock";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Electric Eruption");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 102;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.Redemption().IsHammer = true;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.knockBackResist != 0)
                target.velocity.Y -= 8 * target.knockBackResist;
            if (Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 60);
            if (Main.rand.NextBool(5) && target.knockBackResist > 0)
                target.AddBuff(ModContent.BuffType<StunnedDebuff>(), 60);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Projectile.ai[0] > target.Center.X ? -1 : 1;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.Kill();
            }
            Projectile.friendly = false;
            if (Projectile.frame >= 1 && Projectile.frame <= 3)
                Projectile.friendly = true;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
    }
}
