using Redemption.Effects.PrimitiveTrails;
using Redemption.Globals.ILEdits;
using System;
using System.Collections.Generic;
using System.Linq;
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
            On.Terraria.Main.DrawCachedProjs += Main_DrawCachedProjs;
            On.Terraria.Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float += TrailCheck;
            On.Terraria.Main.DrawDust += AdditiveCalls;

            foreach (var item in typeof(RedeDetours).Assembly.GetTypes().Where(x => typeof(ILEdit).IsAssignableFrom(x) && !x.IsAbstract))
            {
                var inst = (ILEdit)Activator.CreateInstance(item);
                inst.Load(Redemption.Instance);
            }
        }
        private static void AdditiveCalls(On.Terraria.Main.orig_DrawDust orig, Main self)
        {
            AdditiveCallManager.DrawAdditiveCalls(Main.spriteBatch);
            orig(self);
        }
        public static void Unload()
        {
            On.Terraria.Main.DrawProjectiles -= Main_DrawProjectiles;
        }
        private static void Main_DrawCachedProjs(On.Terraria.Main.orig_DrawCachedProjs orig, Main self, List<int> projCache, bool startSpriteBatch)
        {
            if (!Main.dedServ && projCache == Main.instance.DrawCacheProjsBehindNPCs)
                Redemption.TrailManager.DrawTrails(Main.spriteBatch, TrailLayer.UnderCachedProjsBehindNPC);

            orig(self, projCache, startSpriteBatch);
        }
        private static void Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            if (!Main.dedServ)
                Redemption.TrailManager.DrawTrails(Main.spriteBatch, TrailLayer.UnderProjectile);

            orig(self);

            if (!Main.dedServ)
                Redemption.TrailManager.DrawTrails(Main.spriteBatch, TrailLayer.AboveProjectile);
        }
        private static int TrailCheck(On.Terraria.Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float orig, IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1)
        {
            int index = orig(spawnSource, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1);
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
