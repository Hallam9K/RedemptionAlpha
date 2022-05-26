using Microsoft.Xna.Framework;
using Redemption.NPCs.Bosses.Obliterator;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Summons
{
    public class OmegaRadar : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omega Radar");
            Tooltip.SetDefault("Summons one of Vlitch's Overlords"
                + "\n'Feel the sense of frustration, prepare for obliteration'"
                + "\nOnly usable at night"
                + "\nOnly usable after the first 2 Vlitch Overlords are defeated"
                + "\nNot consumable");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
		{
			Item.width = 34;
            Item.height = 30;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(0, 25, 0, 0);
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item44;
            Item.consumable = false;
		}

		public override bool CanUseItem(Player player)
		{
            return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<OO>());
		}

		public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position);

            int type = ModContent.NPCType<OO>();

            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.NewNPC(new EntitySource_BossSpawn(player), (int)player.position.X + 350, (int)player.position.Y - 800, type);
            return true;
		}
	}
}