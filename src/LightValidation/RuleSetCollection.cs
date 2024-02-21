using LightValidation.Constants;
using LightValidation.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LightValidation;

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView<string>))]
public sealed class RuleSetCollection : IReadOnlyCollection<string>
{
    private static readonly string[] DefaultRuleSets = [RuleSet.Default];
    public static readonly RuleSetCollection Default = new();

    private readonly string[] _ruleSets;

    private RuleSetCollection()
    {
        _ruleSets = DefaultRuleSets;
    }

    public RuleSetCollection(IEnumerable<string> ruleSets)
    {
        ArgumentNullException.ThrowIfNull(ruleSets);

        _ruleSets = ruleSets.Distinct().ToArray();
    }

    public int Count => _ruleSets.Length;

    public bool Contains(string ruleSet)
    {
        return Array.Exists(_ruleSets, x => x == RuleSet.Any || x == ruleSet);
    }

    public IEnumerator<string> GetEnumerator()
    {
        return ((IEnumerable<string>)_ruleSets).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _ruleSets.GetEnumerator();
    }
}
