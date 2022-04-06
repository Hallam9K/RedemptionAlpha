using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Buffs;
using Redemption.Buffs.Debuffs;
using Redemption.Globals.Player;
using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Tools.PostML
{
    public class VesselPickaxeAxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Hitting an enemy will make them bleed heavily for a long period of time");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 247;
            Item.DamageType = DamageClass.Melee;
            Item.width = 60;
            Item.height = 62;
            Item.useTime = 7;
            Item.useAnimation = 10;
            Item.pick = 320;
            Item.axe = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(0, 55, 0, 0);
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<LaceratedDebuff>(), 1200);
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.HasBuff(ModContent.BuffType<VesselPickBuff>()))
                Item.useTime = 3;
            else
                Item.useTime = 7;
            return true;
        }
        public bool activate;
        public override bool? UseItem(Player player)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            if (player.altFunctionUse == 2 && modPlayer.shadowBinder && modPlayer.shadowBinderCharge >= 4 && !activate)
            {
                for (int i = 0; i < 15; i++)
                {
                    int dustIndex = Dust.NewDust(player.position, player.width, player.height, DustID.AncientLight);
                    Main.dust[dustIndex].velocity *= 2.4f;
                }
                SoundEngine.PlaySound(SoundID.Item74, player.position);
                modPlayer.shadowBinderCharge -= 4;
                player.AddBuff(ModContent.BuffType<VesselPickBuff>(), 600);
                activate = true;
            }
            return true;
        }
        public override void HoldItem(Player player)
        {
            if (player.itemAnimation == 0)
                activate = false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.player[Main.myPlayer];
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            int tooltipLocation = tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("Tooltip0"));
            string text;
            if (modPlayer.shadowBinder)
                text = "Right-clicking will give this tool a boost in mining speed for 10 seconds (Consumes 4 Shadowbound Souls)";
            else
                text = "Has a special ability if Sielukaivo Shadowbinder is equipped";
            TooltipLine line = new(Mod, "text", text) { OverrideColor = Color.DarkGray };
            tooltips.Insert(tooltipLocation, line);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<VesselFragment>(), 24)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
