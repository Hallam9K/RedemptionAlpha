using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.Effects.PrimitiveTrails;
using Redemption.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Redemption.Globals.RedeNet;

namespace Redemption
{
    public static class RedeDetours
    {
        public static Dictionary<int, (Entity entity, IEntitySource source)> projOwners = new();

        public static void Initialize()
        {
            On.Terraria.Main.DrawProjectiles += Main_DrawProjectiles;
            On.Terraria.Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float += Projectile_NewProjectile;
        }
        public static void Unload()
        {
            On.Terraria.Main.DrawProjectiles -= Main_DrawProjectiles;
            On.Terraria.Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float -= Projectile_NewProjectile;
        }
        private static void Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            if (!Main.dedServ)
                RedeSystem.TrailManager.DrawTrails(Main.spriteBatch);

            orig(self);
        }
        private static int Projectile_NewProjectile(On.Terraria.Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float orig, IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1)
        {
            int index = orig(spawnSource, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1);

            Projectile projectile = Main.projectile[index];
            Entity attacker = null;

            if (spawnSource is EntitySource_ItemUse && projectile.friendly && !projectile.hostile)
            {
                EntitySource_ItemUse sourceItem = spawnSource as EntitySource_ItemUse;
                attacker = sourceItem.Entity;
            }
            else if (spawnSource is EntitySource_Buff && projectile.friendly && !projectile.hostile)
            {
                EntitySource_Buff sourceBuff = spawnSource as EntitySource_Buff;
                attacker = sourceBuff.Entity;
            }
            else if (spawnSource is EntitySource_ItemUse_WithAmmo && projectile.friendly && !projectile.hostile)
            {
                EntitySource_ItemUse_WithAmmo sourceItemAmmo = spawnSource as EntitySource_ItemUse_WithAmmo;
                attacker = sourceItemAmmo.Entity;
            }
            else if (spawnSource is EntitySource_Mount && projectile.friendly && !projectile.hostile)
            {
                EntitySource_Mount sourceMount = spawnSource as EntitySource_Mount;
                attacker = sourceMount.Entity;
            }
            else if (spawnSource is EntitySource_Parent)
            {
                EntitySource_Parent sourceParent = spawnSource as EntitySource_Parent;
                if (sourceParent.Entity is Projectile)
                    attacker = Main.player[(sourceParent.Entity as Projectile).owner];
                else
                    attacker = sourceParent.Entity;
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
