using System;
using System.Reflection.Metadata;

namespace MyNewApp.Dtos;

public class GameStore
{

    // public: can be called outside of class
    // static: one per type
    // readonly: can be construct once, but can't be new again
    // Dictionary: search items by Ids (fast)
    public static readonly Dictionary<int, GameSummaryDto> GameListing = [];

    // Noting used ids
    private static int NextId = 1;

    // Interlocked: Provide safer POST request, when handling multiple. 
    public static int CheckInsertId() => Interlocked.Increment(ref NextId);
}
