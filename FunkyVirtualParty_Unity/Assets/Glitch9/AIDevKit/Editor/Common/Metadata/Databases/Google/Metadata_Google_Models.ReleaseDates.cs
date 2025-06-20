using System.Collections.Generic;

namespace Glitch9.AIDevKit.Editor
{
    internal static partial class Metadata_Google_Models
    {
        internal static readonly Dictionary<string, UnixTime> ReleaseDates = new()
        {
            { "models/embedding-001", new UnixTime(2023, 3, 30) }, // 출시일: 2023년 3월 (추정)
            { "models/text-bison-001", new UnixTime(2023, 5, 1) }, // 출시일: 2023년 5월 (추정)
            { "models/embedding-gecko-001", new UnixTime(2023, 5, 1) }, // 출시일: 2023년 5월 (추정)
            { "models/chat-bison-001", new UnixTime(2023, 7, 10) }, // 출시일: 2023년 7월 10일

            { "models/gemini-1.5-flash-001", new UnixTime(2024, 5, 14) }, // 출시일: 2024년 5월 14일
            { "models/gemini-1.5-flash-001-tuning", new UnixTime(2024, 5, 14) }, // 출시일: 2024년 5월 14일
            { "models/gemini-1.5-pro-001", new UnixTime(2024, 5, 24) }, // 출시일: 2024년 5월 24일

            { "models/gemini-1.0-pro-vision-latest", new UnixTime(2024, 2, 15) }, // 출시일: 2024년 2월 15일
            { "models/gemini-pro-vision", new UnixTime(2024, 2, 15) }, // 출시일: 2024년 2월 15일

            { "models/text-embedding-004", new UnixTime(2024, 1, 10) }, // 출시일: 2024년 1월 10일

            { "models/gemini-exp-1206", new UnixTime(2024, 12, 6) }, // 출시일: 2024년 12월 6일 (추정)
            { "models/gemini-2.0-flash-thinking-exp-1219", new UnixTime(2024, 12, 19) }, // 출시일: 2024년 12월 19일 (추정)
            { "models/gemini-2.0-flash-exp", new UnixTime(2024, 12, 13) }, // 출시일: 2024년 12월 13일

            { "models/aqa", new UnixTime(2024, 11, 15) }, // 출시일: 2024년 11월 15일 (추정, AQA 소개 블로그 기준)
            { "models/learnlm-1.5-pro-experimental", new UnixTime(2024, 11, 21) }, // 출시일: 2024년 11월 21일

            { "models/gemini-1.5-flash-8b-exp-0827", new UnixTime(2024, 8, 27) }, // 출시일: 2024년 8월 27일 (추정)

            { "models/gemini-1.5-pro-002", new UnixTime(2024, 9, 24) }, // 출시일: 2024년 9월 24일
            { "models/gemini-1.5-pro", new UnixTime(2024, 9, 24) }, // 출시일: 2024년 9월 24일
            { "models/gemini-1.5-pro-latest", new UnixTime(2024, 9, 24) }, // 출시일: 2024년 9월 24일
            { "models/gemini-1.5-flash", new UnixTime(2024, 9, 24) }, // 출시일: 2024년 9월 24일
            { "models/gemini-1.5-flash-002", new UnixTime(2024, 9, 24) }, // 출시일: 2024년 9월 24일
            { "models/gemini-1.5-flash-latest", new UnixTime(2024, 9, 24) }, // 출시일: 2024년 9월 24일
            { "models/gemini-1.5-flash-8b-exp-0924", new UnixTime(2024, 9, 24) }, // 출시일: 2024년 9월 24일 (추정)

            { "models/gemini-1.5-flash-8b", new UnixTime(2024, 10, 1) }, // 출시일: 2024년 10월 (추정)
            { "models/gemini-1.5-flash-8b-001", new UnixTime(2024, 10, 1) }, // 출시일: 2024년 10월 (추정)
            { "models/gemini-1.5-flash-8b-latest", new UnixTime(2024, 10, 1) }, // 출시일: 2024년 10월 (추정)

            { "models/imagen-3.0-generate-002", new UnixTime(2024, 12, 1) }, // 출시일: 2024년 12월 (추정)

            { "models/gemini-2.0-flash-thinking-exp-01-21", new UnixTime(2025, 1, 21) }, // 출시일: 2025년 1월 21일
            { "models/gemini-2.0-flash-thinking-exp", new UnixTime(2025, 1, 21) }, // 출시일: 2025년 1월 21일

            { "models/gemini-2.0-flash", new UnixTime(2025, 2, 5) }, // 출시일: 2025년 2월 5일
            { "models/gemini-2.0-flash-001", new UnixTime(2025, 2, 5) }, // 출시일: 2025년 2월 5일
            { "models/gemini-2.0-flash-exp-image-generation", new UnixTime(2025, 2, 5) }, // 출시일: 2025년 2월 5일 (추정)
            { "models/gemini-2.0-flash-lite-001", new UnixTime(2025, 2, 5) }, // 출시일: 2025년 2월 5일
            { "models/gemini-2.0-flash-lite", new UnixTime(2025, 2, 5) }, // 출시일: 2025년 2월 5일
            { "models/gemini-2.0-flash-lite-preview-02-05", new UnixTime(2025, 2, 5) }, // 출시일: 2025년 2월 5일
            { "models/gemini-2.0-flash-lite-preview", new UnixTime(2025, 2, 5) }, // 출시일: 2025년 2월 5일
            { "models/gemini-2.0-pro-exp", new UnixTime(2025, 2, 5) }, // 출시일: 2025년 2월 5일 (추정)
            { "models/gemini-2.0-pro-exp-02-05", new UnixTime(2025, 2, 5) }, // 출시일: 2025년 2월 5일 (추정)

            { "models/gemini-embedding-exp", new UnixTime(2025, 3, 7) }, // 출시일: 2025년 3월 7일 (같은 모델로 간주)
            { "models/gemini-embedding-exp-03-07", new UnixTime(2025, 3, 7) }, // 출시일: 2025년 3월 7일

            { "models/learnlm-2.0-flash-experimental", new UnixTime(2025, 3, 1) }, // 출시일: 2025년 3월 (추정)

            { "models/gemma-3-1b-it", new UnixTime(2025, 3, 12) }, // 출시일: 2025년 3월 12일
            { "models/gemma-3-4b-it", new UnixTime(2025, 3, 12) }, // 출시일: 2025년 3월 12일
            { "models/gemma-3-12b-it", new UnixTime(2025, 3, 12) }, // 출시일: 2025년 3월 12일
            { "models/gemma-3-27b-it", new UnixTime(2025, 3, 12) }, // 출시일: 2025년 3월 12일

            { "models/gemini-2.5-pro-exp-03-25", new UnixTime(2025, 3, 25) }, // 출시일: 2025년 3월 25일
            { "models/gemini-2.5-pro-preview-03-25", new UnixTime(2025, 3, 25) }, // 출시일: 2025년 3월 25일

            { "models/gemini-2.5-flash-preview-04-17", new UnixTime(2025, 4, 17) }, // 출시일: 2025년 4월 17일 (추정)
            { "models/gemini-2.0-flash-live-001",  new UnixTime(2025, 4, 9) },
            { "models/veo-2.0-generate-001",  new UnixTime(2025, 4, 9) },
        };

    }
}