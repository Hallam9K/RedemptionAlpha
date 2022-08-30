using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Melee;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class Chernobyl : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chernobyl");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 20;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.knockBack = 5f;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<Chernobyl_Proj>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WoodYoyo)
                .AddIngredient(ModContent.ItemType<XenomiteItem>(), 10)
                .AddRecipeGroup(RedeRecipe.BioweaponBileRecipeGroup, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}