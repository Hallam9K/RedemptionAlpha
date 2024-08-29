using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Thorn
{
    public class BrambleTrap : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = false;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.BloodButcherer] = false;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCNature[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 68;
            NPC.friendly = false;
            NPC.damage = 5;
            NPC.defense = 0;
            NPC.lifeMax = 10;
            NPC.HitSound = SoundID.Grass;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.chaseable = false;
            NPC.behindTiles = true;
            NPC.hide = true;
            NPC.ShowNameOnHover = false;
            NPC.spriteDirection = Main.rand.NextBool() ? -1 : 1;
            NPC.scale = Main.rand.NextFloat(.8f, 1.2f);
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsOverPlayers.Add(index);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.JungleGrass, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.JungleGrass, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 16; i++)
                        Gore.NewGore(NPC.GetSource_FromThis(), RedeHelper.RandAreaInEntity(NPC) - new Vector2(5, 6), Vector2.Zero, ModContent.Find<ModGore>("Redemption/ThornFX").Type, Main.rand.NextFloat(1, 1.3f));
                }
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (item.axe > 0)
                modifiers.SetCrit();
            if (ElementID.HasElement(item, ElementID.Fire))
                NPC.AddBuff(BuffID.OnFire3, 180);
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.Redemption().IsAxe)
                modifiers.SetCrit();
            if (ElementID.HasElement(projectile, ElementID.Fire))
                NPC.AddBuff(BuffID.OnFire3, 180);
        }
        public override void AI()
        {
            if (NPC.ai[0]++ % 10 == 0)
            {
                if (NPC.CountNPCS(Type) > 30)
                {
                    int firstTrap = NPC.FindFirstNPC(Type);
                    if (firstTrap >= 0 && Main.npc[firstTrap].active)
                        Main.npc[firstTrap].ai[1] = 1;
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<Thorn>()))
                    NPC.ai[1] = 1;
            }
            if (NPC.ai[1] > 0)
            {
                if (++NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y -= 76;
                    if (NPC.frame.Y < 0)
                        NPC.active = false;
                }
                return;
            }
            if (NPC.frame.Y >= 7 * 76)
            {
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active || player.dead || !Helper.CheckCircularCollision(NPC.Center, (int)(30 * NPC.scale), player.Hitbox))
                        continue;

                    player.AddBuff(ModContent.BuffType<EnsnaredDebuff>(), 10);
                }
            }
            else if (NPC.frame.Y <= 76)
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.frame.Y < 7 * 76 || !NPC.Hitbox.Intersects(npc.Hitbox))
                        continue;

                    NPC.ai[1] = 1;
                }
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.ai[1] > 0)
                return;
            if (++NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 8 * frameHeight)
                    NPC.frame.Y = 8 * frameHeight;
            }
        }
    }
}