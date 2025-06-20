using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.ElevenLabs
{

    public enum SubscriptionStatus
    {
        [ApiEnum("Free Trial", "free")] Free,
        [ApiEnum("Free Trial", "trialing")] Trialing,
        [ApiEnum("Active", "active")] Active,
        [ApiEnum("Past Due", "past_due")] PastDue,
        [ApiEnum("Canceled", "canceled")] Canceled,
        [ApiEnum("Unpaid", "unpaid")] Unpaid,
        [ApiEnum("Incomplete", "incomplete")] Incomplete,
        [ApiEnum("Incomplete Expired", "incomplete_expired")] IncompleteExpired,
        [ApiEnum("Paused", "paused")] Paused
    }

    public enum SubscriptionCurrency
    {
        [ApiEnum("US Dollar", "usd")] Usd,
        [ApiEnum("Euro", "eur")] Eur
    }

    public enum TimePeriod
    {
        [ApiEnum("Monthly", "monthly_period")] Monthly,
        [ApiEnum("Annual", "annual_period")] Annual
    }

    public class UserSubscriptionResponse
    {
        [JsonProperty("subscription")] public UserSubscription Subscription { get; set; }
    }

    public class UserSubscription
    {
        /// <summary>
        /// Required. The tier of the user’s subscription.
        /// </summary>
        [JsonProperty("tier")] public string Tier { get; set; }

        /// <summary>
        /// Required. The number of characters used by the user.
        /// </summary>
        [JsonProperty("character_count")] public int CharacterCount { get; set; }

        /// <summary>
        /// Required. The maximum number of characters allowed in the current billing period.
        /// </summary>
        [JsonProperty("character_limit")] public int CharacterLimit { get; set; }

        /// <summary>
        /// Required. Whether the user can extend their character limit.
        /// </summary>
        [JsonProperty("can_extend_character_limit")] public bool CanExtendCharacterLimit { get; set; }

        /// <summary>
        /// Required. Whether the user is allowed to extend their character limit.
        /// </summary>
        [JsonProperty("allowed_to_extend_character_limit")] public bool AllowedToExtendCharacterLimit { get; set; }

        /// <summary>
        /// Required. The number of voice slots used by the user.
        /// </summary>
        [JsonProperty("voice_slots_used")] public int VoiceSlotsUsed { get; set; }

        /// <summary>
        /// Required. The number of professional voice slots used by the workspace/user if single seat.
        /// </summary>
        [JsonProperty("professional_voice_slots_used")] public int ProfessionalVoiceSlotsUsed { get; set; }

        /// <summary>
        /// Required. The maximum number of voice slots allowed for the user.
        /// </summary>
        [JsonProperty("voice_limit")] public int VoiceLimit { get; set; }

        /// <summary>
        /// Required. The number of voice add/edits used by the user.
        /// </summary>
        [JsonProperty("voice_add_edit_counter")] public int VoiceAddEditCounter { get; set; }

        /// <summary>
        /// Required. The maximum number of professional voices allowed for the user.
        /// </summary>
        [JsonProperty("professional_voice_limit")] public int ProfessionalVoiceLimit { get; set; }

        /// <summary>
        /// Required. Whether the user can extend their voice limit.
        /// </summary>
        [JsonProperty("can_extend_voice_limit")] public bool CanExtendVoiceLimit { get; set; }

        /// <summary>
        /// Required. Whether the user can use instant voice cloning.
        /// </summary>
        [JsonProperty("can_use_instant_voice_cloning")] public bool CanUseInstantVoiceCloning { get; set; }

        /// <summary>
        /// Required. Whether the user can use professional voice cloning.
        /// </summary>
        [JsonProperty("can_use_professional_voice_cloning")] public bool CanUseProfessionalVoiceCloning { get; set; }

        /// <summary>
        /// Required. The status of the user’s subscription.
        /// </summary>
        [JsonProperty("status")] public SubscriptionStatus Status { get; set; }

        /// <summary>
        /// Required. Whether the user has open invoices.
        /// </summary>
        [JsonProperty("has_open_invoices")] public bool HasOpenInvoices { get; set; }

        /// <summary>
        /// Optional. Maximum number of characters that the character limit can be exceeded by.
        /// Managed by the workspace admin.
        /// </summary>
        [JsonProperty("max_character_limit_extension")] public int? MaxCharacterLimitExtension { get; set; }

        /// <summary>
        /// Optional. The Unix timestamp of the next character count reset.
        /// </summary>
        [JsonProperty("next_character_count_reset_unix")] public UnixTime? NextCharacterCountResetUnix { get; set; }

        /// <summary>
        /// Optional. The maximum number of voice add/edits allowed for the user.
        /// </summary>
        [JsonProperty("max_voice_add_edits")] public int? MaxVoiceAddEdits { get; set; }

        /// <summary>
        /// Optional. The currency of the user’s subscription.
        /// </summary>
        [JsonProperty("currency")] public SubscriptionCurrency? Currency { get; set; }

        /// <summary>
        /// Optional. The billing period of the user’s subscription.
        /// </summary>
        [JsonProperty("billing_period")] public TimePeriod? BillingPeriod { get; set; }

        /// <summary>
        /// Optional. The character refresh period of the user’s subscription.
        /// </summary>
        [JsonProperty("character_refresh_period")] public TimePeriod? CharacterRefreshPeriod { get; set; }

        /// <summary>
        /// Optional. The next invoice for the user.
        /// </summary>
        [JsonProperty("next_invoice")] public NextInvoice NextInvoice { get; set; }
    }

    public class NextInvoice
    {
        /// <summary>
        /// Required. The amount due in cents for the next invoice.
        /// </summary>
        [JsonProperty("amount_due_cents")] public int AmountDueCents { get; set; }

        /// <summary>
        /// Required. The Unix timestamp of the next payment attempt.
        /// </summary>
        [JsonProperty("next_payment_attempt_unix")] public UnixTime NextPaymentAttemptUnix { get; set; }
    }
}