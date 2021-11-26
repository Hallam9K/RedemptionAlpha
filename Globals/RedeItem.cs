using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Globals.Player;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.PreHM.Melee;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeItem : GlobalItem
    {
        public override void ModifyHitNPC(Item item, Terraria.Player player, Terraria.NPC target, ref int damage,
            ref float knockBack, ref bool crit)
        {
            if (item.axe > 0 && crit)
                damage += damage / 2;
        }
        public override bool OnPickup(Item item, Terraria.Player player)
        {
            if (item.type == ItemID.Heart && player.GetModPlayer<BuffPlayer>().heartInsignia)
                player.AddBuff(ModContent.BuffType<HeartInsigniaBuff>(), 180);

            return true;
        }
        public override void PostUpdate(Item item)
        {
            if (item.type == ItemID.Heart && Main.LocalPlayer.GetModPlayer<BuffPlayer>().heartInsignia)
            {
                if (!Main.rand.NextBool(6))
                    return;

                int sparkle = Dust.NewDust(new Vector2(item.position.X, item.position.Y), item.width, item.height,
                    DustID.ShadowbeamStaff, Scale: 2);
                Main.dust[sparkle].velocity.X = 0;
                Main.dust[sparkle].velocity.Y = -2;
                Main.dust[sparkle].noGravity = true;
            }
        }
        public override void OnCreate(Item item, ItemCreationContext context)
        {
            if (item.type == ModContent.ItemType<AlignmentTeller>())
                RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Greetings, I am the Chalice of Alignment, and I believe any action can be redeemed.", 260, 30, 0, Color.DarkGoldenrod);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.CountsAsClass(DamageClass.Melee) && item.damage >= 4 && item.useStyle == ItemUseStyleID.Swing && !item.noUseGraphic)
            {
                if (item.axe > 0)
                {
                    TooltipLine axeLine = new(Mod, "AxeBonus", "Axe Bonus: 3x critical strike damage, increased chance to decapitate skeletons") { overrideColor = Colors.RarityOrange };
                    tooltips.Add(axeLine);
                }
                else if (!ItemTags.BluntSwing.Has(item.type) && item.hammer == 0 && item.pick == 0)
                {
                    TooltipLine axeLine = new(Mod, "SlashBonus", "Slash Bonus: Small chance to decapitate skeletons, killing them instantly") { overrideColor = Colors.RarityOrange };
                    tooltips.Add(axeLine);
                }
            }
            if (item.hammer > 0)
            {
                TooltipLine axeLine = new(Mod, "HammerBonus", "Hammer Bonus: Deals triple damage to Guard Points") { overrideColor = Colors.RarityOrange };
                tooltips.Add(axeLine);
            }

            if (!RedeConfigClient.Instance.ElementDisable)
            {
                if (ItemTags.Arcane.Has(item.type) || ProjectileTags.Arcane.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Arcane Bonus: Can hit enemies from the spirit realm") { overrideColor = Color.LightBlue };
                    tooltips.Add(line);
                }
                if (ItemTags.Blood.Has(item.type) || ProjectileTags.Blood.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Blood") { overrideColor = Color.IndianRed };
                    tooltips.Add(line);
                }
                if (ItemTags.Celestial.Has(item.type) || ProjectileTags.Celestial.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Celestial") { overrideColor = Color.Pink };
                    tooltips.Add(line);
                }
                if (ItemTags.Earth.Has(item.type) || ProjectileTags.Earth.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Earth") { overrideColor = Color.SandyBrown };
                    tooltips.Add(line);
                }
                if (ItemTags.Fire.Has(item.type) || ProjectileTags.Fire.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Fire Bonus: Chance to inflict a strong 'On Fire!' debuff on flammable enemies") { overrideColor = Color.Orange };
                    tooltips.Add(line);
                }
                if (ItemTags.Holy.Has(item.type) || ProjectileTags.Holy.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Holy Bonus: Increased damage to undead and demons") { overrideColor = Color.LightGoldenrodYellow };
                    tooltips.Add(line);
                }
                if (ItemTags.Ice.Has(item.type) || ProjectileTags.Ice.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Ice Bonus: Chance to freeze slimes and slow down infected enemies") { overrideColor = Color.LightCyan };
                    tooltips.Add(line);
                }
                if (ItemTags.Nature.Has(item.type) || ProjectileTags.Nature.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Nature") { overrideColor = Color.LawnGreen };
                    tooltips.Add(line);
                }
                if (ItemTags.Poison.Has(item.type) || ProjectileTags.Poison.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Poison") { overrideColor = Color.MediumPurple };
                    tooltips.Add(line);
                }
                if (ItemTags.Psychic.Has(item.type) || ProjectileTags.Psychic.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Psychic Bonus: Ignores enemy Guard Points") { overrideColor = Color.LightPink };
                    tooltips.Add(line);
                }
                if (ItemTags.Shadow.Has(item.type) || ProjectileTags.Shadow.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Shadow") { overrideColor = Color.MediumSlateBlue };
                    tooltips.Add(line);
                }
                if (ItemTags.Thunder.Has(item.type) || ProjectileTags.Thunder.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Thunder Bonus: Electrifies and deals extra damage if the target is in water") { overrideColor = Color.LightYellow };
                    tooltips.Add(line);
                }
                if (ItemTags.Water.Has(item.type) || ProjectileTags.Water.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Water Bonus: Increased damage to demons") { overrideColor = Color.SkyBlue };
                    tooltips.Add(line);
                }
                if (ItemTags.Wind.Has(item.type) || ProjectileTags.Wind.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Wind") { overrideColor = Color.LightGray };
                    tooltips.Add(line);
                }
            }
        }
    }
}