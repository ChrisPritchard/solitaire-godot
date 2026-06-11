
using System.Collections.Generic;

public static class SpriteRegions
{
    private const int CardWidth = 36;
    private const int CardHeight = 54;

    private static Rect2I Rel(int dx, int dy) => new Rect2I(dx, dy, CardWidth, CardHeight);

    public static readonly Dictionary<int, Dictionary<int, Rect2I>> CardIndexes =
        new Dictionary<int, Dictionary<int, Rect2I>>
        {
            [0] = new Dictionary<int, Rect2I> // hearts
            {
                [2] = Rel(22, 6),   [3] = Rel(62, 6),   [4] = Rel(102, 6),  [5] = Rel(142, 6),
                [6] = Rel(182, 6),  [7] = Rel(62, 64),  [8] = Rel(102, 64), [9] = Rel(142, 64),
                [10] = Rel(182, 64),[11] = Rel(62, 122),[12] = Rel(102, 122),[13] = Rel(142, 122),
                [1] = Rel(182, 122)
            },
            [1] = new Dictionary<int, Rect2I> // spades
            {
                [2] = Rel(226, 300),[3] = Rel(266, 300),[4] = Rel(306, 300),[5] = Rel(346, 300),
                [6] = Rel(386, 300),[7] = Rel(226, 184),[8] = Rel(266, 184),[9] = Rel(306, 184),
                [10] = Rel(346, 184),[11] = Rel(226, 242),[12] = Rel(266, 242),[13] = Rel(306, 242),
                [1] = Rel(346, 242)
            },
            [2] = new Dictionary<int, Rect2I> // diamonds
            {
                [2] = Rel(22, 300), [3] = Rel(62, 300), [4] = Rel(102, 300),[5] = Rel(142, 300),
                [6] = Rel(182, 300),[7] = Rel(62, 184), [8] = Rel(102, 184),[9] = Rel(142, 184),
                [10] = Rel(182, 184),[11] = Rel(62, 242),[12] = Rel(102, 242),[13] = Rel(142, 242),
                [1] = Rel(182, 242)
            },
            [3] = new Dictionary<int, Rect2I> // clubs
            {
                [2] = Rel(226, 6),  [3] = Rel(266, 6),  [4] = Rel(306, 6),  [5] = Rel(346, 6),
                [6] = Rel(386, 6),  [7] = Rel(226, 64), [8] = Rel(266, 64), [9] = Rel(306, 64),
                [10] = Rel(346, 64),[11] = Rel(226, 122),[12] = Rel(266, 122),[13] = Rel(306, 122),
                [1] = Rel(346, 122)
            }
        };

    public static readonly Rect2I CardBack = Rel(12, 242);
    public static readonly Rect2I CardSpace = Rel(12, 183);
}