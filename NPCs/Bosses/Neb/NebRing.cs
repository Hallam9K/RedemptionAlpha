using Redemption.NPCs.Bosses.Neb.Clone;
using Redemption.NPCs.Bosses.Neb.Phase2;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb
{
    public class NebRing : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Ring");
        }
        public override void SetDefaults()
        {
            Projectile.width = 408;
            Projectile.height = 408;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 254;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || (npc.type != NPCType<Nebuleus>() && npc.type != NPCType<Nebuleus_Clone>() && npc.type != NPCType<Nebuleus2>() && npc.type != NPCType<Nebuleus2_Clone>()))
                Projectile.Kill();
            Projectile.Center = npc.Center;
            Projectile.velocity = Vector2.Zero;

            Projectile.localAI[0]++;
            Projectile.rotation += 0.04f;
            if (Projectile.localAI[0] <= 120)
                Projectile.alpha -= 2;
            if (Projectile.localAI[0] >= 200)
                Projectile.alpha += 4;

            if (Projectile.alpha >= 255)
                Projectile.Kill();
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}