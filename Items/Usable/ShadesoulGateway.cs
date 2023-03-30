using Microsoft.Xna.Framework;
using Redemption.NPCs.Friendly;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class ShadesoulGateway : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Opens a portal to the Soulless Caverns" +
                "\nCan also be used to leave the Soulless Caverns" +
                "\n'You feel keeping the gateway opened would be a bad idea...'"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 1;
            Item.noUseGraphic = true;
            Item.useAnimation = 80;
            Item.useTime = 80;
            Item.UseSound = SoundID.NPCDeath52;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
        public override bool? UseItem(Player player)
        {
            int type = ModContent.NPCType<SoullessPortal>();
            for (int g = 0; g < Main.maxNPCs; ++g)
            {
                if (Main.npc[g].active && Main.npc[g].type == type)
                {
                    Main.npc[g].ai[0] = 3;
                    break;
                }
            }
            if (!NPC.AnyNPCs(type))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.NewNPC(new EntitySource_BossSpawn(player), (int)player.Center.X, (int)player.Center.Y, type);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);

                Main.NewText("A Shadesoul Gateway has been opened...", Color.LightSlateGray);
            }
            else
            {
                Main.NewText("A Shadesoul Gateway has been closed...", Color.LightSlateGray);
            }
            return true;
        }
    }
}