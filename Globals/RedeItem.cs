using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Items.Usable;
using Redemption.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using System.Linq;
using Redemption.NPCs.Critters;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Accessories.PreHM;
using ReLogic.Content;

namespace Redemption.Globals
{
    public class RedeItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            return base.Clone(item, itemClone);
        }
        public bool TechnicallyHammer;
        public bool TechnicallyAxe;

        public override void ModifyHitNPC(Item item, Terraria.Player player, Terraria.NPC target, ref int damage,
            ref float knockBack, ref bool crit)
        {
            if ((item.axe > 0 || TechnicallyAxe) && crit)
                damage += damage / 2;
        }
        public override bool OnPickup(Item item, Terraria.Player player)
        {
            if (item.type == ItemID.Heart && player.RedemptionPlayerBuff().heartInsignia)
                player.AddBuff(ModContent.BuffType<HeartInsigniaBuff>(), 180);

            return true;
        }
        public override void PostUpdate(Item item)
        {
            if (item.type == ItemID.Heart && Main.LocalPlayer.RedemptionPlayerBuff().heartInsignia)
            {
                if (!Main.rand.NextBool(6))
                    return;

                int sparkle = Dust.NewDust(new Vector2(item.position.X, item.position.Y), item.width, item.height,
                    DustID.ShadowbeamStaff, Scale: 2);
                Main.dust[sparkle].velocity.X = 0;
                Main.dust[sparkle].velocity.Y = -2;
                Main.dust[sparkle].noGravity = true;
            }
            if (item.type == ItemID.GoldCrown || item.type == ItemID.PlatinumCrown)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    Terraria.NPC chicken = Main.npc[i];
                    if (!chicken.active || chicken.type != ModContent.NPCType<Chicken>())
                        continue;

                    if (chicken.frame.Y != 488 && chicken.frame.Y != 532)
                        continue;

                    if (!item.Hitbox.Intersects(chicken.Hitbox))
                        continue;

                    SoundEngine.PlaySound(SoundID.Item68, item.position);
                    SoundEngine.PlaySound(CustomSounds.Choir with { Pitch = 0.1f }, item.position);
                    RedeDraw.SpawnExplosion(item.Center, Color.White, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow3", AssetRequestMode.ImmediateLoad).Value);
                    chicken.active = false;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Item.NewItem(item.GetSource_Loot(), item.getRect(), ModContent.ItemType<CrownOfTheKing>(), item.stack);
                        item.active = false;
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI);
                    }
                }
            }    
        }

        readonly int[] bannedArenaItems = new int[]
        {
            ItemID.RodofDiscord,
            ItemID.IceRod,
            ItemID.PortalGun,
            ItemID.MagicMirror,
            ItemID.IceMirror,
            ItemID.CellPhone,
            ItemID.StaticHook,
            ItemID.AntiGravityHook,
            ItemID.Sandgun,
            ItemID.ActuationRod,
            ItemID.GravitationPotion
        };
        public override bool CanUseItem(Item item, Terraria.Player player)
        {
            if (ArenaWorld.arenaActive && bannedArenaItems.Any(x => x == item.type))
                return false;

            return base.CanUseItem(item, player);
        }
        public override void OnCreate(Item item, ItemCreationContext context)
        {
            if (item.type == ModContent.ItemType<AlignmentTeller>())
                RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Greetings, I am the Chalice of Alignment, and I believe any action can be redeemed.", 260, 30, 0, Color.DarkGoldenrod);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine axeLine = new(Mod, "AxeBonus", "Axe Bonus: 3x critical strike damage, increased chance to decapitate skeletons") { OverrideColor = Colors.RarityOrange };

            if ((item.CountsAsClass(DamageClass.Melee) && item.damage >= 4 && item.useStyle == ItemUseStyleID.Swing && !item.noUseGraphic))
            {
                if (item.axe > 0)
                    tooltips.Add(axeLine);

                else if (!ItemTags.BluntSwing.Has(item.type) && item.hammer == 0 && item.pick == 0)
                {
                    TooltipLine slashLine = new(Mod, "SlashBonus", "Slash Bonus: Small chance to decapitate skeletons, killing them instantly") { OverrideColor = Colors.RarityOrange };
                    tooltips.Add(slashLine);
                }
            }
            if (TechnicallyAxe)
                tooltips.Add(axeLine);

            if (item.hammer > 0 || item.type == ItemID.PaladinsHammer || TechnicallyHammer)
            {
                TooltipLine hammerLine = new(Mod, "HammerBonus", "Hammer Bonus: Deals quadruple damage to Guard Points") { OverrideColor = Colors.RarityOrange };
                tooltips.Add(hammerLine);
            }

            if (!RedeConfigClient.Instance.ElementDisable && !ItemTags.NoElement.Has(item.type) && !ProjectileTags.NoElement.Has(item.shoot))
            {
                if (ItemTags.Arcane.Has(item.type) || ProjectileTags.Arcane.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Arcane Bonus: Can hit enemies from the spirit realm") { OverrideColor = Color.LightBlue };
                    tooltips.Add(line);
                }
                if (ItemTags.Blood.Has(item.type) || ProjectileTags.Blood.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Blood Bonus: Increased damage to organic enemies, but decreased to robotic") { OverrideColor = Color.IndianRed };
                    tooltips.Add(line);
                }
                if (ItemTags.Celestial.Has(item.type) || ProjectileTags.Celestial.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Celestial") { OverrideColor = Color.Pink };
                    tooltips.Add(line);
                }
                if (ItemTags.Earth.Has(item.type) || ProjectileTags.Earth.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Earth Bonus: Deals extra damage and has a chance to stun grounded enemies") { OverrideColor = Color.SandyBrown };
                    tooltips.Add(line);
                }
                if (ItemTags.Fire.Has(item.type) || ProjectileTags.Fire.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Fire Bonus: Chance to inflict a strong 'On Fire!' debuff on flammable enemies") { OverrideColor = Color.Orange };
                    tooltips.Add(line);
                }
                if (ItemTags.Holy.Has(item.type) || ProjectileTags.Holy.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Holy Bonus: Increased damage to undead and demons") { OverrideColor = Color.LightGoldenrodYellow };
                    tooltips.Add(line);
                }
                if (ItemTags.Ice.Has(item.type) || ProjectileTags.Ice.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Ice Bonus: Chance to freeze slimes and slow down infected enemies") { OverrideColor = Color.LightCyan };
                    tooltips.Add(line);
                }
                if (ItemTags.Nature.Has(item.type) || ProjectileTags.Nature.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Nature") { OverrideColor = Color.LawnGreen };
                    tooltips.Add(line);
                }
                if (ItemTags.Poison.Has(item.type) || ProjectileTags.Poison.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Poison Bonus: Increased damage to poisoned enemies") { OverrideColor = Color.MediumPurple };
                    tooltips.Add(line);
                }
                if (ItemTags.Psychic.Has(item.type) || ProjectileTags.Psychic.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Psychic Bonus: Ignores enemy Guard Points") { OverrideColor = Color.LightPink };
                    tooltips.Add(line);
                }
                if (ItemTags.Shadow.Has(item.type) || ProjectileTags.Shadow.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Shadow Bonus: Slain enemies have a chance to drop a pickup which increases Shadow damage") { OverrideColor = Color.MediumSlateBlue };
                    tooltips.Add(line);
                }
                if (ItemTags.Thunder.Has(item.type) || ProjectileTags.Thunder.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Thunder Bonus: Electrifies and deals extra damage if the target is in water") { OverrideColor = Color.LightYellow };
                    tooltips.Add(line);
                }
                if (ItemTags.Water.Has(item.type) || ProjectileTags.Water.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Water Bonus: Increased damage to demons and can electrify robotic targets") { OverrideColor = Color.SkyBlue };
                    tooltips.Add(line);
                }
                if (ItemTags.Wind.Has(item.type) || ProjectileTags.Wind.Has(item.shoot))
                {
                    TooltipLine line = new(Mod, "Element", "Wind Bonus: Deals extra knockback to airborne targets") { OverrideColor = Color.LightGray };
                    tooltips.Add(line);
                }
            }
            if (item.rare == ModContent.RarityType<DonatorRarity>())
            {
                TooltipLine donatorLine = new(Mod, "DonatorLine", "-Donator Item-") { OverrideColor = Color.SpringGreen };
                tooltips.Add(donatorLine);
            }
        }
    }
}