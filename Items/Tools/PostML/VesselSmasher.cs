using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using Redemption.Buffs.NPCBuffs;
using Terraria.GameContent.Creative;
using Redemption.Rarities;
using Redemption.BaseExtension;
using Redemption.Globals.Player;
using Redemption.Items.Materials.PostML;

namespace Redemption.Items.Tools.PostML
{
    public class VesselSmasher : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Smashing an enemy will make it take 15% more damage for 5 seconds");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 540;
            Item.DamageType = DamageClass.Melee;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 2;
            Item.useAnimation = 26;
            Item.hammer = 100;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 11;
            Item.value = Item.buyPrice(0, 55, 0, 0);
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SmashedDebuff>(), 300);
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (player.altFunctionUse == 2)
                knockBack *= 4;
        }
        public bool activate;
        public override bool? UseItem(Player player)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            if (player.altFunctionUse == 2 && modPlayer.shadowBinder && modPlayer.shadowBinderCharge >= 1 && !activate)
            {
                for (int i = 0; i < 5; i++)
                {
                    int dustIndex = Dust.NewDust(player.position, player.width, player.height, DustID.AncientLight);
                    Main.dust[dustIndex].velocity *= 2.4f;
                }
                modPlayer.shadowBinderCharge -= 1;
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
                text = "Right-clicking will swing with extreme knockback (Consumes 1 Shadowbound Soul)";
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