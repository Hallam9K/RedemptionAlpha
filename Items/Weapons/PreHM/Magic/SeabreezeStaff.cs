using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Particles;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class SeabreezeStaff : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.WaterS);
        public override void SetStaticDefaults()
        {
            ElementID.ItemWater[Type] = true;
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.width = 46;
            Item.height = 40;
            Item.useTime = 5;
            Item.useAnimation = 15;
            Item.useLimitPerAnimation = 3;
            Item.reuseDelay = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = 700;
            Item.channel = true;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = CustomSounds.WindLong.WithPitchOffset(1f).WithVolumeScale(.4f) with { MaxInstances = 3 };
            Item.autoReuse = true;
            Item.shoot = ProjectileType<WindGust_Proj>();
            Item.shootSpeed = 3f;

            Item.Redemption().HideElementTooltip[ElementID.Water] = true;
        }
        private int CastCount;
        public override void HoldItem(Player player)
        {
            if (!player.channel)
                CastCount = 0;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 150f;
            if (Collision.CanHit(position, 0, 0, position - Offset, 0, 0))
            {
                position -= Offset;
            }

            position.Y -= 40;

            Color speedColor = Color.White;
            if (CastCount >= 6)
                speedColor = Color.LightBlue;

            for (int i = 0; i < 5; i++)
            {
                RedeParticleManager.CreateSpeedParticle(position + (Vector2.Normalize(velocity) * 50f) + RedeHelper.Spread(60), RedeHelper.PolarVector(Main.rand.Next(30, 61), (Main.MouseWorld - player.Center).ToRotation() + Main.rand.NextFloat(-0.05f, .05f)), 1f, speedColor, .92f, 40, extension: 150);
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int aiType = 0;
            CastCount++;
            if (CastCount >= 7)
            {
                aiType = 1;
                if (CastCount >= 9)
                    CastCount = 0;
            }


            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, Main.rand.Next(2), aiType);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.PalmWood, 15)
            .AddIngredient(ItemID.FallenStar, 5)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}