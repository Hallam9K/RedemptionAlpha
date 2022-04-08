using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Projectiles.Minions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class InsulatiumPotionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Insulated");
            Description.SetDefault("\"You are immune to Electrified\"");
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().zapField = true;
            player.buffImmune[BuffID.Electrified] = true;
            int projType = ModContent.ProjectileType<ZapField_Proj>();
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
                Projectile.NewProjectile(player.GetProjectileSource_Buff(buffIndex), player.Center, Vector2.Zero, projType, 500, 0f, player.whoAmI);
        }
    }
    public class ZapField_Proj : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electricity Field");
        }
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 220;
        }
        NPC target;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.RedemptionPlayerBuff().zapField || player.dead || !player.active)
                Projectile.Kill();
            Projectile.Center = player.Center;
            if (RedeHelper.ClosestNPC(ref target, 300, Projectile.Center, false, player.MinionAttackTargetNPC))
            {
                if (Projectile.localAI[0]++ % 50 == 0 && Main.myPlayer == Projectile.owner)
                {
                    Vector2 ai = RedeHelper.PolarVector(10, (target.Center - Projectile.Center).ToRotation());
                    float ai2 = Main.rand.Next(100);
                    int p = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, RedeHelper.PolarVector(18, (target.Center - Projectile.Center).ToRotation()), ModContent.ProjectileType<TeslaGenerator_Lightning>(), Projectile.damage, 0, Main.myPlayer, ai.ToRotation(), ai2);
                    Main.projectile[p].DamageType = DamageClass.Generic;
                    Main.projectile[p].netUpdate2 = true;
                }
            }
        }
    }
}