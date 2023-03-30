using Microsoft.Xna.Framework;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class DrillRevolver : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Converts bullets into drill bits that shred through enemies and tiles\n" +
                "Inflicts Broken Armor, piercing through Guard Points"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 82;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 44;
            Item.height = 24;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1;
            Item.value = Item.sellPrice(0, 8, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item40;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DrillRevolver_Bullet>();
            Item.shootSpeed = 7;
            Item.useAmmo = AmmoID.Bullet;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<OmegaPowerCell>())
                .AddIngredient(ModContent.ItemType<CorruptedXenomite>(), 7)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 4)
                .AddIngredient(ModContent.ItemType<Plating>(), 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<DrillRevolver_Bullet>();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
