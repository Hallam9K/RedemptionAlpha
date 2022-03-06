using Redemption.NPCs.Bosses.Gigapora;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Summons
{
    public class OmegaPowerdrill : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omega Powerdrill");
            Tooltip.SetDefault("Summons the 2nd Omega Prototype\n" +
                "'Mechanical whirls beneath the ground, be wary of the deadly sound'"
                + "\nOnly usable at night"
                + "\nNot consumable");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 42;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }
        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<Gigapora>()) && !NPC.AnyNPCs(ModContent.NPCType<Porakone>());
        }
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position, 0);

                int type = ModContent.NPCType<Porakone>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.NewNPC(new EntitySource_BossSpawn(player), (int)player.position.X + 200, (int)player.position.Y - 500, type);
                else
                    NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: type);
            }
            return true;
        }
    }
}