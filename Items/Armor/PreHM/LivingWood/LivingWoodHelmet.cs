using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.PreHM.LivingWood
{
    [AutoloadEquip(EquipType.Head)]
    public class LivingWoodHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Living Wood Helmet");
            // Tooltip.SetDefault("+1 increased summon damage");
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.sellPrice(copper: 30);
            Item.rare = ItemRarityID.White;
            Item.defense = 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<LivingWoodBody>() && legs.type == ItemType<LivingWoodLeggings>();
        }

        public override void UpdateEquip(Player player)
        {
            player.maxTurrets += 1;
        }
        public override void ArmorSetShadows(Player player)
        {
            if (Main.rand.NextBool(25) && Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y) > 1f && !player.rocketFrame)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                Gore.NewGore(player.GetSource_FromThis(), new Vector2(player.Center.X + Main.rand.Next(-12, 4), player.Center.Y + Main.rand.Next(6)), player.velocity, GoreID.TreeLeaf_Normal);
            }
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.LivingWood", ElementID.NatureS);
            player.buffImmune[BuffID.Poisoned] = true;
            player.GetDamage(DamageClass.Summon).Flat += 1;
            player.RedemptionPlayerBuff().ElementalDamage[ElementID.Nature] += 0.15f;
            player.RedemptionPlayerBuff().ElementalResistance[ElementID.Nature] += 0.15f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LivingTwig>(24)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
