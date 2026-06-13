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

## decision logic

I feel most of the game logic can be expressed in a complicated set of conditionals 🤔:

input events:
    if dragging:
        if mouse up:
            if can place:
                place, stop dragging (multi cards can only go on tableaus, mind)
            else
                return to origin
        else if move:
            move dragged cards
    else:
        if mouse down:
            if stock:
                expand stock
            else if can pick:
                pick, start dragging

with pick logic being:
    is this waste:
        is card on top:
            then pick up single
    else is this tableau:
        is card on top:
            then pick up single
        else is card under sequence:
            then pick up multi

and drop logic being:
    is single drag:
        is this foundation, waste spare or tableau:
            is this top position:
                is card in sequence:
                    then yes
    else is this multi:
        is this tableau:
            is this top position:
                is base card in sequence:
                    then yes

sequence is slightly different:
    is this foundation: 
        is card +1 same suit:
            yes
    else
        is card -1, opposing suit:
            yes

I think thats it? how and where should dragging be implemented? perhaps as a inputevent handler on the main controller. it will need to track the cards its dragging, and for each it needs their 'drag start'

## objects and methods

for objects we need:

- root spaces, which can be foundations, stock, tableau roots (and also the first place in the waste). each needs to know what it is so it can report this on update
- cards, which need to know where they are, and what is above or below them. when a card is placed on a card, it needs to update its parent with itself. when it is removed, its parent needs to be told

card:
    value (suit and number)
    parent (card?)
    child (card?)
    location: stock, waste, tableau, foundation

does it need to know which tableau or foundation it is in? probably not, since its behaviour can be derived just from its location type

card:
    can accept
        false if stock or waste, else needs no child, needs source to be single if foundation, and needs value match based on location
    can be dragged
        false if foundation or stock or has child in waste, else check child can drag in tableau (recursive)

so we have cards, spaces and the stock. for the stock, this is something of a unique object - cards within are masked, so its more that it tracks values rather than actual cards. 
- it might need to indicate its size, optionally
- when exhausted, it needs to become/be replaced with a space.

## dragging and offsets

we could either track an array of dragged cards (sometimes of length one)
or we could just track the single dragged card, and if that card has children, we'd recalculate their positions as needed
dragged cards of more than one item can only come from the tableau, which means the offset for each child is fixed.
we would need to track both the start position of dragging and the mouse offset from that start. this is so that if returning to original position we can gather that. on placement, this is reset to its current position (or null)

possibly we could have a drag state.

## simplification, next steps

DONE stacktarget as a shared version of card and space. card, space and stock are the three 'things' in the world, though stock can't be a parent or dragged, and is just used to trigger to waste actions
- some issues: only cards can be placed, which causes a circular reference if they share the base type
- could create a gameobject class, instead of stack target, which also allows for canbedragged
    - spaces and the stock would return no for this
    - stock could also use this, though it has no need for the location property
    - possibly location is not defined on game object

FIXED should show zorders on cards - and sort font display - to debug z order issues

DONE could move dragging state into its own section, to simplify reset and apply operations

need to add sounds

instant card appearance can be replaced with twining movement (disabling interaction while this happens)

a hint option, showing what can be moved, possibly with some effect on cards

game over, reset options, win tally