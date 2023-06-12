using Microsoft.Xna.Framework;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Projectiles.Magic;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class Rockslide : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Casts a large amount of rocks to float above the player\n" +
                "Release left-click to launch them at cursor point"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 32;
            Item.height = 36;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 8;
            Item.channel = true;
            Item.value = Item.buyPrice(0, 70, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.shootSpeed = 0;
            Item.shoot = ModContent.ProjectileType<Rockslide_Proj>();
            Item.UseSound = SoundID.Item69;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 10;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.Items.Rockslide.Lore"))
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
                .AddIngredient(ModContent.ItemType<Earthbind>())
                .AddIngredient(ItemID.BeetleHusk, 4)
                .AddIngredient(ItemID.LunarTabletFragment, 10)
                .AddIngredient(ItemID.SoulofNight, 15)
                .AddIngredient(ItemID.SoulofMight, 15)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}
