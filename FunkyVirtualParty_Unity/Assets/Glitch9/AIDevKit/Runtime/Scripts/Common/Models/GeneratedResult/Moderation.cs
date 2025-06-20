

namespace Glitch9.AIDevKit
{
    public class Moderation : GeneratedResult<SafetyRating>
    {
        public bool IsFlagged => IsFlaggedInternal();
        public string FlaggedReason => GetFlaggedReasonInternal();

        public Moderation(SafetyRating value, Usage usage = null) : base(value, usage) { }
        public Moderation(SafetyRating[] values, Usage usage = null) : base(values, usage) { }

        private bool? _isFlagged;
        private string _flaggedReason;

        private bool IsFlaggedInternal()
        {
            if (_isFlagged.HasValue) return _isFlagged.Value;

            if (Values.IsNullOrEmpty())
            {
                _isFlagged = false;
                return false;
            }

            foreach (SafetyRating rating in Values)
            {
                if (rating == null) continue;
                if (rating.IsFlagged)
                {
                    _isFlagged = true;
                    return true;
                }
            }

            _isFlagged = false;
            return false;
        }

        private string GetFlaggedReasonInternal()
        {
            if (_flaggedReason != null) return _flaggedReason;

            _flaggedReason = string.Empty;
            if (Values.IsNullOrEmpty()) return string.Empty;
            foreach (SafetyRating rating in Values)
            {
                if (rating == null || !rating.IsFlagged) continue;
                _flaggedReason += rating.ToString() + ", ";
            }

            if (_flaggedReason.EndsWith(", "))
            {
                _flaggedReason = _flaggedReason.Substring(0, _flaggedReason.Length - 2);
            }

            return _flaggedReason;
        }
    }
}