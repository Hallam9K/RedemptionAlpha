using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Ranged;
using System.Collections.Generic;
using Terraria.Localization;
using Redemption.BaseExtension;
using Redemption.Globals;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class LunarShot : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.FireS, ElementID.NatureS);
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lunar Shot");
            /* Tooltip.SetDefault("Replaces Wooden Arrows with Lunar Bolts, dealing " + ElementID.FireS + " and " + ElementID.NatureS + " damage" +
                "\nLunar Bolts summon bats while the moon is out" +
                "\nSummons an extra bat while it's a full moon"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
			Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
            Item.height = 44;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1;
            Item.value = Item.sellPrice(0, 0, 2, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = false;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;
            Item.ExtraItemShoot(ModContent.ProjectileType<LunarShot_Proj>());
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
                type = ModContent.ProjectileType<LunarShot_Proj>();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string text = "There is no moonlight to reflect...";
            if (Main.dayTime || Main.moonPhase == 4)
            {
                TooltipLine line = new(Mod, "text", text)
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Insert(2, line);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<MoonflareFragment>(), 8)
                .AddTile(TileID.Anvils)
                .AddCondition(RedeConditions.InMoonlight)
                .Register();
        }
    }
}
