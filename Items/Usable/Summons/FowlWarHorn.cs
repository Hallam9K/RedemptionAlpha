using Microsoft.Xna.Framework;
using Redemption.Globals.World;
using SubworldLibrary;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Redemption.Globals.RedeNet;

namespace Redemption.Items.Usable.Summons
{
    public class FowlWarHorn : ModItem
	{
		public override void SetStaticDefaults()
		{
            /* Tooltip.SetDefault("Calls upon the Fowl Morning"
                + "\nOnly usable before midday"); */
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }

        public override void SetDefaults()
		{
            Item.width = 28;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 2000;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.noMelee = true;
            Item.consumable = true;
            Item.autoReuse = false;
            Item.UseSound = SoundID.Item43;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ZoneTowerNebula || player.ZoneTowerSolar || player.ZoneTowerStardust || player.ZoneTowerVortex || FowlMorningWorld.FowlMorningActive || !Main.dayTime || Main.time >= 27000 || SubworldSystem.AnyActive<Redemption>())
                return false;
            return true;
        }
        public override bool? UseItem(Player player)
        {
            if (FowlMorningWorld.FowlMorningActive)
                return false;

            if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                Redemption.WriteToPacket(Redemption.Instance.GetPacket(), (byte)ModMessageType.StartFowlMorning).Send();
            else
                FowlMorningWorld.FowlMorningActive = true;

            string status = "The fowl legion charges in!";
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), new Color(250, 170, 50));
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(status), new Color(250, 170, 50));

            string waveText = FowlMorningNPC.GetWaveChatText(FowlMorningWorld.ChickWave);
            Color color = new(175, 75, 255);
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(waveText), color);
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(waveText), color);

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
            return true;
        }

    }
}
