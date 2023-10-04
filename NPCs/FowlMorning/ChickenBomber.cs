using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Items.Usable.Potions;
using Redemption.Dusts;
using Redemption.Globals.World;
using Redemption.Globals;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Weapons.PreHM.Ranged;
using Terraria.DataStructures;
using Redemption.NPCs.Minibosses.FowlEmperor;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Biomes;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Accessories.PreHM;
using Terraria.Localization;

namespace Redemption.NPCs.FowlMorning
{
    public class ChickenBomber : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Velocity = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 34;
            NPC.height = 40;
            NPC.friendly = false;
            NPC.damage = 20;
            NPC.defense = 2;
            NPC.lifeMax = 18;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 20;
            NPC.knockBackResist = 0.4f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<FowlMorningBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ChickenBomberBanner>();
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 4; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Smoke,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust3>(),
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                    Main.dust[dust].velocity *= 3f;
                }
            }
            if (Main.rand.NextBool(2))
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust3>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.ai[1] = Main.rand.NextBool() ? 1 : -1;
            NPC.ai[2] = Main.rand.Next(180, 251);
            NPC.netUpdate = true;
        }
        private float bombOpacity = 1;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.LookByVelocity();
            if (NPC.DespawnHandler(3))
                return;
            if (NPC.ai[0]++ % 120 == 0)
            {
                if (NPC.ai[1] != 1)
                    NPC.ai[1] = 1;
                else
                    NPC.ai[1] = -1;
            }

            NPC.Move(player.Center + new Vector2(100 * NPC.ai[1], -NPC.ai[2]), 7, 30);

            if (NPC.ai[3] == 1)
            {
                bombOpacity += 0.01f;
                if (bombOpacity >= 1)
                {
                    bombOpacity = 1;
                    NPC.ai[3] = 0;
                }
                return;
            }
            if (NPC.ai[0] % 80 == 0 && Main.rand.NextBool() && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 11), Vector2.Zero, ModContent.ProjectileType<Rooster_EggBomb>(), NPCHelper.HostileProjDamage(NPC.damage), 3);
                Main.projectile[p].rotation = MathHelper.PiOver2 * NPC.spriteDirection;
                Main.projectile[p].netUpdate = true;
                bombOpacity = 0;
                NPC.ai[3] = 1;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.rotation = NPC.velocity.X * 0.05f;
            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override bool PreKill()
        {
            if (FowlMorningWorld.FowlMorningActive && Main.netMode != NetmodeID.MultiplayerClient)
            {
                FowlMorningWorld.ChickPoints += Main.expertMode ? 4 : 2;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            return true;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EggBomb>(), 2, 4, 6));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GreneggLauncher>(), 60));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Grain>(), 200));
            npcLoot.Add(ItemDropRule.ByCondition(new OnFireCondition(), ModContent.ItemType<FriedChicken>(), 4));
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.life <= 0 && Main.rand.NextBool(4))
            {
                if (item.HasElement(ElementID.Fire))
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
                else if (NPC.FindBuffIndex(BuffID.OnFire) != -1 || NPC.FindBuffIndex(BuffID.OnFire3) != -1)
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
            }
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.life <= 0 && Main.rand.NextBool(4))
            {
                if (projectile.HasElement(ElementID.Fire))
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
                else if (NPC.FindBuffIndex(BuffID.OnFire) != -1 || NPC.FindBuffIndex(BuffID.OnFire3) != -1)
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D bombTex = ModContent.Request<Texture2D>(Texture + "_Bomb").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            spriteBatch.Draw(bombTex, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor) * bombOpacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            return false;
        }
        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.ChickenBomber"))
            });
        }
    }
}