namespace CramMods.STUMP.Settings
{
    public enum OverrideType
    {
        Diffuse,
        Color = Diffuse,
        Texture = Diffuse,

        Normal,
        Bump = Normal,
        MSN = Normal,

        Specular,
        Shine = Specular,
        S = Specular,

        Subsurface,
        Scatter = Subsurface,
        SK = Subsurface,
    }
}