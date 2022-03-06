using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            On.Terraria.Main.DrawDust += Main_DrawDust;
        }
        public static void Unload()
        {
            On.Terraria.Main.DrawProjectiles -= Main_DrawProjectiles;
            On.Terraria.Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float -= Projectile_NewProjectile;
            On.Terraria.Main.DrawDust -= Main_DrawDust;
        }
        private static void Main_DrawDust(On.Terraria.Main.orig_DrawDust orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            ParticleManager.PreUpdate(Main.spriteBatch);
            ParticleManager.Update(Main.spriteBatch);
            ParticleManager.PostUpdate(Main.spriteBatch);
            Main.spriteBatch.End();
            orig(self);
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
        public static void NewParticle(Vector2 Position, Vector2 Velocity, Particle Type, Color Color, float Scale, float AI0 = 0, float AI1 = 0, float AI2 = 0, float AI3 = 0, float AI4 = 0, float AI5 = 0, float AI6 = 0, float AI7 = 0)
        {

        }
    }
}
