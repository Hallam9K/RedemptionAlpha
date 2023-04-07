using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.NPCs.Bosses.Keeper;
using Redemption.NPCs.Minibosses.SkullDigger;
using Redemption.Globals;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Redemption.Items.Materials.PreHM;

namespace Redemption.Items.Usable.Summons
{
    public class WeddingRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wedding Ring");
            /* Tooltip.SetDefault("Attracts the attention of a sorrowful mistress"
                + "\nOnly usable at night"
                + "\nNot consumable"); */

            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<Keeper>()) && !NPC.AnyNPCs(ModContent.NPCType<SkullDigger>()) && !NPC.AnyNPCs(ModContent.NPCType<KeeperSpirit>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = ModContent.NPCType<Keeper>();
                if (RedeBossDowned.keeperSaved)
                    type = ModContent.NPCType<KeeperSpirit>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                }
                else
                {
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                }
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GraveSteelAlloy>(), 6)
                .AddIngredient(ItemID.Ruby)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (RedeBossDowned.keeperSaved)
            {
                TooltipLine line = new(Mod, "SpiritLine",
                    "The ring still glows a faint blue...")
                {
                    OverrideColor = Color.LightSkyBlue
                };
                tooltips.Insert(tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("Tooltip2")), line);
            }
        }
    }
}
