using Newtonsoft.Json;
using System;

namespace Glitch9
{
    public interface IResult
    {
        bool IsSuccess { get; }
        bool IsFailure => !IsSuccess;
        string ErrorMessage { get; }
    }

    /// <summary>
    /// Represents the base class for result objects, providing success, failure, and error states.
    /// </summary>
    public class Result : IResult
    {
        [JsonIgnore] public bool IsSuccess { get; set; } = true;
        [JsonIgnore] public bool IsFailure { get => !IsSuccess; set => IsSuccess = !value; }
        [JsonIgnore] public bool IsError { get => !IsSuccess; set => IsSuccess = !value; }

        // --- Success Handling --------------------------------------------------
        [JsonIgnore] public string Message { get; set; }  // Success Message

        // --- Error Handling ----------------------------------------------------
        [JsonIgnore] public virtual string ErrorMessage { get; set; }  // Error Message
        [JsonIgnore] public Exception Exception { get; set; }
        [JsonIgnore] public string StackTrace => Exception?.StackTrace;

        public static IResult Success() => new Result { IsSuccess = true };
        public static IResult Success(string message) => new Result { IsSuccess = true, Message = message };
        public static IResult Fail(string errorMessage = null) => new Result { IsSuccess = false, ErrorMessage = errorMessage };
        public static IResult Fail(Exception e, params string[] failReasons) => new Result { IsSuccess = false, Exception = e, ErrorMessage = failReasons.JoinWithSpace() };
    }

    /// <summary>
    /// Represents a result object that includes a data, providing success, failure, and error states.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    public class Result<T> : Result
    {
        /// <summary>
        /// Gets the value associated with the result.
        /// </summary>
        [JsonIgnore] public T Value { get; set; }

        /// <summary>
        /// Creates a successful result with a specified value.
        /// </summary>
        /// <param name="value">The value to associate with the result.</param>
        /// <returns>A successful <see cref="IResult"/> with a value.</returns>
        public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };

        /// <summary>
        /// Creates a failed result with a specified value and an optional failure reason.
        /// </summary>
        /// <param name="value">The value to associate with the result.</param>
        /// <param name="failReason">The reason for the failure.</param>
        /// <returns>A failed <see cref="IResult"/> with a value.</returns>
        public static Result<T> Fail(T value, string failReason = null) => new() { IsSuccess = false, ErrorMessage = failReason, Value = value };

        /// <summary>
        /// Creates an error result with a specified value, exception, and additional messages.
        /// </summary>
        /// <param name="value">The value to associate with the result.</param>
        /// <param name="ex">The exception causing the error.</param>
        /// <param name="failReasons">Additional messages to associate with the error.</param>
        /// <returns>An error <see cref="IResult"/> with a value.</returns>
        public static Result<T> Fail(T value, Exception ex, params string[] failReasons)
        => new() { IsSuccess = false, Exception = ex, ErrorMessage = ex.Message + failReasons.JoinWithSpace(), Value = value };
    }
}
