using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.NPCs.Bosses.Erhan;
using Redemption.Base;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Redemption.Globals;
using Redemption.WorldGeneration.Soulless;
using SubworldLibrary;

namespace Redemption.Items.Usable.Summons
{
    public class DemonScroll : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Forbidden Ritual");
            /* Tooltip.SetDefault("May draw unwanted attention\n" +
                "Requires the user to have at least 140 max life"
                + "\nNot consumable" +
                "\n[i:" + ModContent.ItemType<BadRoute>() + "][c/ff5533: This item may have a negative impact onto the world]"); */
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = false;
            Item.width = 30;
            Item.height = 34;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Blue;
        }
        public override bool CanUseItem(Player player)
        {
            return !SubworldSystem.IsActive<SoullessSub>() && (player.statLifeMax2 >= 140 || player.statLifeMax2 == 1) && !NPC.AnyNPCs(ModContent.NPCType<PalebatImp>()) && !NPC.AnyNPCs(ModContent.NPCType<Erhan>()) && !NPC.AnyNPCs(ModContent.NPCType<ErhanSpirit>());
        }
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                int type = ModContent.NPCType<PalebatImp>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);

                if (RedeBossDowned.erhanDeath > 0 || player.statLifeMax2 == 1)
                    return true;

                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(player.position, player.width, player.height, DustID.Blood);
                    Dust.NewDust(player.position, player.width, player.height, DustID.Torch);
                }

                SoundEngine.PlaySound(SoundID.NPCDeath19, player.position);
                int cap = (int)MathHelper.Min(70, player.statLife - 10);
                if (cap > 0)
                    BaseAI.DamagePlayer(player, cap, 0, null, false, true);
            }
            return true;
        }
    }
}
