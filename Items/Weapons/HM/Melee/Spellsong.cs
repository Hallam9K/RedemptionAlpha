using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class Spellsong : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.ArcaneS);
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spellsong, Core of the West");
            /* Tooltip.SetDefault("Slashes emit arcane waves" +
                "\nHolding left-click will do a 3-swing combo, ending with Spellsong shooting a beam that conjures mirages of itself"); */

            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 76;
            Item.height = 76;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 12);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.channel = true;

            // Weapon Properties
            Item.damage = 138;
            Item.knockBack = 7;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<Spellsong_Proj>();
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 18)
                .AddIngredient(ItemID.Ectoplasm, 8)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.Items.Spellsong.Lore"))
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

            TooltipLine slashLine = new(Mod, "SharpBonus", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.SlashBonus")) { OverrideColor = Colors.RarityOrange };
            tooltips.Add(slashLine);
        }
    }
}
