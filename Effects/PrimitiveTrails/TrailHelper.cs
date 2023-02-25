using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Weapons.PostML.Melee;
using Redemption.NPCs.Bosses.ADD;
using Redemption.NPCs.Bosses.Gigapora;
using Redemption.NPCs.Bosses.Keeper;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.NPCs.Bosses.Neb.Phase2;
using Redemption.NPCs.Bosses.Neb;
using Redemption.NPCs.Friendly;
using Redemption.Projectiles.Melee;
using Redemption.Projectiles.Minions;
using Redemption.Projectiles.Ranged;
using static Redemption.Globals.RedeNet;

namespace Redemption.Effects.PrimitiveTrails
{
    public enum TrailLayer
    {
        UnderProjectile,
        UnderCachedProjsBehindNPC,
        AboveProjectile
    }

    public class TrailManager
    {
        private readonly List<BaseTrail> _trails = new();
        private readonly Effect _effect;

        private BasicEffect _basicEffect; //Not readonly due to thread queue

        public TrailManager(Mod mod)
        {
            _trails = new List<BaseTrail>();
            _effect = ModContent.Request<Effect>("Redemption/Effects/trailShaders", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            Main.QueueMainThreadAction(() =>
            {
                _basicEffect = new BasicEffect(Main.graphics.GraphicsDevice)
                {
                    VertexColorEnabled = true
                };
            });
        }

        public static void TryTrailKill(Projectile projectile)
        {
            if (projectile.type == ModContent.ProjectileType<KeeperDreadCoil>() || projectile.type == ModContent.ProjectileType<EaglecrestSling_Proj>() || projectile.type == ModContent.ProjectileType<CrystalGlaive_Proj>() || projectile.type == ModContent.ProjectileType<InfectiousGlaive_Proj>() || projectile.type == ModContent.ProjectileType<XeniumLance_Proj>() || projectile.type == ModContent.ProjectileType<DualcastBall>() || projectile.type == ModContent.ProjectileType<UkkoThunderwave>() || projectile.type == ModContent.ProjectileType<Gigapora_Fireball>() || projectile.type == ModContent.ProjectileType<ShieldCore_Bolt>() || projectile.type == ModContent.ProjectileType<ShieldCore_DualcastBall>() || projectile.type == ModContent.ProjectileType<ShadowBolt>() || projectile.type == ModContent.ProjectileType<KS3_EnergyBolt>() || projectile.type == ModContent.ProjectileType<CurvingStar>() || projectile.type == ModContent.ProjectileType<CurvingStar2>() || projectile.type == ModContent.ProjectileType<Neb_Start_Visual>() || projectile.type == ModContent.ProjectileType<StarFall_Proj2>() || projectile.type == ModContent.ProjectileType<StarFall_Proj>() || projectile.type == ModContent.ProjectileType<WraithSlayer_Slash>() || projectile.type == ModContent.ProjectileType<NebulaStar>() || projectile.type == ModContent.ProjectileType<NebulaSpark>() || projectile.type == ModContent.ProjectileType<MicroshieldCore_Bolt>())
                Redemption.TrailManager.TryEndTrail(projectile, Math.Max(15f, projectile.velocity.Length() * 3f));
            switch (projectile.type)
            {
                case ProjectileID.WoodenArrowFriendly:
                    Redemption.TrailManager.TryEndTrail(projectile, Math.Max(15f, projectile.velocity.Length() * 3f));
                    break;
            }
        }

        public void CreateTrail(Projectile projectile, ITrailColor trailType, ITrailCap trailCap, ITrailPosition trailPosition, float widthAtFront, float maxLength, ITrailShader shader = null, TrailLayer layer = TrailLayer.UnderProjectile, float dissolveSpeed = -1)
        {
            var newTrail = new Trail(projectile, trailType, trailCap, trailPosition, shader ?? new DefaultShader(), layer, widthAtFront, maxLength, dissolveSpeed);
            newTrail.BaseUpdate();
            _trails.Add(newTrail);
        }

        public void CreateCustomTrail(BaseTrail trail)
        {
            trail.BaseUpdate();
            _trails.Add(trail);
        }

        public void UpdateTrails()
        {
            for (int i = 0; i < _trails.Count; i++)
            {
                BaseTrail trail = _trails[i];

                trail.BaseUpdate();
                if (trail.Dead)
                {
                    _trails.RemoveAt(i);
                    i--;
                }
            }
        }

        public void ClearAllTrails() => _trails.Clear();

        public void DrawTrails(SpriteBatch spriteBatch, TrailLayer layer)
        {
            foreach (BaseTrail trail in _trails)
            {
                if (trail.Layer == layer)
                    trail.Draw(_effect, spriteBatch.GraphicsDevice);
            }
        }

        public void TryEndTrail(Projectile projectile, float dissolveSpeed)
        {
            for (int i = 0; i < _trails.Count; i++)
            {
                BaseTrail trail = _trails[i];

                if (trail.MyProjectile.whoAmI == projectile.whoAmI && trail is Trail t)
                {
                    t.DissolveSpeed = dissolveSpeed;
                    t.StartDissolve();
                    return;
                }
            }
        }

        public static void ManualTrailSpawn(Projectile projectile)
        {
            if (projectile.ModProjectile is IManualTrailProjectile)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                    (projectile.ModProjectile as IManualTrailProjectile).DoTrailCreation(Redemption.TrailManager);

                else
                    Redemption.WriteToPacket(Redemption.Instance.GetPacket(), (byte)ModMessageType.SpawnTrail, projectile.whoAmI).Send();
            }
        }
    }

    public abstract class BaseTrail
    {
        public bool Dead { get; set; } = false;
        public Projectile MyProjectile { get; set; }
        public TrailLayer Layer { get; set; }

        private readonly int _originalProjectileType;
        private bool _dissolving = false;

        public BaseTrail(Projectile projectile, TrailLayer layer)
        {
            MyProjectile = projectile;
            Layer = layer;
            _originalProjectileType = projectile.type;
        }

        public void BaseUpdate()
        {
            if ((!MyProjectile.active || MyProjectile.type != _originalProjectileType) && !_dissolving)
                StartDissolve();

            if (_dissolving)
                Dissolve();
            else
                Update();
        }

        public void StartDissolve()
        {
            OnStartDissolve();
            _dissolving = true;
        }

        /// <summary>
        /// Behavior for the trail every tick, only called before the trail begins dying
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Behavior for the trail after it starts its death, called every tick after the trail begins dying
        /// </summary>
        public virtual void Dissolve() { }

        /// <summary>
        /// Additional behavior for the trail upon starting its death
        /// </summary>
        public virtual void OnStartDissolve() { }

        /// <summary>
        /// How the trail is drawn to the screen
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="effect2"></param>
        /// <param name="device"></param>
        public virtual void Draw(Effect effect, GraphicsDevice device) { }
    }
}