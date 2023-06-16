using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Redemption.WorldGeneration;
using Redemption.NPCs.Lab.Janitor;
using Redemption.NPCs.Lab.Behemoth;
using Redemption.NPCs.Lab.Blisterface;
using Redemption.NPCs.Lab.MACE;
using Redemption.NPCs.Bosses.PatientZero;
using Terraria.Audio;

namespace Redemption.Items.Usable.Summons
{
    public class LabHologramDevice : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hologram Remote");
            // Tooltip.SetDefault("Use in an Abandoned Laboratory boss's arena to resummon it");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = false;
            Item.width = 24;
            Item.height = 38;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
        }
        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<JanitorBot>()) && !NPC.AnyNPCs(ModContent.NPCType<JanitorBot_Holo>()) && !NPC.AnyNPCs(ModContent.NPCType<JanitorBot_Defeated>()) && !NPC.AnyNPCs(ModContent.NPCType<IrradiatedBehemoth>()) && !NPC.AnyNPCs(ModContent.NPCType<IrradiatedBehemoth_Holo>()) && !NPC.AnyNPCs(ModContent.NPCType<Blisterface>()) && !NPC.AnyNPCs(ModContent.NPCType<Blisterface_Holo>()) && !NPC.AnyNPCs(ModContent.NPCType<MACEProject>()) && !NPC.AnyNPCs(ModContent.NPCType<MACEProject_Holo>()) && !NPC.AnyNPCs(ModContent.NPCType<PZ>()) && !NPC.AnyNPCs(ModContent.NPCType<PZ_Body_Holo>());
        }
        public override bool? UseItem(Player player)
        {
            Rectangle janitorRect = new((int)(RedeGen.LabVector.X + 145) * 16, (int)(RedeGen.LabVector.Y + 11) * 16, 47 * 16, 11 * 16);
            Rectangle behemothRect = new((int)(RedeGen.LabVector.X + 201) * 16, (int)(RedeGen.LabVector.Y + 45) * 16, 25 * 16, 76 * 16);
            Rectangle blisterfaceRect = new((int)(RedeGen.LabVector.X + 193) * 16, (int)(RedeGen.LabVector.Y + 163) * 16, 30 * 16, 20 * 16);
            Rectangle maceRect = new((int)(RedeGen.LabVector.X + 43) * 16, (int)(RedeGen.LabVector.Y + 153) * 16, 61 * 16, 29 * 16);
            Rectangle pzRect = new((int)(RedeGen.LabVector.X + 109) * 16, (int)(RedeGen.LabVector.Y + 170) * 16, 70 * 16, 42 * 16);

            int type;
            if (player.Hitbox.Intersects(janitorRect))
                type = ModContent.NPCType<JanitorBot_Holo>();
            else if (player.Hitbox.Intersects(behemothRect))
                type = ModContent.NPCType<IrradiatedBehemoth_Holo>();
            else if (player.Hitbox.Intersects(blisterfaceRect))
                type = ModContent.NPCType<Blisterface_Holo>();
            else if (player.Hitbox.Intersects(maceRect))
                type = ModContent.NPCType<MACEProject_Holo>();
            else if (player.Hitbox.Intersects(pzRect))
                type = ModContent.NPCType<PZ_Body_Holo>();
            else
            {
                CombatText.NewText(player.getRect(), Color.Cyan, "Nothing happens...", true, true);
                return true;
            }

            SoundEngine.PlaySound(CustomSounds.BallFire, player.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, type);
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            return true;
        }
    }
}
