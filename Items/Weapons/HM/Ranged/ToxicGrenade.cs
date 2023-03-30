using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class ToxicGrenade : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Throw a grenade that leaves behind a radioactive cloud");
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.damage = 56;
            Item.knockBack = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 0, 85);
            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 50;
            Item.useTime = 50;
            Item.UseSound = SoundID.Item7;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.shoot = ModContent.ProjectileType<ToxicGrenade_Proj>();
            Item.shootSpeed = 11f;
            Item.ammo = Item.type;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (!Main.hardMode)
                damage.Base -= 14;
        }
        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient(ItemID.Grenade, 10)
                .AddIngredient(ModContent.ItemType<ToxicBile>())
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.Grenade)
                .AddIngredient(ModContent.ItemType<XenomiteShard>(), 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
