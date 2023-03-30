using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class Chernobyl : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Chernobyl");
            // Tooltip.SetDefault("Enemies slain by this weapon explode into fireballs");
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useTime = 20;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.knockBack = 5f;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.LightRed;
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
                .AddIngredient(ModContent.ItemType<ToxicBile>(), 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}