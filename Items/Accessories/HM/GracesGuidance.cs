using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    [AutoloadEquip(EquipType.Neck)]
    public class GracesGuidance : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.HolyS, ElementID.FireS);
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Grace's Guidance");
            /* Tooltip.SetDefault("12% increased " + ElementID.HolyS + " elemental damage and resistance\n" +
                "6% increased " + ElementID.HolyS + " and " + ElementID.FireS + " elemental critical strike chance\n" +
                "Stacks if both elements are present\n" +
                "Critical strikes with a " + ElementID.HolyS + " elemental weapon has a chance to release homing lightmass\n" +
                "An aura of holy flames surrounds you while holding a " + ElementID.FireS + " or " + ElementID.HolyS + " elemental weapon\n" +
                "Increases length of invincibility after taking damage"); */
            Item.ResearchUnlockCount = 1;
            ElementID.ItemHoly[Type] = true;
            ElementID.ItemFire[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 52;
            Item.value = Item.sellPrice(0, 8, 80, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SacredCross>()
                .AddIngredient<PowerCellWristband>()
                .AddIngredient<LostSoul>(8)
                .AddIngredient(ItemID.FragmentSolar, 8)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
        private int timer;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            modPlayer.ElementalDamage[ElementID.Holy] += 0.12f;
            modPlayer.ElementalResistance[ElementID.Holy] += 0.12f;
            player.longInvince = true;
            modPlayer.gracesGuidance = true;

            if (player.whoAmI == Main.myPlayer && player.active && !player.dead && (player.HeldItem.HasElementItem(ElementID.Fire) || player.HeldItem.HasElementItem(ElementID.Holy)))
            {
                if (timer++ == 30)
                    RedeDraw.SpawnCirclePulse(player.Center, Color.DarkOrange * 0.8f, 0.7f, player);
                if (timer >= 40)
                {
                    RedeDraw.SpawnCirclePulse(player.Center, Color.Goldenrod * 0.8f, 0.8f, player);
                    timer = 0;
                }

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || !npc.CanBeChasedBy() || NPCLoader.CanBeHitByItem(npc, player, Item) is false || player.DistanceSQ(npc.Center) > 280 * 280)
                        continue;

                    npc.AddBuff(ModContent.BuffType<HolyFireDebuff>(), 4);
                }
            }
        }
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            if (equippedItem.type == ModContent.ItemType<PowerCellWristband>() || equippedItem.type == ModContent.ItemType<SacredCross>())
                return false;
            return true;
        }
    }
}
