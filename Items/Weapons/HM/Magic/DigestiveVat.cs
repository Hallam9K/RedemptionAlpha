using Microsoft.Xna.Framework;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class DigestiveVat : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Fires an arc of digestive fluid at the enemy\n" +
                "Inflicts a defense-reducing stomach acid debuff"); */
            Item.staff[Item.type] = true;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 57;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 6;
            Item.useAnimation = 24;
            Item.reuseDelay = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 19;
            Item.shoot = ModContent.ProjectileType<DigestiveVat_Proj>();
            Item.UseSound = SoundID.NPCHit20;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = 2;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(10));
                float scale = 1f - (Main.rand.NextFloat() * 0.4f);
                perturbedSpeed *= scale;
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BottledWater)
            .AddIngredient(ModContent.ItemType<ToxicBile>(), 6)
            .AddTile(TileID.Bottles)
            .Register();
        }
    }
}