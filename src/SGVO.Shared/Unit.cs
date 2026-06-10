namespace SGVO.Shared;

/// <summary>
/// Represents a void type for commands that don't return a value.
/// Used as TResponse in ICommand&lt;Unit&gt; to avoid Result&lt;Result&gt; nesting.
/// </summary>
public readonly record struct Unit
{
    public static readonly Unit Value = default;
}
