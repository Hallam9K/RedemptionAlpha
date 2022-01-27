using Redemption.Effects;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Redemption.Globals.RedeNet;

namespace Redemption
{
    public static class RedeDetours
    {
        public static Dictionary<int, (Entity entity, IProjectileSource source)> projOwners = new();

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
            Entity attacker = null;

            if (spawnSource is ProjectileSource_Item && projectile.friendly && !projectile.hostile)
            {
                ProjectileSource_Item sourceItem = spawnSource as ProjectileSource_Item;
                attacker = sourceItem.Player;
            }
            else if (spawnSource is ProjectileSource_Buff && projectile.friendly && !projectile.hostile)
            {
                ProjectileSource_Buff sourceBuff = spawnSource as ProjectileSource_Buff;
                attacker = sourceBuff.Player;
            }
            else if (spawnSource is ProjectileSource_Item_WithAmmo && projectile.friendly && !projectile.hostile)
            {
                ProjectileSource_Item_WithAmmo sourceItemAmmo = spawnSource as ProjectileSource_Item_WithAmmo;
                attacker = sourceItemAmmo.Player;
            }
            else if (spawnSource is ProjectileSource_Mount && projectile.friendly && !projectile.hostile)
            {
                ProjectileSource_Mount sourceMount = spawnSource as ProjectileSource_Mount;
                attacker = sourceMount.Player;
            }
            else if (spawnSource is ProjectileSource_ProjectileParent && projectile.friendly && !projectile.hostile)
            {
                ProjectileSource_ProjectileParent sourceParent = spawnSource as ProjectileSource_ProjectileParent;
                attacker = Main.player[sourceParent.ParentProjectile.owner];
            }
            else if (spawnSource is ProjectileSource_NPC)
            {
                ProjectileSource_NPC sourceNPC = spawnSource as ProjectileSource_NPC;
                attacker = sourceNPC.NPC;
            }
            if (attacker != null)
            {
                if (projOwners.ContainsKey(index))
                    projOwners.Remove(index);
                projOwners.Add(index, (attacker, spawnSource));
            }

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
