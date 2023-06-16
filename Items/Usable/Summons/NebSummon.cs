using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Collections.Generic;
using Redemption.NPCs.Bosses.Neb;
using Redemption.NPCs.Bosses.Neb.Phase2;
using Redemption.NPCs.Bosses.Neb.Clone;
using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.WorldGeneration.Soulless;
using SubworldLibrary;

namespace Redemption.Items.Usable.Summons
{
    public class NebSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Galaxy Stone");
            /* Tooltip.SetDefault("Summons the Angel of the Cosmos"
                + "\nOnly usable at night"
                + "\nNot consumable"); */
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 8));
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 13;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 26;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 65, 0, 0);
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.noUseGraphic = true;
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
        }
        public override bool CanUseItem(Player player)
        {
            return !SubworldSystem.IsActive<SoullessSub>() && !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<Nebuleus>()) && !NPC.AnyNPCs(ModContent.NPCType<Nebuleus2>()) && !NPC.AnyNPCs(ModContent.NPCType<Nebuleus_Clone>()) && !NPC.AnyNPCs(ModContent.NPCType<Nebuleus2_Clone>()) && !NPC.AnyNPCs(ModContent.NPCType<Neb_Start>());
        }
        public override bool AltFunctionUse(Player player)
        {
            return RedeBossDowned.nebDeath > 5;
        }
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                int type = ModContent.NPCType<Neb_Start>();
                if (RedeBossDowned.nebDeath > 5 && player.altFunctionUse == 2)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Teleport2, player.position);

                    if (RedeBossDowned.nebDeath < 7)
                    {
                        type = ModContent.NPCType<Nebuleus2>();

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(new EntitySource_BossSpawn(player), (int)player.position.X + 200, (int)player.position.Y - 200, type);
                        else
                            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                    }
                    else
                    {
                        Main.NewText("Nebuleus is nowhere to be found...", Color.MediumPurple.R, Color.MediumPurple.G, Color.MediumPurple.B);
                        type = ModContent.NPCType<Nebuleus2_Clone>();

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(new EntitySource_BossSpawn(player), (int)player.position.X + 200, (int)player.position.Y - 200, type);
                        else
                            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                    }
                }
                else
                {
                    if (RedeBossDowned.nebDeath < 7)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(new EntitySource_BossSpawn(player), (int)player.position.X + 200, (int)player.position.Y - 200, type);
                        else
                            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                    }
                    else
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.Teleport1, player.position);

                        Main.NewText("Nebuleus is nowhere to be found...", Color.MediumPurple.R, Color.MediumPurple.G, Color.MediumPurple.B);
                        type = ModContent.NPCType<Nebuleus_Clone>();

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(new EntitySource_BossSpawn(player), (int)player.position.X + 200, (int)player.position.Y - 200, type);
                        else
                            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                    }
                }
            }
            return true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.player[Main.myPlayer];
            int tooltipLocation = tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("Tooltip0"));
            string text1 = "Right-click to summon Nebuleus' Final Form instantly";
            TooltipLine line = new(Mod, "text1", text1)
            {
                OverrideColor = Main.DiscoColor
            };
            if (RedeBossDowned.nebDeath > 5)
            {
                tooltips.Insert(tooltipLocation, line);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.FragmentNebula, 20)
            .AddIngredient(ItemID.LunarBar, 10)
            .AddIngredient(ItemID.HallowedBar, 10)
            .AddIngredient<GildedStar>(20)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}
