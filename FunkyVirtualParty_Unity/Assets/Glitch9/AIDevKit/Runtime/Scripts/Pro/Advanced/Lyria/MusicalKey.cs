using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.Advanced.Lyria
{
    public enum MusicScale
    {
        [ApiEnum("C major / A minor", "C_MAJOR_A_MINOR")]
        CMajorAMinor,

        [ApiEnum("D♭ major / B♭ minor", "D_FLAT_MAJOR_B_FLAT_MINOR")]
        DFlatMajorBFlatMinor,

        [ApiEnum("D major / B minor", "D_MAJOR_B_MINOR")]
        DMajorBMinor,

        [ApiEnum("E♭ major / C minor", "E_FLAT_MAJOR_C_MINOR")]
        EFlatMajorCMinor,

        [ApiEnum("E major / C♯/D♭ minor", "E_MAJOR_D_FLAT_MINOR")]
        EMajorDFlatMinor,

        [ApiEnum("F major / D minor", "F_MAJOR_D_MINOR")]
        FMajorDMinor,

        [ApiEnum("G♭ major / E♭ minor", "G_FLAT_MAJOR_E_FLAT_MINOR")]
        GFlatMajorEFlatMinor,

        [ApiEnum("G major / E minor", "G_MAJOR_E_MINOR")]
        GMajorEMinor,

        [ApiEnum("A♭ major / F minor", "A_FLAT_MAJOR_F_MINOR")]
        AFlatMajorFMinor,

        [ApiEnum("A major / F♯/G♭ minor", "A_MAJOR_G_FLAT_MINOR")]
        AMajorGFlatMinor,

        [ApiEnum("B♭ major / G minor", "B_FLAT_MAJOR_G_MINOR")]
        BFlatMajorGMinor,

        [ApiEnum("B major / G♯/A♭ minor", "B_MAJOR_A_FLAT_MINOR")]
        BMajorAFlatMinor,

        [ApiEnum("Default / The model decides", "SCALE_UNSPECIFIED")]
        ScaleUnspecified
    }
}