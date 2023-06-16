using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class SteampunkMinigun : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Steam-Cog Minigun");
            /* Tooltip.SetDefault("Holding left-click will charge the weapon up, releasing will cause it to shoot rapidly for a short duration\n" +
                "Shooting duration scales with the amount of time charged up, capping at 5 seconds\n" +
                "Replaces normal bullets with high velocity bullets\n" +
                "66% chance to not consume ammo"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 34;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 78;
            Item.height = 28;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = CustomSounds.WindUp;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 90;
            Item.useAmmo = AmmoID.Bullet;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<SteampunkMinigun_Proj>();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.Items.SteampunkMinigun.Lore"))
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.Viewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChainGun)
                .AddIngredient(ItemID.Cog, 25)
                .AddIngredient<NiricPipe>(15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
