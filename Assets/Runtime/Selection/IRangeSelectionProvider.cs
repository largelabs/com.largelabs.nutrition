
using System.Collections.Generic;
using UnityEngine;

public interface IRangeSelectionProvider
{
    int MaxSelectionRadius { get; }
    Vector2Int? CurrentOriginCell { get; }
    IReadOnlyList<Vector2Int> SelectedRange { get; }
    IReadOnlyList<HashSet<Vector2Int>> SelectedRangeInSteps { get; }
}