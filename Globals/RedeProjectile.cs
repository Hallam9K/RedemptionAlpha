using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Terraria.DataStructures;
using System.Collections.Generic;
using Redemption.Effects.PrimitiveTrails;
using Terraria.ID;
using static Redemption.Globals.RedeNet;
using System.Linq;

namespace Redemption.Globals
{
    public class RedeProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool TechnicallyMelee;
        public bool IsHammer;
        public bool IsAxe;
        public bool Unparryable;
        public bool RitDagger;
        public override void SetDefaults(Projectile projectile)
        {
            if (ProjectileLists.IsTechnicallyMelee.Contains(projectile.type))
                TechnicallyMelee = true;
        }

        private readonly int[] bannedArenaProjs = new int[]
        {
            ProjectileID.SandBallGun,
            ProjectileID.EbonsandBallGun,
            ProjectileID.PearlSandBallGun,
            ProjectileID.CrimsandBallGun,
            ProjectileID.SandBallFalling,
            ProjectileID.EbonsandBallFalling,
            ProjectileID.PearlSandBallFalling,
            ProjectileID.CrimsandBallFalling,
            ProjectileID.Bomb,
            ProjectileID.StickyBomb,
            ProjectileID.BouncyBomb,
            ProjectileID.Dynamite,
            ProjectileID.StickyDynamite,
            ProjectileID.BouncyDynamite,
            ProjectileID.SnowBallHostile,
            ProjectileID.IceBlock,
            ProjectileID.AntiGravityHook,
            ProjectileID.StaticHook,
            ProjectileID.PortalGunBolt,
            ProjectileID.PortalGunGate
        };
        public override void AI(Projectile projectile)
        {
            if (ArenaWorld.arenaActive && bannedArenaProjs.Any(x => x == projectile.type) && projectile.Hitbox.Intersects(new Rectangle((int)ArenaWorld.arenaTopLeft.X, (int)ArenaWorld.arenaTopLeft.Y, (int)ArenaWorld.arenaSize.X, (int)ArenaWorld.arenaSize.Y)))
                projectile.Kill();

            if (ArenaWorld.arenaActive && projectile.aiStyle == 7 && !projectile.Hitbox.Intersects(new Rectangle((int)ArenaWorld.arenaTopLeft.X, (int)ArenaWorld.arenaTopLeft.Y, (int)ArenaWorld.arenaSize.X, (int)ArenaWorld.arenaSize.Y)))
                projectile.Kill();
        }
        public override void ModifyHitNPC(Projectile projectile, Terraria.NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (IsAxe && crit)
                damage += damage / 2;
        }
        public static void Decapitation(Terraria.NPC target, ref int damage, ref bool crit, int chance = 200)
        {
            if (target.life < target.lifeMax && NPCLists.SkeletonHumanoid.Contains(target.type))
            {
                if (Main.rand.NextBool(chance))
                {
                    CombatText.NewText(target.getRect(), Color.Orange, "Decapitated!");
                    target.Redemption().decapitated = true;
                    damage = damage < target.life ? target.life : damage;
                    crit = true;
                }
            }
        }
        public static Dictionary<int, (Entity entity, IEntitySource source)> projOwners = new();
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Entity attacker = null;
            if (source is EntitySource_ItemUse && projectile.friendly && !projectile.hostile)
            {
                EntitySource_ItemUse sourceItem = source as EntitySource_ItemUse;
                attacker = sourceItem.Entity;
            }
            else if (source is EntitySource_Buff && projectile.friendly && !projectile.hostile)
            {
                EntitySource_Buff sourceBuff = source as EntitySource_Buff;
                attacker = sourceBuff.Entity;
            }
            else if (source is EntitySource_ItemUse_WithAmmo && projectile.friendly && !projectile.hostile)
            {
                EntitySource_ItemUse_WithAmmo sourceItemAmmo = source as EntitySource_ItemUse_WithAmmo;
                attacker = sourceItemAmmo.Entity;
            }
            else if (source is EntitySource_Mount && projectile.friendly && !projectile.hostile)
            {
                EntitySource_Mount sourceMount = source as EntitySource_Mount;
                attacker = sourceMount.Entity;
            }
            else if (source is EntitySource_Parent)
            {
                EntitySource_Parent sourceParent = source as EntitySource_Parent;
                if (sourceParent.Entity is Projectile)
                    attacker = Main.player[(sourceParent.Entity as Projectile).owner];
                else
                    attacker = sourceParent.Entity;
            }
            if (attacker != null)
            {
                if (projOwners.ContainsKey(projectile.whoAmI))
                    projOwners.Remove(projectile.whoAmI);
                projOwners.Add(projectile.whoAmI, (attacker, source));
            }

            if (projectile.ModProjectile is ITrailProjectile)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                    (projectile.ModProjectile as ITrailProjectile).DoTrailCreation(RedeSystem.TrailManager);

                else
                    Redemption.WriteToPacket(Redemption.Instance.GetPacket(), (byte)ModMessageType.SpawnTrail, projectile.whoAmI).Send();
            }
        }
    }
    public abstract class TrueMeleeProjectile : ModProjectile
    {
        public float SetSwingSpeed(float speed)
        {
            Terraria.Player player = Main.player[Projectile.owner];
            return speed / player.GetAttackSpeed(DamageClass.Melee);
        }

        public virtual void SetSafeDefaults() { }

        public override void SetDefaults()
        {
            SetSafeDefaults();
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.Redemption().Unparryable = true;
            Projectile.Redemption().TechnicallyMelee = true;
        }
    }
}