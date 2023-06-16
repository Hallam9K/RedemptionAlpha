using Microsoft.Xna.Framework;
using Redemption.Effects.PrimitiveTrails;
using Redemption.Globals;
using System.Collections.Generic;
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
            On_Main.DrawProjectiles += Main_DrawProjectiles;
            On_Main.DrawCachedProjs += Main_DrawCachedProjs;
            On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float += TrailCheck;
            On_Main.DrawDust += AdditiveCalls;
            On_Player.CheckForGoodTeleportationSpot += DontTeleport;
        }
        private static void AdditiveCalls(On_Main.orig_DrawDust orig, Main self)
        {
            AdditiveCallManager.DrawAdditiveCalls(Main.spriteBatch);
            orig(self);
        }
        public static void Unload()
        {
            On_Main.DrawProjectiles -= Main_DrawProjectiles;
        }
        private static Vector2 DontTeleport(On_Player.orig_CheckForGoodTeleportationSpot orig, Player self, ref bool canSpawn, int teleportStartX, int teleportRangeX, int teleportStartY, int teleportRangeY, Player.RandomTeleportationAttemptSettings settings)
        {
            Vector2 result = orig(self, ref canSpawn, teleportStartX, teleportRangeX, teleportStartY, teleportRangeY, settings);

            // If invalid spot, recurse untill a valid one is found
            if (RedeTileHelper.CannotTeleportInFront[Framing.GetTileSafely((int)result.X, (int)result.Y).WallType])
            {
                settings.attemptsBeforeGivingUp--;
                result = self.CheckForGoodTeleportationSpot(ref canSpawn, teleportStartX, teleportRangeX, teleportStartY, teleportRangeY, settings);
            }
            return result;
        }
        private static void Main_DrawCachedProjs(On_Main.orig_DrawCachedProjs orig, Main self, List<int> projCache, bool startSpriteBatch)
        {
            if (!Main.dedServ && projCache == Main.instance.DrawCacheProjsBehindNPCs)
                Redemption.TrailManager.DrawTrails(Main.spriteBatch, TrailLayer.UnderCachedProjsBehindNPC);

            orig(self, projCache, startSpriteBatch);
        }
        private static void Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            if (!Main.dedServ)
                Redemption.TrailManager.DrawTrails(Main.spriteBatch, TrailLayer.UnderProjectile);

            orig(self);

            if (!Main.dedServ)
                Redemption.TrailManager.DrawTrails(Main.spriteBatch, TrailLayer.AboveProjectile);
        }
        private static int TrailCheck(On_Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float orig, IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1, float ai2)
        {
            int index = orig(spawnSource, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1, ai2);
            Projectile projectile = Main.projectile[index];

            if (projectile.ModProjectile is ITrailProjectile)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                    (projectile.ModProjectile as ITrailProjectile).DoTrailCreation(Redemption.TrailManager);

                else
                    Redemption.WriteToPacket(Redemption.Instance.GetPacket(), (byte)ModMessageType.SpawnTrail, index).Send();
            }
            return index;
        }
    }
}
