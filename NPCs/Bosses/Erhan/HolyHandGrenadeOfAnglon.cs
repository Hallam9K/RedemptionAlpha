using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class HolyHandGrenadeOfAnglon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Hand Grenade of Anglon");
            Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 36;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
        }
        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0)
            {
                RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120), 0.2f, 0.85f, 4);
                RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120), 0.2f);
            }
            if (Projectile.localAI[0] == 136)
            {
                SoundEngine.PlaySound(SoundID.Tink);
                Projectile.frame = 1;
                if (Main.netMode != NetmodeID.Server)
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(13, 2), RedeHelper.SpreadUp(5),
                        ModContent.Find<ModGore>("Redemption/HolyGrenadePin").Type);
            }
            if (Projectile.localAI[0] == 338)
            {
                Projectile.alpha = 255;
                SoundEngine.PlaySound(SoundID.Item14);
                RedeDraw.SpawnExplosion(Projectile.Center, new Color(255, 216, 0), DustID.GoldFlame, 20, 30, 3);
            }
            if (Projectile.localAI[0] >= 398)
                Projectile.Kill();
        }
    }
}
