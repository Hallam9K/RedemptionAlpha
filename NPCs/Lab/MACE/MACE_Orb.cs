﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.MACE
{
    public class MACE_Orb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xenium Orb");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Lighting.AddLight(Projectile.Center, 0f, Projectile.Opacity * 0.3f, 0f);
            if (Projectile.localAI[0]++ == 30)
                Projectile.velocity.X = 0;

            if (Projectile.localAI[0] == 60 && !Main.dedServ)
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Alarm2").WithVolume(.5f), Projectile.position);

            if (Projectile.localAI[0] >= 60 && Projectile.localAI[0] % 4 == 0 && Projectile.localAI[0] <= 80 && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), new Vector2(Projectile.Center.X, Projectile.Center.Y + 77), Vector2.Zero, ModContent.ProjectileType<MACE_OrbLaser_Tele>(), 0, 0, Main.myPlayer);
            }
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Zap1").WithVolume(.5f).WithPitchVariance(0.1f), Projectile.position);
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), new Vector2(Projectile.Center.X, Projectile.Center.Y + 47), Vector2.Zero, ModContent.ProjectileType<MACE_OrbLaser>(), 34, 3, Main.myPlayer);
        }
    }
}