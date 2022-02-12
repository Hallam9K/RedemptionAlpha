using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Items.Materials.PreHM;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Items.Weapons.PreHM.Melee;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class Midnight : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Midnight, Defiler of the Prince");
            Tooltip.SetDefault("");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 58;
            Item.height = 52;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 25);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;	

            // Weapon Properties
            Item.damage = 120;
            Item.knockBack = 10.5f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 3.7f;
            Item.shoot = ModContent.ProjectileType<Midnight_SlashProj>();
            Item.Redemption().TechnicallyAxe = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<NoblesHalberd>())
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddIngredient(ItemID.Ectoplasm, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}