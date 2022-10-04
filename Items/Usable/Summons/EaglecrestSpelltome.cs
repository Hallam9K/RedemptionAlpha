using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.NPCs.Minibosses.EaglecrestGolem;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Usable.Summons
{
    public class EaglecrestSpelltome : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Calls upon Eaglecrest Golem"
                + "\nSold by Zephos/Daerel after Eater of Worlds/Brain of Cthulhu is defeated");

            SacrificeTotal = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 38;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }
        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<EaglecrestGolem>()) && !NPC.AnyNPCs(ModContent.NPCType<EaglecrestGolem_Sleep>());
        }
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                int type = ModContent.NPCType<EaglecrestGolem_Sleep>();

                Main.NewText("A sleeping stone appears...", Color.Gray);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                else
                    NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: type);
            }
            return true;
        }
    }
}
