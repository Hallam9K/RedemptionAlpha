using Redemption.BaseExtension;
using Redemption.DamageClasses;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.PreHM.ElderWood
{
    [AutoloadEquip(EquipType.Head)]
    public class ElderWoodHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = Item.sellPrice(copper: 30);
            Item.rare = ItemRarityID.White;
            Item.defense = 2;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<ElderWoodBreastplate>() && legs.type == ItemType<ElderWoodGreaves>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Magic) += 4;
        }
        public override void ArmorSetShadows(Player player)
        {
            if (Main.rand.NextBool(25) && Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y) > 1f && !player.rocketFrame)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                Gore.NewGore(player.GetSource_FromThis(), new Vector2(player.Center.X + Main.rand.Next(-12, 4), player.Center.Y + Main.rand.Next(6)), player.velocity, Find<ModGore>("Redemption/ElderTreeFX").Type);
            }
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.ElderWood", ElementID.NatureS, ElementID.PoisonS);
            player.RedemptionPlayerBuff().elderWoodBonus = true;
            player.buffImmune[BuffID.Poisoned] = true;
            player.manaCost *= .9f;
            player.RedemptionPlayerBuff().ElementalDamage[ElementID.Nature] += 0.08f;
            player.RedemptionPlayerBuff().ElementalDamage[ElementID.Poison] += 0.08f;
            player.RedemptionPlayerBuff().ElementalResistance[ElementID.Nature] += 0.12f;
            player.RedemptionPlayerBuff().ElementalResistance[ElementID.Poison] += 0.12f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Placeable.Tiles.ElderWood>(20)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}