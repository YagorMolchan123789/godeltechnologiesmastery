namespace GTE.Mastery.Documents.Api.Attributes
{
    /// <summary>
    /// A marker attribute indicating that the marked construct (class, method, etc.) SHOULD NOT be modified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface |
                    AttributeTargets.Method)]
    public sealed class DoNotModifyAttribute : Attribute
    {
    }
}
