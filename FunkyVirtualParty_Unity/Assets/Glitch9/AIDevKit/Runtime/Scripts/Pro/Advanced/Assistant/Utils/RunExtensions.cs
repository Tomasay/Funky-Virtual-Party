using System;
using Cysharp.Threading.Tasks;

namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    internal static class RunExtensions
    {
        private readonly static RunStatus[] _successStatus = new[]
        {
            RunStatus.Completed,
            RunStatus.RequiresAction
        };

        private readonly static RunStatus[] _failureStatus = new[]
        {
            RunStatus.Expired,
            RunStatus.Cancelling,
            RunStatus.Cancelled,
            RunStatus.Failed
        };

        private readonly static RunStatus[] _terminalStatus = new[]
        {
            RunStatus.Completed,
            RunStatus.Incomplete,
            RunStatus.Expired,
            RunStatus.Cancelled,
            RunStatus.Failed
        };

        internal static RunStatus[] GetStatusArray(this RunStatusType type)
        {
            return type switch
            {
                RunStatusType.Success => _successStatus,
                RunStatusType.Failure => _failureStatus,
                RunStatusType.Terminal => _terminalStatus,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        internal static bool IsStatusType(this RunStatus? status, RunStatusType type)
        {
            RunStatus[] statusArray = GetStatusArray(type);
            return Array.Exists(statusArray, s => s == status);
        }

        internal static bool IsStatusType(this RunStatus status, RunStatusType type)
        {
            RunStatus[] statusArray = GetStatusArray(type);
            return Array.Exists(statusArray, s => s == status);
        }

        internal static async UniTask WaitUntil(this RunStatus status, RunStatusType type)
        {
            await UniTask.WaitUntil(() => status.IsStatusType(type));
        }
    }
}