using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.WorldGeneration;
using Terraria.Audio;
using Redemption.Base;
using Redemption.Biomes;
using Terraria.Localization;

namespace Redemption.NPCs.Lab.Blisterface
{
    public class Blisterface_Inactive : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Lab/Blisterface/Blisterface";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("");
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 96;
            NPC.height = 64;
            NPC.friendly = false;
            NPC.damage = 100;
            NPC.defense = 10;
            NPC.lifeMax = 32520;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(0, 10, 0, 0);
            NPC.knockBackResist = 0.0f;
            NPC.SpawnWithHigherTime(30);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = true;
            AnimationType = NPCID.Piranha;
            NPC.ShowNameOnHover = false;
        }
        public override bool CheckActive()
        {
            return !Main.LocalPlayer.InModBiome<LabBiome>();
        }
        public override void AI()
        {
            if (NPC.Center.Y < (RedeGen.LabVector.Y + 186) * 16)
                NPC.velocity.Y += 0.1f;
            if (NPC.Center.Y > (RedeGen.LabVector.Y + 191) * 16)
                NPC.velocity.Y -= 0.1f;

            Player player = Main.LocalPlayer;
            Rectangle activeZone = new((int)(RedeGen.LabVector.X + 204) * 16, (int)(RedeGen.LabVector.Y + 168) * 16, 7 * 16, 20 * 16);
            if (player.Hitbox.Intersects(activeZone) && !player.dead && player.active)
            {
                if (!Main.dedServ)
                {
                    RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.Blisterface.Name"), 60, 90, 0.8f, 0, Color.Green, Language.GetTextValue("Mods.Redemption.TitleCard.Blisterface.Modifier"));
                    SoundEngine.PlaySound(CustomSounds.SpookyNoise, NPC.position);
                }
                NPC.SetDefaults(ModContent.NPCType<Blisterface>());
                NPC.netUpdate = true;
            }
            BaseAI.AIFish(NPC, ref NPC.ai, true);
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 30)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 7 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
    }
}