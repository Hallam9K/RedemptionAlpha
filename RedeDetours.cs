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
        public static Dictionary<int, (Entity entity, IProjectileSource source)> projOwners = new();

        public static void Initialize()
        {
            On.Terraria.Main.DrawProjectiles += Main_DrawProjectiles;
            On.Terraria.Projectile.NewProjectile_IProjectileSource_float_float_float_float_int_int_float_int_float_float += Projectile_NewProjectile;
            On.Terraria.Main.DrawDust += Main_DrawDust;
            Main.OnResolutionChanged += Main_OnResolutionChanged;
        }
        public static void Unload()
        {
            On.Terraria.Main.DrawProjectiles -= Main_DrawProjectiles;
            On.Terraria.Projectile.NewProjectile_IProjectileSource_float_float_float_float_int_int_float_int_float_float -= Projectile_NewProjectile;
            On.Terraria.Main.DrawDust -= Main_DrawDust;
            Main.OnResolutionChanged -= Main_OnResolutionChanged;
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
        private static void Main_OnResolutionChanged(Vector2 obj)
        {
            int width = Main.graphics.GraphicsDevice.Viewport.Width;
            int height = Main.graphics.GraphicsDevice.Viewport.Height;
            Vector2 zoom = Main.GameViewMatrix.Zoom;
            Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
            Matrix projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            RedeSystem.TrailManager.worldViewProjection = view * projection;
        }
        public static void NewParticle(Vector2 Position, Vector2 Velocity, Particle Type, Color Color, float Scale, float AI0 = 0, float AI1 = 0, float AI2 = 0, float AI3 = 0, float AI4 = 0, float AI5 = 0, float AI6 = 0, float AI7 = 0)
        {

        }
    }
}
