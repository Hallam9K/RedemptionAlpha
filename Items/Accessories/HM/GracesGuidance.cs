using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    [AutoloadEquip(EquipType.Neck)]
    public class GracesGuidance : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grace's Guidance");
            Tooltip.SetDefault("12% increased Holy elemental damage and resistance\n" +
                "6% increased Holy and Fire elemental critical strike chance\n" +
                "Stacks if both elements are present\n" +
                "Critical strikes with a Holy elemental weapon has a chance to release homing lightmass\n" +
                "An aura of holy flames surrounds you while holding a Fire or Holy elemental weapon\n" +
                "Increases length of invincibility after taking damage");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 52;
            Item.value = Item.sellPrice(0, 8, 80, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
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
            modPlayer.ElementalDamage[7] += 0.12f;
            modPlayer.ElementalResistance[7] += 0.12f;
            player.longInvince = true;
            modPlayer.gracesGuidance = true;

            if (player.whoAmI == Main.myPlayer && player.active && !player.dead && (ItemLists.Fire.Contains(player.HeldItem.type) || ProjectileLists.Fire.Contains(player.HeldItem.shoot) || ItemLists.Holy.Contains(player.HeldItem.type) || ProjectileLists.Holy.Contains(player.HeldItem.shoot)))
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
                    if (!npc.active || !npc.CanBeChasedBy() || player.DistanceSQ(npc.Center) > 280 * 280)
                        continue;

                    npc.AddBuff(ModContent.BuffType<HolyFireDebuff>(), 4);
                }
            }
        }
        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            if (slot < 10)
            {
                int maxAccessoryIndex = 5 + player.extraAccessorySlots;
                for (int i = 3; i < 3 + maxAccessoryIndex; i++)
                {
                    if (slot != i && player.armor[i].type == ModContent.ItemType<PowerCellWristband>())
                        return false;
                    if (slot != i && player.armor[i].type == ModContent.ItemType<SacredCross>())
                        return false;
                }
            }
            return true;
        }
    }
}
