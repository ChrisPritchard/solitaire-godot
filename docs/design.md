# Design

lets get my thinking muscles going. terminology reminder:

- the 'tableau' which is made up of seven piles, all face up, each with one more card than the previous
- the 'stock', consisting of all remaining cards from which three cards can be drawn at a time into...
- the 'waste', where a new horizontal pile is made from three-card-at-a-time draws from the stock
- four 'foundations', non-staggered stacks built from Aces up (Ace, 2, 3 etc through to King)

we need cards, that can be:

- dragged
- stacked
    - on top, so maybe with a slight offset to show under cards or perhaps a special sprite to show its a pack?
    - on top offset, for building a sequence, showing the suit of the card underneath
    - on top flat, no slight offset, for the left final stacks
    - on top right offset, for the waste, showing the left suit

a card can only be dropped in 'valid' locations. this would be areas on the board that are detected, and then an evaluation concerned.

mechanics:

- card interaction detection, top most area only
- dragging - single card or, if cards are stacked on that card and the drag is 'valid', the card and those beneath it
- dropping - detect top most area underneath cursor, then valid check

step one:

card object, which exposes a settable z-order property
getcardsatpoint function, using a point query to find all areas, getting their parent cards, and ordering by zorder
offset settings, for placement
detecting a droppable empty area - getcardsatpoint could return an area if no cards detected, or null if no area?

we could do this with some tuples/union types, perhaps:

- topmost card or, space or, nothing
- and spaces could be: tableu space, stock space, foundation space

some game rules can be embedded in the above. e.g. if the card is under other cards then its not a drop point, and if those cards are not in sequence then its not draggable either. though perhaps this logic should be in the event handler as opposed to the selector.

code for selecting areas under a spot:

```c#
var query = new PhysicsPointQueryParameters2D
{
    Position = globalPos,
    CollisionMask = 1,
    CollideWithAreas = true,
    CollideWithBodies = false
};
var results = spaceState.IntersectPoint(query);
foreach (var result in results)
{
    if (result["collider"].AsGodotObject() is Area2D area)
```