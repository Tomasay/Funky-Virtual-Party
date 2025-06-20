using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Glitch9
{
    /// <summary>
    /// A collection of colors that are not defined in UnityEngine.Color
    /// </summary>
    public readonly struct ExColor
    {
        private static readonly ConcurrentDictionary<string, Color> _colorCache = new();
        private static Color GetColor(string colorName, Func<Color> colorCreator)
        {
            if (_colorCache.TryGetValue(colorName, out Color cachedColor))
            {
                return cachedColor;
            }

            Color newColor = colorCreator();
            _colorCache[colorName] = newColor;
            return newColor;
        }

#pragma warning disable IDE1006
        /// <summary>
        /// 투명색. 완전히 투명한 색을 나타냅니다. (#000000)
        /// </summary>
        public static Color transparent => GetColor(nameof(transparent), () => new Color(0f, 0f, 0f, 0f));

        /// <summary>
        /// 오렌지색. 주황색을 나타냅니다. (#E06262)
        /// </summary>
        public static Color orange => GetColor(nameof(orange), () => new Color(.88f, 0.387f, 0f));

        /// <summary>
        /// 황토색. 황토색을 나타냅니다. (#FFBF00)
        /// </summary>
        public static Color amber => GetColor(nameof(amber), () => new Color(1f, 0.75f, 0f));

        /// <summary>
        /// 밀색. 밝은 크림색을 나타냅니다. (#FFFED3)
        /// </summary>
        public static Color cream => GetColor(nameof(cream), () => new Color(1f, 0.992f, 0.816f));

        /// <summary>
        /// 분홍색. 연한 분홍색을 나타냅니다. (#FFC0CB)
        /// </summary>
        public static Color pink => GetColor(nameof(pink), () => new Color(1f, 0.753f, 0.796f));

        /// <summary>
        /// 보라색. 짙은 보라색을 나타냅니다. (#800080)
        /// </summary>
        public static Color purple => GetColor(nameof(purple), () => new Color(0.502f, 0f, 0.502f));

        /// <summary>
        /// 바이올렛색. 분홍 빛이 도는 진한 보라색을 나타냅니다. (#FFC0CB)
        /// </summary>
        public static Color violet => GetColor(nameof(violet), () => new Color(0.933f, 0.51f, 0.933f));

        /// <summary>
        /// 인디고색. 주로 보라와 파란 사이의 색을 나타냅니다. (#4B0082)
        /// </summary>
        public static Color indigo => GetColor(nameof(indigo), () => new Color(0.294f, 0f, 0.51f));

        /// <summary>
        /// 연기색. 연기가 낀 듯 어두운 회색을 나타냅니다. (#FFFFFF)
        /// </summary>
        public static Color smoke => GetColor(nameof(smoke), () => new Color(0.961f, 0.961f, 0.961f));

        /// <summary>
        /// 회색. 밝은 회색을 나타냅니다. (#C0C0C0)
        /// </summary>
        public static Color ash => GetColor(nameof(ash), () => new Color(0.698f, 0.745f, 0.71f));

        /// <summary>
        /// 스틸색. 중간 밝기의 회색을 나타냅니다. (#808080)
        /// </summary>
        public static Color steel => GetColor(nameof(steel), () => new Color(0.502f, 0.502f, 0.502f));

        /// <summary>
        /// 숯색. 짙은 회색을 나타냅니다. (#363636)
        /// </summary>
        public static Color charcoal => GetColor(nameof(charcoal), () => new Color(0.212f, 0.212f, 0.212f));

        /// <summary>
        /// 그림자색. 아주 어두운 회색을 나타냅니다. (#202020)
        /// </summary>
        public static Color shadow => GetColor(nameof(shadow), () => new Color(0.125f, 0.125f, 0.125f));

        /// <summary>
        /// 라이코리스색. 매우 어두운 회색을 나타냅니다. (#1A1A1A)
        /// </summary>
        public static Color licorice => GetColor(nameof(licorice), () => new Color(0.102f, 0.102f, 0.102f));

        /// <summary>
        /// 베이지색. 연한 베이지색을 나타냅니다. (#F5F5DC)
        /// </summary>
        public static Color beige => GetColor(nameof(beige), () => new Color(0.96f, 0.96f, 0.86f));

        /// <summary>
        /// 코랄색. 주황빛을 띤 분홍색을 나타냅니다. (#FF7F50)
        /// </summary>
        public static Color coral => GetColor(nameof(coral), () => new Color(1.0f, 0.5f, 0.31f));

        /// <summary>
        /// 후크샤색. 강렬한 자홍색을 나타냅니다. (#FF00FF)
        /// </summary>
        public static Color fuchsia => GetColor(nameof(fuchsia), () => new Color(1.0f, 0.0f, 1.0f));

        /// <summary>
        /// 골드색. 금색을 나타냅니다. (#FFD700)
        /// </summary>
        public static Color gold => GetColor(nameof(gold), () => new Color(1.0f, 0.84f, 0.0f));

        /// <summary>
        /// 상아색. 백색을 가미한 연한 베이지색을 나타냅니다. (#FFFFF0)
        /// </summary>
        public static Color ivory => GetColor(nameof(ivory), () => new Color(1.0f, 1.0f, 0.94f));

        /// <summary>
        /// 카키색. 노란 갈색을 나타냅니다. (#F0E68C)
        /// </summary>
        public static Color khaki => GetColor(nameof(khaki), () => new Color(0.94f, 0.9f, 0.55f));

        /// <summary>
        /// 라벤더색. 연한 보라색을 나타냅니다. (#E6E6FA)
        /// </summary>
        public static Color lavender => GetColor(nameof(lavender), () => new Color(0.9f, 0.9f, 0.98f));

        /// <summary>
        /// 마룬색. 짙은 갈색을 나타냅니다. (#800000)
        /// </summary>
        public static Color maroon => GetColor(nameof(maroon), () => new Color(0.5f, 0.0f, 0.0f));

        /// <summary>
        /// 네이비색. 짙은 파란색을 나타냅니다. (#000080)
        /// </summary>
        public static Color navy => GetColor(nameof(navy), () => new Color(0.0f, 0.0f, 0.5f));

        /// <summary>
        /// 올리브색. 연한 녹색에 갈색이 섞인 색을 나타냅니다. (#808000)
        /// </summary>
        public static Color olive => GetColor(nameof(olive), () => new Color(0.5f, 0.5f, 0.0f));

        /// <summary>
        /// 페리윙클색. 연한 보라색을 가미한 연한 파란색을 나타냅니다. (#CCCCFF)
        /// </summary>
        public static Color periwinkle => GetColor(nameof(periwinkle), () => new Color(0.8f, 0.8f, 1.0f));

        /// <summary>
        /// 자수정색. 자수정색을 나타냅니다. (#DDA0DD)
        /// </summary>
        public static Color plum => GetColor(nameof(plum), () => new Color(0.87f, 0.63f, 0.87f));

        /// <summary>
        /// 석영색. 연한 자색을 나타냅니다. (#D6D6E8)
        /// </summary>
        public static Color quartz => GetColor(nameof(quartz), () => new Color(0.85f, 0.85f, 0.95f));

        /// <summary>
        /// 샐몬색. 연한 주황빛을 띤 분홍색을 나타냅니다. (#FA8072)
        /// </summary>
        public static Color salmon => GetColor(nameof(salmon), () => new Color(0.98f, 0.5f, 0.45f));

        /// <summary>
        /// 황갈색. 연한 갈색을 나타냅니다. (#D2B48C)
        /// </summary>
        public static Color tan => GetColor(nameof(tan), () => new Color(0.82f, 0.71f, 0.55f));

        /// <summary>
        /// 청록색. 파란색과 녹색이 섞인 연한 청색을 나타냅니다. (#008080)
        /// </summary>
        public static Color teal => GetColor(nameof(teal), () => new Color(0.0f, 0.5f, 0.5f));

        /// <summary>
        /// 터쿼이즈색. 연한 청록색을 나타냅니다. (#40E0D0)
        /// </summary>
        public static Color turquoise => GetColor(nameof(turquoise), () => new Color(0.25f, 0.88f, 0.82f));

        /// <summary>
        /// 잉버색. 짙은 갈색을 나타냅니다. (#3B2F2F)
        /// </summary>
        public static Color umber => GetColor(nameof(umber), () => new Color(0.39f, 0.32f, 0.28f));

        /// <summary>
        /// 밀색. 연한 갈색을 가미한 베이지색을 나타냅니다. (#F5DEB3)
        /// </summary>
        public static Color wheat => GetColor(nameof(wheat), () => new Color(0.96f, 0.87f, 0.7f));

        /// <summary>
        /// 비취색. 밝은 청록색을 나타냅니다. (#00FF7F)
        /// </summary>
        public static Color jade => GetColor(nameof(jade), () => new Color(0.0f, 0.66f, 0.42f));

        /// <summary>
        /// 루비색. 진한 붉은색을 나타냅니다. (#E32636)
        /// </summary>
        public static Color ruby => GetColor(nameof(ruby), () => new Color(0.88f, 0.07f, 0.37f));

        /// <summary>
        /// 에메랄드색. 짙은 녹색을 나타냅니다. (#50C878)
        /// </summary>
        public static Color emerald => GetColor(nameof(emerald), () => new Color(0.31f, 0.78f, 0.47f));

        /// <summary>
        /// 황제색. 밝은 황금색을 나타냅니다. (#FFD700)
        /// </summary>
        public static Color topaz => GetColor(nameof(topaz), () => new Color(1.0f, 0.78f, 0.49f));

        /// <summary>
        /// 가넷색. 진한 붉은색을 나타냅니다. (#7B1113)
        /// </summary>
        public static Color garnet => GetColor(nameof(garnet), () => new Color(0.6f, 0.22f, 0.21f));

        /// <summary>
        /// 자수정색. 자수정색을 나타냅니다. (#9966CC)
        /// </summary>
        public static Color amethyst => GetColor(nameof(amethyst), () => new Color(0.6f, 0.4f, 0.8f));

        /// <summary>
        /// 시트린색. 노란색을 가미한 연한 갈색을 나타냅니다. (#E4D00A)
        /// </summary>
        public static Color citrine => GetColor(nameof(citrine), () => new Color(0.89f, 0.82f, 0.04f));

        /// <summary>
        /// 오닉스색. 검은색을 나타냅니다. (#0A0A0A)
        /// </summary>
        public static Color onyx => GetColor(nameof(onyx), () => new Color(0.06f, 0.06f, 0.06f));

        /// <summary>
        /// 진주색. 밝은 크림색을 나타냅니다. (#FAF0E6)
        /// </summary>
        public static Color pearl => GetColor(nameof(pearl), () => new Color(0.94f, 0.92f, 0.84f));

        /// <summary>
        /// 지르콘색. 밝은 청색을 나타냅니다. (#C0C0C0)
        /// </summary>
        public static Color zircon => GetColor(nameof(zircon), () => new Color(0.77f, 0.88f, 0.94f));

        /// <summary>
        /// 문스톤색. 밝은 회색을 나타냅니다. (#F0F0F0)
        /// </summary>
        public static Color moonstone => GetColor(nameof(moonstone), () => new Color(0.96f, 0.94f, 0.9f));

        /// <summary>
        /// 오팔색. 밝은 회색을 나타냅니다. (#D3D3D3)
        /// </summary>
        public static Color opal => GetColor(nameof(opal), () => new Color(0.9f, 0.88f, 0.84f));

        /// <summary>
        /// 말라카이트색. 밝은 녹색을 나타냅니다. (#0BDA51)
        /// </summary>
        public static Color malachite => GetColor(nameof(malachite), () => new Color(0.04f, 0.85f, 0.32f));

        /// <summary>
        /// 베릴색. 밝은 녹색을 나타냅니다. (#76FF7A)
        /// </summary>
        public static Color beryl => GetColor(nameof(beryl), () => new Color(0.76f, 1.0f, 0.0f));

        /// <summary>
        /// 코발트색. 짙은 파란색을 나타냅니다. (#0047AB)
        /// </summary>
        public static Color cobalt => GetColor(nameof(cobalt), () => new Color(0.0f, 0.28f, 0.67f));

        /// <summary>
        /// 파이어브릭색. 짙은 붉은색을 나타냅니다. (#B22222)
        /// </summary>
        public static Color firebrick => GetColor(nameof(firebrick), () => new Color(0.7f, 0.13f, 0.13f));

        /// <summary>
        /// 글라우커스색. 푸른색과 회색이 섞인 색을 나타냅니다. (#6082B6)
        /// </summary>
        public static Color glaucous => GetColor(nameof(glaucous), () => new Color(0.38f, 0.51f, 0.71f));

        /// <summary>
        /// 사파이어색. 진한 파란색을 나타냅니다. (#0F52BA)
        /// </summary>
        public static Color sapphire => GetColor(nameof(sapphire), () => new Color(0f, 0.4f, 0.75f));

        /// <summary>
        /// 아쥬르색. 청록색과 파란색이 섞인 색을 나타냅니다. (#007FFF)
        /// </summary>
        public static Color azure => GetColor(nameof(azure), () => new Color(0f, 0.67f, 1f));

        /// <summary>
        /// 딥씨색. 아주 어두운 파란색을 나타냅니다. (#001633)
        /// </summary>
        public static Color deepsea => GetColor(nameof(deepsea), () => new Color(0f, 0.2f, 0.4f));

        /// <summary>
        /// 티파니색. 청록색과 초록색이 섞인 색을 나타냅니다. (#0ABAB5)
        /// </summary>
        public static Color tiffany => GetColor(nameof(tiffany), () => new Color(0f, 0.75f, 0.65f));

        /// <summary>
        /// 아쿠아마린색. 청록색과 파란색이 섞인 연한 청록색을 나타냅니다. (#7FFFD4)
        /// </summary>
        public static Color aquamarine => GetColor(nameof(aquamarine), () => new Color(0f, 1f, 0.77f));

        /// <summary>
        /// 스칼렛색. 밝은 빨간색을 나타냅니다. (#FF2400)
        /// </summary>
        public static Color scarlet => GetColor(nameof(scarlet), () => new Color(1f, 0.24f, 0f));

        /// <summary>
        /// 로즈색. 연한 붉은색을 나타냅니다. (#FF007F)
        /// </summary>
        public static Color rose => GetColor(nameof(rose), () => new Color(1f, 0.27f, 0.33f));

        /// <summary>
        /// 파스텔색. 연한 색조를 가진 색을 나타냅니다. (#EAEAEA)
        /// </summary>
        public static Color pastel => GetColor(nameof(pastel), () => new Color(0.9f, 0.9f, 0.9f));

        /// <summary>
        /// 로열퍼플색. 진한 보라색을 나타냅니다. (#5A2D6B)
        /// </summary>
        public static Color royalpurple => GetColor(nameof(royalpurple), () => new Color(0.37f, 0.31f, 0.67f));

        /// <summary>
        /// 라일락색. 연한 자주색을 나타냅니다. (#C8A2C8)
        /// </summary>
        public static Color lilac => GetColor(nameof(lilac), () => new Color(0.79f, 0.81f, 0.96f));

        /// <summary>
        /// 페리윙클색. 연한 파란색을 나타냅니다. (#CCCCFF)
        /// </summary>
        public static Color periwrinkle => GetColor(nameof(periwrinkle), () => new Color(0.65f, 0.68f, 0.88f));

        /// <summary>
        /// 전나무색. 짙은 녹색을 나타냅니다. (#2E8B57)
        /// </summary>
        public static Color spruce => GetColor(nameof(spruce), () => new Color(0.19f, 0.31f, 0.31f));

        /// <summary>
        /// 고사리색. 짙은 녹색에 연한 녹색이 가미된 색을 나타냅니다. (#2E8B57)
        /// </summary>
        public static Color fern => GetColor(nameof(fern), () => new Color(0.36f, 0.73f, 0.39f));

        /// <summary>
        /// 클레멘타인색. 주황색과 연한 갈색이 섞인 색을 나타냅니다. (#FF7E00)
        /// </summary>
        public static Color clementine => GetColor(nameof(clementine), () => new Color(1f, 0.68f, 0f));

        /// <summary>
        /// 베이비핑크색. 연한 분홍색을 나타냅니다. (#FFB6C1)
        /// </summary>
        public static Color babypink => GetColor(nameof(babypink), () => new Color(1f, 0.75f, 0.8f));

        /// <summary>
        /// 허니색. 연한 갈색에 주황빛이 가미된 색을 나타냅니다. (#E88E5A)
        /// </summary>
        public static Color honey => GetColor(nameof(honey), () => new Color(0.93f, 0.59f, 0.02f));

        /// <summary>
        /// 파란색 틴트. 연한 파란색을 나타냅니다. (#E6F2FF)
        /// </summary>
        public static Color bluetint => GetColor(nameof(bluetint), () => new Color(0.9f, 0.95f, 1f));

        /// <summary>
        /// 초록색 틴트. 연한 초록색을 나타냅니다. (#E6FFE6)
        /// </summary>
        public static Color greentint => GetColor(nameof(greentint), () => new Color(0.9f, 1f, 0.95f));

        /// <summary>
        /// 빨간색 틴트. 연한 빨간색을 나타냅니다. (#FFE6E6)
        /// </summary>
        public static Color redtint => GetColor(nameof(redtint), () => new Color(1f, 0.9f, 0.95f));

#pragma warning restore IDE1006
    }
}