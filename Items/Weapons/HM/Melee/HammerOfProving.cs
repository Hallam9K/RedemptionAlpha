using Redemption.Buffs;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class HammerOfProving : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hammer of Proving");
            /* Tooltip.SetDefault("Hold left-click while in the air to keep the hammer out\n" +
                "Increases the user's fall speed while held\n" +
                "Increased damage based on how fast the player is falling\n" +
                "Stuns enemies if falling above a certain speed"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 80;
            Item.height = 80;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 5);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;

            // Weapon Properties
            Item.damage = 200;
            Item.knockBack = 9;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<HammerOfProving_Proj>();
            Item.Redemption().TechnicallyHammer = true;
        }
        public override void HoldItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<HammerBuff>(), 2);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 15)
                .AddRecipeGroup(RedeRecipe.GoldRecipeGroup, 15)
                .AddIngredient(ItemID.Ruby, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
