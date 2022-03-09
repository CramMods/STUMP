using CramMods.NARFI;
using CramMods.STUMP.Settings;
using Mutagen.Bethesda;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace CramMods.STUMP
{
    public class BodyBuilder
    {
        private ISkyrimMod _mod;

        public BodyBuilder(ISkyrimMod patchMod) => _mod = patchMod;

        public IArmor BuildSkin(INpcGetter npc, VariantSettings variant)
        {
            BodyInfo body = MakeBasicBody(npc);

            List<ITextureSet> textures = new();

            if (variant.Overrides.Count(o => o.Part == OverridePart.Body) > 0)
            {
                ITextureSet tex = _mod.TextureSets.AddNew($"{GetNpcId(npc)}Torso");
                tex.Diffuse = GetOverridePath(variant, OverridePart.Body, OverrideType.Diffuse) ?? null;
                tex.NormalOrGloss = GetOverridePath(variant, OverridePart.Body, OverrideType.MSN) ?? null;
                tex.BacklightMaskOrSpecular = GetOverridePath(variant, OverridePart.Body, OverrideType.S) ?? null;
                tex.GlowOrDetailMap = GetOverridePath(variant, OverridePart.Body, OverrideType.SK) ?? null;

                tex.Flags = TextureSet.Flag.FaceGenTextures;
                if (tex.NormalOrGloss != null) tex.Flags |= TextureSet.Flag.HasModelSpaceNormalMap;
                if (tex.BacklightMaskOrSpecular == null) tex.Flags |= TextureSet.Flag.NoSpecularMap;

                if (GetNpcGender(npc) == Gender.Male) body.Torso.SkinTexture!.Male = tex.AsNullableLinkGetter();
                else body.Torso.SkinTexture!.Female = tex.AsNullableLinkGetter();

                textures.Add(tex);
            }

            if (variant.Overrides.Count(o => o.Part == OverridePart.Hands) > 0)
            {
                ITextureSet tex = _mod.TextureSets.AddNew($"{GetNpcId(npc)}Hands");
                tex.Diffuse = GetOverridePath(variant, OverridePart.Hands, OverrideType.Diffuse) ?? null;
                tex.NormalOrGloss = GetOverridePath(variant, OverridePart.Hands, OverrideType.MSN) ?? null;
                tex.BacklightMaskOrSpecular = GetOverridePath(variant, OverridePart.Hands, OverrideType.S) ?? null;
                tex.GlowOrDetailMap = GetOverridePath(variant, OverridePart.Hands, OverrideType.SK) ?? null;

                tex.Flags = TextureSet.Flag.FaceGenTextures;
                if (tex.NormalOrGloss != null) tex.Flags |= TextureSet.Flag.HasModelSpaceNormalMap;
                if (tex.BacklightMaskOrSpecular == null) tex.Flags |= TextureSet.Flag.NoSpecularMap;

                if (GetNpcGender(npc) == Gender.Male) body.Hands.SkinTexture!.Male = tex.AsNullableLinkGetter();
                else body.Hands.SkinTexture!.Female = tex.AsNullableLinkGetter();

                textures.Add(tex);
            }

            if (variant.Overrides.Count(o => o.Part == OverridePart.Feet) > 0)
            {
                ITextureSet tex = _mod.TextureSets.AddNew($"{GetNpcId(npc)}Feet");
                tex.Diffuse = GetOverridePath(variant, OverridePart.Feet, OverrideType.Diffuse) ?? null;
                tex.NormalOrGloss = GetOverridePath(variant, OverridePart.Feet, OverrideType.MSN) ?? null;
                tex.BacklightMaskOrSpecular = GetOverridePath(variant, OverridePart.Feet, OverrideType.S) ?? null;
                tex.GlowOrDetailMap = GetOverridePath(variant, OverridePart.Feet, OverrideType.SK) ?? null;

                tex.Flags = TextureSet.Flag.FaceGenTextures;
                if (tex.NormalOrGloss != null) tex.Flags |= TextureSet.Flag.HasModelSpaceNormalMap;
                if (tex.BacklightMaskOrSpecular == null) tex.Flags |= TextureSet.Flag.NoSpecularMap;

                if (GetNpcGender(npc) == Gender.Male) body.Feet.SkinTexture!.Male = tex.AsNullableLinkGetter();
                else body.Feet.SkinTexture!.Female = tex.AsNullableLinkGetter();

                textures.Add(tex);
            }

            FormList forms = _mod.FormLists.AddNew($"{GetNpcId(npc)}Forms");
            forms.Items.AddRange(textures);

            return body.Body;
        }

        private BodyInfo MakeBasicBody(INpcGetter npc)
        {
            IArmor body = _mod.Armors.AddNew($"{GetNpcId(npc)}Skin");

            body.MajorFlags = Armor.MajorFlag.NonPlayable;
            body.FormVersion = 44;
            body.ObjectBounds = new() { First = new(0, 0, 0), Second = new(0, 0, 0) };
            body.BodyTemplate = new() { ActsLike44 = true, ArmorType = ArmorType.Clothing, FirstPersonFlags = BipedObjectFlag.Head | BipedObjectFlag.Body | BipedObjectFlag.Hands | BipedObjectFlag.Feet };
            body.Race = npc.Race.AsNullable();
            body.Description = string.Empty;
            body.Value = 0;
            body.Weight = 0.0F;
            body.ArmorRating = 0.0F;

            IArmorAddon torso = MakeBasicBodyPart(npc, OverridePart.Body);
            body.Armature.Add(torso);

            IArmorAddon hands = MakeBasicBodyPart(npc, OverridePart.Hands);
            body.Armature.Add(hands);

            IArmorAddon feet = MakeBasicBodyPart(npc, OverridePart.Feet);
            body.Armature.Add(feet);

            return new()
            {
                Body = body,
                Torso = torso,
                Hands = hands,
                Feet = feet,
            };
        }

        private IArmorAddon MakeBasicBodyPart(INpcGetter npc, OverridePart partType)
        {
            string partName = partType switch { OverridePart.Body => "Torso", OverridePart.Hands => "Hands", OverridePart.Feet => "Feet", _ => throw new Exception() };

            IArmorAddon part = _mod.ArmorAddons.AddNew($"{GetNpcId(npc)}{partName}");

            part.FormVersion = 44;
            part.BodyTemplate = new() { ActsLike44 = true, ArmorType = ArmorType.Clothing };
            part.Race = npc.Race.AsNullable();
            part.Priority = new GenderedItem<byte>(0, 0);
            part.WeightSliderEnabled = new GenderedItem<bool>(true, true);
            part.Unknown = 0x0002;
            part.DetectionSoundValue = 0;
            part.Unknown2 = 0x17;
            part.WeaponAdjust = 0.0F;

            switch (partType)
            {
                case OverridePart.Body:
                    part.BodyTemplate.FirstPersonFlags = BipedObjectFlag.Body | BipedObjectFlag.Forearms | BipedObjectFlag.Amulet | BipedObjectFlag.Ring | BipedObjectFlag.Calves;
                    part.WorldModel = new GenderedItem<Model?>(new Model() { File = @"Actors\Character\Character Assets\MaleBody_1.nif" }, new Model() { File = @"Actors\Character\Character Assets\FemaleBody_1.nif" });
                    part.FirstPersonModel = new GenderedItem<Model?>(new Model() { File = @"Actors\Character\Character Assets\1stPersonMaleBody_1.nif" }, new Model() { File = @"Actors\Character\Character Assets\1stPersonFemaleBody_1.nif" });
                    part.SkinTexture = new GenderedItem<IFormLinkNullableGetter<ITextureSetGetter>>(Skyrim.TextureSet.SkinBodyMale_1.AsNullable(), Skyrim.TextureSet.SkinBodyFemale_1.AsNullable());
                    part.TextureSwapList = new GenderedItem<IFormLinkNullableGetter<IFormListGetter>>(Skyrim.FormList.SkinMaleHumanBody.AsNullable(), Skyrim.FormList.SkinFemaleHumanBody.AsNullable());
                    break;

                case OverridePart.Hands:
                    part.BodyTemplate.FirstPersonFlags = BipedObjectFlag.Hands;
                    part.WorldModel = new GenderedItem<Model?>(new Model() { File = @"Actors\Character\Character Assets\MaleHands_1.nif" }, new Model() { File = @"Actors\Character\Character Assets\FemaleHands_1.nif" });
                    part.FirstPersonModel = new GenderedItem<Model?>(new Model() { File = @"Actors\Character\Character Assets\1stPersonMaleHands_1.nif" }, new Model() { File = @"Actors\Character\Character Assets\1stPersonFemaleHands_1.nif" });
                    part.SkinTexture = new GenderedItem<IFormLinkNullableGetter<ITextureSetGetter>>(Skyrim.TextureSet.SkinHandMale_1.AsNullable(), Skyrim.TextureSet.SkinHandFemale_1.AsNullable());
                    part.TextureSwapList = new GenderedItem<IFormLinkNullableGetter<IFormListGetter>>(Skyrim.FormList.SkinMaleHumanHand.AsNullable(), FormLinkNullableGetter<IFormListGetter>.Null);
                    break;

                case OverridePart.Feet:
                    part.BodyTemplate.FirstPersonFlags = BipedObjectFlag.Feet;
                    part.WorldModel = new GenderedItem<Model?>(new Model() { File = @"Actors\Character\Character Assets\MaleFeet_1.nif" }, new Model() { File = @"Actors\Character\Character Assets\FemaleFeet_1.nif" });
                    part.SkinTexture = new GenderedItem<IFormLinkNullableGetter<ITextureSetGetter>>(Skyrim.TextureSet.SkinBodyMale_1.AsNullable(), Skyrim.TextureSet.SkinBodyFemale_1.AsNullable());
                    part.TextureSwapList = new GenderedItem<IFormLinkNullableGetter<IFormListGetter>>(Skyrim.FormList.SkinMaleHumanBody.AsNullable(), Skyrim.FormList.SkinFemaleHumanBody.AsNullable());
                    part.FootstepSound = Skyrim.FootstepSet.FSTBarefootFootstepSet.AsNullable();
                    break;

                default: throw new Exception();
            }

            return part;
        }

        private string GetNpcId(INpcGetter npc) => npc.EditorID ?? npc.FormKey.ToString();
        private Gender GetNpcGender(INpcGetter npc) => npc.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female) ? Gender.Female : Gender.Male;
        private string? GetOverridePath(VariantSettings variant, OverridePart part, OverrideType type) => variant.Overrides.Find(o => (o.Part == part) && (o.Type == type))?.Path;
    }

    public struct BodyInfo
    {
        public IArmor Body;
        public IArmorAddon Torso;
        public IArmorAddon Hands;
        public IArmorAddon Feet;
    }
}
