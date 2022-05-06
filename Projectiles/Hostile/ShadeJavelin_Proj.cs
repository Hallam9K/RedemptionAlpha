using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Dusts;
using Terraria.Audio;
using Redemption.BaseExtension;
using System.Collections.Generic;
using Redemption.Globals;
using Redemption.Base;

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
			if (Projectile.localAI[1] == 0)
			{
				Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
				Projectile.velocity.Y += 0.3f;
			}
			else
			{
				if (Projectile.localAI[1]++ >= 40)
					Projectile.Kill();
			}
			if (Projectile.ai[1] == 1)
			{
				int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.DungeonSpirit, Scale: 2);
				Main.dust[dust].velocity.Y = -2;
				Main.dust[dust].velocity.X = 0;
				Main.dust[dust].noGravity = true;
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
			if (Projectile.localAI[1] == 0)
			{
				SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact.WithVolume(0.5f), Projectile.position);
				Projectile.position += oldVelocity * 2;
				Projectile.localAI[1] = 1;
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
			if (Projectile.ai[1] == 1)
			{
				SoundEngine.PlaySound(SoundID.NPCDeath39.WithVolume(0.5f), Projectile.position);
				Color c = new(167, 255, 255);
				RedeDraw.SpawnRing(Projectile.Center, c, 0.2f, 0.86f, 8);
				for (int i = 0; i < 7; i++)
				{
					int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>());
					Main.dust[dust].velocity *= 6;
					Main.dust[dust].noGravity = true;
					Color dustColor = new(c.R, c.G, c.B) { A = 0 };
					Main.dust[dust].color = dustColor;
				}
				for (int i = 0; i < Projectile.ai[1]; i++)
				{
					int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 2);
					Main.dust[dust].velocity *= 6;
					Main.dust[dust].noGravity = true;
				}
				Rectangle boom = new((int)Projectile.Center.X - 75, (int)Projectile.Center.Y - 75, 150, 150);
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					Player target = Main.player[i];
					if (!target.active || target.dead)
						continue;

					if (!target.Hitbox.Intersects(boom))
						continue;

					BaseAI.DamagePlayer(target, Projectile.damage * 4, Projectile.knockBack, Projectile);
				}
				NPC host = Main.npc[(int)Projectile.ai[0]];
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC target = Main.npc[i];
					if (!target.active || !target.CanBeChasedBy() || target != host.Redemption().attacker)
						continue;

					if (target.immune[Projectile.whoAmI] > 0 || !target.Hitbox.Intersects(boom))
						continue;

					target.immune[Projectile.whoAmI] = 20;
					int hitDirection = Projectile.Center.X > target.Center.X ? -1 : 1;
					BaseAI.DamageNPC(target, Projectile.damage, Projectile.knockBack, hitDirection, Projectile);
				}
			}
		}
	}
}
