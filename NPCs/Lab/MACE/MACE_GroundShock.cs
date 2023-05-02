using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.WorldGeneration;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.MACE
{
    public class MACE_GroundShock : ModProjectile
    {
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
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!target.noKnockback)
                target.velocity.Y -= 4;
            target.AddBuff(BuffID.Electrified, 60);
            target.AddBuff(ModContent.BuffType<StunnedDebuff>(), 60);
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.Kill();
            }
            Projectile.hostile = false;
            if (Projectile.frame >= 1 && Projectile.frame <= 3)
                Projectile.hostile = true;

            switch (Projectile.ai[0])
            {
                case 0:
                    if (Projectile.localAI[0]++ == 1 && Projectile.Center.X < (RedeGen.LabVector.X + 103) * 16 && Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(Projectile.width, 0), Vector2.Zero, Type, Projectile.damage, 0, Main.myPlayer, Projectile.ai[0]);
                    break;
                case 1:
                    if (Projectile.localAI[0]++ == 1 && Projectile.Center.X > (RedeGen.LabVector.X + 43) * 16 && Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - new Vector2(Projectile.width, 0), Vector2.Zero, Type, Projectile.damage, 0, Main.myPlayer, Projectile.ai[0]);
                    break;
            }
        }
        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
    }
}
