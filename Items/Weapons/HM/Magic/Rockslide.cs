using Microsoft.Xna.Framework;
using Redemption.Projectiles.Magic;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class Rockslide : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Casts a large amount of rocks to float above the player\n" +
                "Release left-click to launch them at cursor point");

            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 85;
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
                TooltipLine line = new(Mod, "Lore",
                    "'An ancient relic owned by King Tenebris. The texts contained within this tome were said\n" +
                    "to hold the knowledge necessary to master psychic magic.\n\n" +
                    "King Tenebris reigns over Erellon within the capital of Arrgath, a province protected by\n" +
                    "lands of plentiful life and barren death. It is said his knowledge of the ancient times is truly boundless.'")
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view lore")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}