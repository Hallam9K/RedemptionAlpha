using Redemption.Items.Materials.HM;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.WorldGeneration.Soulless;
using SubworldLibrary;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Summons
{
    public class CyberTech : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cyber Radio");
            // Tooltip.SetDefault("Transmits a signal towards a colossal spaceship\nOnly usable at day\nNot consumable");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 4));
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 13;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 42;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 25, 0, 0);
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item44;
            Item.noUseGraphic = true;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !SubworldSystem.IsActive<SoullessSub>() && Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<KS3>()) && !NPC.AnyNPCs(ModContent.NPCType<KS3_Clone>()) && !NPC.AnyNPCs(ModContent.NPCType<KS3_ScannerDrone>()) && !NPC.AnyNPCs(ModContent.NPCType<KS3_Start>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                int type = ModContent.NPCType<KS3_Start>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.NewNPC(new EntitySource_BossSpawn(player), (int)player.position.X + 200, (int)player.position.Y - 500, type);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<AIChip>(), 1)
            .AddIngredient(ModContent.ItemType<Plating>(), 4)
            .AddIngredient(ModContent.ItemType<Capacitor>(), 2)
            .AddIngredient(ItemID.SoulofSight, 5)
            .AddIngredient(ItemID.SoulofMight, 5)
            .AddIngredient(ItemID.SoulofFright, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}