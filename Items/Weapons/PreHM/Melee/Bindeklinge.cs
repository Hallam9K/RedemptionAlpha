using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Redemption.Projectiles.Melee;
using Terraria.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class Bindeklinge : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Critical strikes release homing lightmass");
            Item.ResearchUnlockCount = 1;
            ElementID.ItemHoly[Type] = true;
        }

        public override void SetDefaults()
		{
            Item.damage = 19;
            Item.DamageType = DamageClass.Melee;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.crit = 10;
            Item.value = Item.sellPrice(0, 0, 54, 0);
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
            Item.useTurn = true;
            Item.rare = ItemRarityID.Blue;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
            {
                SoundEngine.PlaySound(SoundID.Item101, player.Center);
                for (int i = 0; i < Main.rand.Next(4, 7); i++)
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), target.Center, new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-9, -5)), ModContent.ProjectileType<Lightmass>(), 7, hit.Knockback / 2, player.whoAmI);
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.Items.Bindeklinge.Lore"))
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.Viewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}
