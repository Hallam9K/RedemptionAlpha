using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.NPCs.Minibosses.EaglecrestGolem;
using Microsoft.Xna.Framework;
using Redemption.WorldGeneration.Soulless;
using SubworldLibrary;

namespace Redemption.Items.Usable.Summons
{
    public class EaglecrestSpelltome : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Calls upon Eaglecrest Golem"
                + "\nSold by Zephos/Daerel after Eater of Worlds/Brain of Cthulhu is defeated"); */

            Item.ResearchUnlockCount = 1;
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
            Item.width = 38;
            Item.height = 40;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }
        public override bool CanUseItem(Player player)
        {
            return !SubworldSystem.IsActive<SoullessSub>() && !NPC.AnyNPCs(ModContent.NPCType<EaglecrestGolem>()) && !NPC.AnyNPCs(ModContent.NPCType<EaglecrestGolem_Sleep>());
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
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);

                int golem = NPC.FindFirstNPC(type);
                if (golem > -1)
                {
                    int steps = (int)Main.npc[golem].Distance(player.Center) / 8;
                    for (int i = 0; i < steps; i++)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            Dust dust = Dust.NewDustDirect(Vector2.Lerp(Main.npc[golem].Center, player.Center, (float)i / steps), 2, 2, DustID.Sandnado, Scale: 4);
                            dust.velocity = -player.DirectionTo(dust.position) * 2;
                            dust.noGravity = true;
                        }
                    }
                }
            }
            return true;
        }
    }
}
