using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.PreHM
{
    [AutoloadEquip(EquipType.Body)]
    public class DragonLeadRibplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon-Lead Ribplate");
            Tooltip.SetDefault("7% increased damage\n" +
                "Immunity to most ice-related debuffs");
            ArmorIDs.Body.Sets.IncludedCapeBack[Mod.GetEquipSlot(Name, EquipType.Body)] = Redemption.dragonLeadCapeID;
            ArmorIDs.Body.Sets.IncludedCapeBackFemale[Mod.GetEquipSlot(Name, EquipType.Body)] = Redemption.dragonLeadCapeID;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 28;
            Item.sellPrice(0, 0, 65);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += .07f;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Frostburn2] = true;
            player.buffImmune[BuffID.Wet] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DragonLeadAlloy>(), 20)
                .AddIngredient(ItemID.Bone, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Armour forged from melted dragon bone and metal, made to look like a ribcage. It is said to be used by\n" +
                    "the ancient warlords of Dragonrest.\n" +
                    "The warlords were famous dragon slayers who used the bones of their victims for weaponry and armour,\n" +
                    "nearly bringing the dragons to extinction. That was until every single one was wiped out by Goliathon, the Dragon God.'")
                {
                    overrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view lore")
                {
                    overrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}