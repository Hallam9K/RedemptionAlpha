using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Dusts;
using Terraria.Audio;
using Redemption.BaseExtension;
using System.Collections.Generic;

namespace Redemption.Projectiles.Hostile
{
    public class ShadeJavelin_Proj : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shade Javelin");
            Main.projFrames[Projectile.type] = 3;
        }
        public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.hostile = true;
			Projectile.penetrate = 3;
			Projectile.hide = true;
		}
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			behindNPCsAndTiles.Add(index);
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
				Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
					Projectile.frame = 0;
            }
			if (Projectile.ai[1] == 0)
			{
				Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
				Projectile.velocity.Y += 0.3f;
			}
			else
			{
				if (Projectile.ai[1]++ >= 40)
					Projectile.Kill();
			}
		}
		public override bool? CanHitNPC(NPC target)
		{
			NPC host = Main.npc[(int)Projectile.ai[0]];
			return target == host.Redemption().attacker ? null : false;
		}
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => damage *= 4;
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
			width = height = 8;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.ai[1] == 0)
			{
				SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.position);
				Projectile.position += oldVelocity * 2;
				Projectile.ai[1] = 1;
			}
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.velocity *= 0;
            return false;
        }
        public override void Kill(int timeLeft)
		{
			Vector2 usePos = Projectile.position;
			Vector2 rotVector = (Projectile.rotation - MathHelper.ToRadians(90f)).ToRotationVector2();
			usePos += rotVector * 16f;
			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(usePos, Projectile.width, Projectile.height, ModContent.DustType<VoidFlame>());
				dust.position = (dust.position + Projectile.Center) / 2f;
				dust.velocity += rotVector * 2f;
				dust.velocity *= 0.5f;
				dust.noGravity = true;
				usePos -= rotVector * 8f;
			}
		}
	}
}
