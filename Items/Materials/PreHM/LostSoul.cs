using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.NPCs.Friendly;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class LostSoul : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lost Soul");
            // Tooltip.SetDefault("'The incarnation of Willpower'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;

            Item.ResearchUnlockCount = 10;
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 50;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.consumable = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Caught sourceParent)
            {
                if (sourceParent.Entity is NPC caughtNPC)
                {
                    float scale = (Main.npc[caughtNPC.whoAmI].ModNPC as LostSoulNPC).Scale;
                    int dropAmount = (int)(scale / 2 * 10);
                    Item.stack = 1 + dropAmount;
                }
            }
        }
        public override bool? UseItem(Player player)
        {
            int index = NPC.NewNPC(new EntitySource_SpawnNPC(), (int)player.Center.X, (int)player.Center.Y,
                ModContent.NPCType<LostSoulNPC>(), ai2: 60);
            Main.npc[index].velocity = RedeHelper.PolarVector(10, (Main.MouseWorld - player.Center).ToRotation());

            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                NetMessage.SendData(MessageID.SyncNPC, number: index);

            return true;
        }
        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale);

            if (!Main.rand.NextBool(3))
                return;

            int sparkle = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height / 2,
                DustID.DungeonSpirit, 0, 0, 20);
            Main.dust[sparkle].noGravity = true;
        }
    }
}