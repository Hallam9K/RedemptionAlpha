using Redemption.Effects;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Redemption.Globals.RedeNet;

namespace Redemption
{
	public static class RedeDetours
	{
		public static void Initialize()
		{
			On.Terraria.Main.DrawProjectiles += Main_DrawProjectiles;
			On.Terraria.Projectile.NewProjectile_IProjectileSource_float_float_float_float_int_int_float_int_float_float += Projectile_NewProjectile;
		}

		public static void Unload()
		{
			On.Terraria.Main.DrawProjectiles -= Main_DrawProjectiles;
			On.Terraria.Projectile.NewProjectile_IProjectileSource_float_float_float_float_int_int_float_int_float_float -= Projectile_NewProjectile;
		}

		private static void Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
		{
			if (!Main.dedServ)
				RedeSystem.TrailManager.DrawTrails(Main.spriteBatch);

			orig(self);
		}

		private static int Projectile_NewProjectile(On.Terraria.Projectile.orig_NewProjectile_IProjectileSource_float_float_float_float_int_int_float_int_float_float orig, IProjectileSource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1)
		{
			int index = orig(spawnSource, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1);
			Projectile projectile = Main.projectile[index];

			if (projectile.ModProjectile is ITrailProjectile)
			{
				if (Main.netMode == NetmodeID.SinglePlayer)
					(projectile.ModProjectile as ITrailProjectile).DoTrailCreation(RedeSystem.TrailManager);

				else
					Redemption.WriteToPacket(Redemption.Instance.GetPacket(), (byte)ModMessageType.SpawnTrail, index).Send();
			}
			return index;
		}
	}
}
