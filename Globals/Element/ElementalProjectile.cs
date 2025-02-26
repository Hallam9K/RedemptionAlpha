using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.Globals
{
    public class ElementalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public sbyte[] OverrideElement = new sbyte[16];
        public bool[] InheritElement = new bool[16]; //Only known to the client

        public override GlobalProjectile Clone(Projectile from, Projectile to)
        {
            var clone = (ElementalProjectile)base.Clone(from, to);

            clone.OverrideElement = new sbyte[OverrideElement.Length];
            Array.Copy(OverrideElement, clone.OverrideElement, clone.OverrideElement.Length);

            clone.InheritElement = new bool[InheritElement.Length];
            Array.Copy(InheritElement, clone.InheritElement, clone.InheritElement.Length);

            return clone;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.GetFirstElement() > 0)
                return;
            if (source is EntitySource_ItemUse_WithAmmo itemAmmo && ElementID.ProjectilesInheritElements[itemAmmo.Item.type])
            {
                for (int i = ElementID.Arcane; i <= ElementID.Explosive; i++)
                {
                    if (itemAmmo.Item.HasElement(i))
                    {
                        InheritElement[i] = true;
                        OverrideElement[i] = ElementID.AddElement;
                    }
                }
            }
            else if (source is EntitySource_ItemUse item && ElementID.ProjectilesInheritElements[item.Item.type])
            {
                for (int i = ElementID.Arcane; i <= ElementID.Explosive; i++)
                {
                    if (item.Item.HasElement(i))
                    {
                        InheritElement[i] = true;
                        OverrideElement[i] = ElementID.AddElement;
                    }
                }
            }
            else if (source is EntitySource_Parent parent && parent.Entity is Projectile proj)
            {
                for (int i = ElementID.Arcane; i <= ElementID.Explosive; i++)
                {
                    if ((proj.HasElement(i) && ElementID.ProjectilesInheritElementsFromThis[proj.type]) || proj.GetGlobalProjectile<ElementalProjectile>().InheritElement[i])
                    {
                        InheritElement[i] = true;
                        OverrideElement[i] = ElementID.AddElement;
                    }
                }
            }
        }

        #region Element Syncing
        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            //Send all inherited elements
            List<byte> elements = new();
            for (sbyte i = 0; i <= ElementID.Explosive; i++)
            {
                if (InheritElement[i])
                {
                    elements.Add((byte)i);
                }
            }

            int count = elements.Count;
            bitWriter.WriteBit(count == 0);
            if (count == 0)
            {
                return;
            }

            binaryWriter.Write7BitEncodedInt(count);
            foreach (var e in elements)
            {
                //16 elements fit into 4 bits (2^4 = 16) from 0000 to 1111
                BitsByte bits = e;
                bitWriter.WriteBit(bits[0]);
                bitWriter.WriteBit(bits[1]);
                bitWriter.WriteBit(bits[2]);
                bitWriter.WriteBit(bits[3]);
            }
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            bool empty = bitReader.ReadBit();
            if (empty)
                return;

            int count = binaryReader.Read7BitEncodedInt();
            for (int i = 0; i < count; i++)
            {
                BitsByte bits = 0;
                bits[0] = bitReader.ReadBit();
                bits[1] = bitReader.ReadBit();
                bits[2] = bitReader.ReadBit();
                bits[3] = bitReader.ReadBit();
                OverrideElement[bits] = ElementID.AddElement;
            }
        }
        #endregion

        public override void ModifyHitNPC(Projectile projectile, Terraria.NPC target, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (!RedeConfigServer.Instance.ElementDisable)
            {
                if (projectile.HasElement(ElementID.Explosive))
                    modifiers.ScalingArmorPenetration += .2f;
            }
        }
        public override void OnHitNPC(Projectile projectile, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone)
        {
            if (!RedeConfigServer.Instance.ElementDisable)
            {
                if (Main.player[projectile.owner].RedemptionPlayerBuff().hydraCorrosion && projectile.HasElement(ElementID.Poison))
                    target.AddBuff(BuffType<HydraAcidDebuff>(), 240);
            }
        }
    }
}