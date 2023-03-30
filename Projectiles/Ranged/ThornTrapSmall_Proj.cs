using Microsoft.Xna.Framework;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class ThornTrapSmall_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Thorn Trap");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
            ElementID.ProjNature[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 1)
                Projectile.DamageType = DamageClass.Melee;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                {
                    Projectile.frame = 5;
                }
            }
            Projectile.localAI[0]++;
            Projectile.velocity.Y++;

            if (Projectile.localAI[0] >= 160)
                Projectile.alpha += 10;
            if (Projectile.alpha >= 255)
                Projectile.Kill();
        }
        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.frame >= 3 ? null : false;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0;
            return false;
        }
    }
}