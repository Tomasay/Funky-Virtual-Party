using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    public enum Platform
    {
        Mobile,
        PC,
        Console,
    }

    public enum LanguageTone
    {
        Casual,
        Formal,
    }

    [Flags]
    public enum ArtStyle
    {
        Realistic = 1 << 0,
        Stylized = 1 << 1,
        Cartoon = 1 << 2,
        Anime = 1 << 3,
        PixelArt = 1 << 4,
        Minimalist = 1 << 5,
    }

    [Flags]
    public enum GameGenre
    {
        Action = 1 << 0,
        Adventure = 1 << 1,
        RPG = 1 << 2,
        Strategy = 1 << 3,
        Simulation = 1 << 4,
        Sports = 1 << 5,
        Puzzle = 1 << 6,
        Horror = 1 << 7,
        DatingSim = 1 << 8,
        Fighting = 1 << 9,
        Platformer = 1 << 10,
        Shooter = 1 << 11,
    }

    public enum GameTheme
    {
        Modern,
        Fantasy,
        SciFi,
        Cyberpunk,
        Historical,
        Horror,
        Steampunk,
        PostApocalyptic,
        Superhero,
        Mystery,
    }

    [Serializable]
    public class ProjectContext
    {
        [SerializeField] private Platform mainPlatform = Platform.Mobile;
        [SerializeField] private LanguageTone languageTone = LanguageTone.Casual;
        [SerializeField] private ArtStyle artStyle = ArtStyle.Stylized | ArtStyle.Cartoon;
        [SerializeField] private GameGenre gameGenre = GameGenre.DatingSim | GameGenre.Adventure;
        [SerializeField] private GameTheme gameTheme = GameTheme.Cyberpunk;
        [SerializeField] private string description = AIDevKitConfig.DefaultProjectDescription;
        [SerializeField] private bool is2D = false;
        [SerializeField] private bool isMultiplayer = false;
        [SerializeField] private bool isVR = false;

        public Platform MainPlatform => mainPlatform;
        public LanguageTone LanguageTone => languageTone;
        public GameGenre Genre => gameGenre;
        public GameTheme Theme => gameTheme;
        public ArtStyle ArtStyle => artStyle;
        public string Description => description;
        public bool Is2D => is2D;
        public bool IsMultiplayer => isMultiplayer;
        public bool IsVR => isVR;

        public override string ToString()
        {
            return GetFullDescription();
        }

        public string GetFullDescription()
        {
            using (StringBuilderPool.Get(out StringBuilder sb))
            {
                sb.Append("I'm developing ");

                // Platform
                sb.Append("a ");
                sb.Append(mainPlatform.ToString().ToLower());
                sb.Append(" game, ");

                // Genre
                AppendEnumFlagsDescription(sb, gameGenre, "genre");

                // Theme
                sb.Append(" set in a ");
                sb.Append(gameTheme.ToString().ToLower());
                sb.Append(" world, ");

                // Art style
                AppendEnumFlagsDescription(sb, artStyle, "style");

                sb.Append(". ");

                sb.Append(description);

                return sb.ToString();
            }
        }

        public string GetShortDescription()
        {
            using (StringBuilderPool.Get(out StringBuilder sb))
            {
                sb.Append("A ");

                AppendEnumFlagsDescription(sb, gameGenre, "genre");

                sb.Append(" game set in a ");
                sb.Append(gameTheme.ToString().ToLower());
                sb.Append(" world");

                sb.Append(".");

                return sb.ToString();
            }
        }

        private static void AppendEnumFlagsDescription<T>(StringBuilder sb, T flags, string label) where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            var selected = new List<string>();

            foreach (T val in values)
            {
                if (Convert.ToInt32(val) == 0) continue;
                if (((Enum)(object)flags).HasFlag(val))
                {
                    selected.Add(val.ToString().ToLower());
                }
            }

            if (selected.Count == 0) return;

            if (label == "genre")
                sb.Append(string.Join(" and ", selected)).Append(" ");
            else if (label == "style")
                sb.Append("with a ").Append(string.Join(" and ", selected)).Append(" art style");
        }
    }
}