using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// Permission resource grants user, group or the rest of the world access to the PaLM API resource (e.g. <see cref="TunedModel"/> or <see cref="Corpus"/>).
    /// <para>A role is a collection of permitted operations that allows users to perform specific actions on PaLM API resources.
    /// To make them available to users, groups, or service accounts, you assign roles. When you assign a role,
    /// you grant permissions that the role contains.</para>
    /// <para>There are three concentric roles. Each role is a superset of the previous role's permitted operations:</para>
    /// <para>- reader can use the resource (e.g. tuned model, corpus) for inference</para>
    /// <para>- writer has reader's permissions and additionally can edit and share</para>
    /// <para>- owner has writer's permissions and additionally can delete</para>
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// Output only. Identifier. The permission name. A unique name will be generated on create.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// Optional. Immutable. The type of the grantee.
        /// </summary>
        [JsonProperty("granteeType")] public GranteeType GranteeType { get; set; }

        /// <summary>
        /// Optional. Immutable. The email address of the user of group which this permission refers. Field is not set when permission's grantee type is EVERYONE.
        /// </summary>
        [JsonProperty("emailAddress")] public string EmailAddress { get; set; }

        /// <summary>
        /// Required. The role granted by this permission.
        /// </summary>
        [JsonProperty("role")] public ChatRole Role { get; set; }
    }

    /// <summary>
    /// Defines types of the grantee of this permission.
    /// </summary>
    public enum GranteeType
    {
        /// <summary>
        /// The default value. This value is unused.
        /// </summary>
        [ApiEnum("GRANTEE_TYPE_UNSPECIFIED")] Unspecified,

        /// <summary>
        /// Represents a user. When set, you must provide emailAddress for the user.
        /// </summary>
        [ApiEnum("USER")] User,

        /// <summary>
        /// Represents a group. When set, you must provide emailAddress for the group.
        /// </summary>
        [ApiEnum("GROUP")] Group,

        /// <summary>
        /// Represents access to everyone. No extra information is required.
        /// </summary>
        [ApiEnum("EVERYONE")] Everyone,
    }
}