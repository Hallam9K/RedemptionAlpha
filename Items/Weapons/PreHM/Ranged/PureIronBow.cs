using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class PureIronBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 24;
            Item.height = 58;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 19;
            Item.knockBack = 1;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 14f;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.useAmmo = AmmoID.Arrow;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PureIronAlloy>(), 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
                type = ProjectileID.FrostburnArrow;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.IceMist with { Volume = .4f, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew }, player.position);
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.GetGlobalProjectile<PureIronBowGlobal>().ShotFrom = true;
            return false;
        }
    }
    public class PureIronBowGlobal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool ShotFrom;
        public int mistTimer;
        public override void PostAI(Projectile projectile)
        {
            if (!ShotFrom)
                return;
            if (mistTimer++ % 3 == 0 && Main.rand.NextBool(2) && Main.myPlayer == projectile.owner && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<Icefall_Mist>()] < 500)
            {
                Projectile.NewProjectile(Projectile.InheritSource(projectile), projectile.Center, new Vector2(Main.rand.NextFloat(-1, 1), 0), ModContent.ProjectileType<Icefall_Mist>(), 0, 0, projectile.owner, 1, 1);
            }
        }
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (!ShotFrom)
                return;
            SoundEngine.PlaySound(CustomSounds.CrystalHit, projectile.position);
        }
    }
}