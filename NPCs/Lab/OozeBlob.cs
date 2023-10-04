using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Items.Placeable.Banners;
using System.IO;
using Terraria.Localization;
using Redemption.Globals.NPC;

namespace Redemption.NPCs.Lab
{
    public class OozeBlob : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Bounce
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.ShimmerSlime;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Infected);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCWater[Type] = true;
            ElementID.NPCPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 16;
            NPC.height = 16;
            NPC.friendly = false;
            NPC.damage = 20;
            NPC.defense = 0;
            NPC.lifeMax = 100;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            NPC.value = 0f;
            NPC.knockBackResist = 0.6f;
            NPC.scale = 0.8f;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<LabBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<OozeBlobBanner>();
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.OozeBlob"))
            });
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Xvel);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Xvel = reader.ReadInt32();
        }
        public int Xvel;
        public int consumed;
        public override void OnSpawn(IEntitySource source)
        {
            TimerRand = Main.rand.Next(10, 30);
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            if (Main.LocalPlayer.InModBiome<LabBiome>())
                NPC.DiscourageDespawn(60);

            NPC.height = (int)(16 * NPC.scale);
            NPC.width = (int)(16 * NPC.scale);
            switch (AIState)
            {
                case ActionState.Idle:
                    NPC.LookByVelocity();
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.5f;

                    if (AITimer++ >= TimerRand && (NPC.collideY || NPC.velocity.Y == 0))
                    {
                        Xvel = Main.rand.Next(2, 5);
                        NPC.velocity.X = Xvel * (Main.rand.NextBool(2) ? 1 : -1);
                        NPC.velocity.Y -= Main.rand.Next(4, 7);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(10, 30);
                        AIState = ActionState.Bounce;
                        NPC.netUpdate = true;
                    }
                    break;

                case ActionState.Bounce:
                    NPC.LookByVelocity();
                    if (NPC.collideX)
                        NPC.velocity.X *= -0.7f;
                    if (NPC.collideY || NPC.velocity.Y == 0)
                        AIState = ActionState.Idle;
                    break;
            }

            NPC.DamageHostileAttackers(0, 1);

            if (NPC.scale < 10)
            {
                if (Main.rand.NextBool(100))
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC ooze = Main.npc[i];
                        if (!ooze.active || NPC.whoAmI == ooze.whoAmI || NPC.scale < ooze.scale || ooze.type != Type || !NPC.Hitbox.Intersects(ooze.Hitbox))
                            continue;

                        SoundEngine.PlaySound(SoundID.Item2, NPC.position);
                        BaseAI.DamageNPC(ooze, ooze.lifeMax + 10, 0, NPC, false, true);
                        Consumption();
                    }
                }
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (!target.active || target.dontTakeDamage || target.immortal || NPC.whoAmI == target.whoAmI || target.type == Type || target.life >= NPC.damage ||
                        NPC.height < target.height - 8 || NPC.width < target.width - 8 || target.boss || NPCLists.Inorganic.Contains(target.type) ||
                        NPCLists.Spirit.Contains(target.type) || !NPC.Hitbox.Intersects(target.Hitbox))
                        continue;

                    SoundEngine.PlaySound(SoundID.Item2, NPC.position);
                    BaseAI.DamageNPC(target, NPC.damage + 10, 0, NPC, false, true);
                    Consumption();
                }
            }
        }
        public void Consumption()
        {
            NPC.scale += 0.1f;
            NPC.position.Y -= 6;
            NPC.HealEffect(50);
            NPC.lifeMax += 50;
            NPC.life += 50;
            NPC.damage += 5;
            NPC.knockBackResist -= 0.05f;
            NPC.knockBackResist = MathHelper.Clamp(NPC.knockBackResist, 0, 1);
            consumed++;
            NPC.netUpdate = true;
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.frameCounter++ >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 3 * frameHeight)
                        NPC.frame.Y = 0;
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 4 * frameHeight;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SludgeDust>());
            }
            Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SludgeDust>());
        }
        public override void OnKill()
        {
            if (consumed > 2)
            {
                for (int i = 0; i < consumed / 2; i++)
                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + Main.rand.Next(0, NPC.width), (int)NPC.position.Y + Main.rand.Next(0, NPC.height), ModContent.NPCType<OozeBlob>());
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), (int)(Main.rand.Next(60, 300) * (NPC.scale * 2)));
        }
    }
}