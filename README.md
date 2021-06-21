# Farm

This is one of my first attempts at making a game. 

As most people do the first time I set my sights too high. 
I tried to make far too many mechanics for my first game. 

However it does show allot of my problem solving skills and it shows 
a good start point which is a good contrast to what i make now.

How To Play:
The controls are hard coded in this and i had yet to make a system to change 
the controls like i have currently. 

Movement: W,A,S,D
Inventory: I (move things to the top bar to be on the hot bar)
HotBarSelection: 1,2,3,4,5,6,7,8,9
UseSelectedItem: Space
Interact: E(This starts conversation with NPCs haven't done much more than a hello in this)
Harvest: H
DebugMenu: LeftCTRL,LeftALT,BackSlash (This menu is important to speed things along)


What is programmed:
-Plants:
The plants are prototyped items. its simple and easy to make new plants with different growth
stages and different item rewards for fully growing and harvesting. In my more current games
I have made a better interface for the people who might be adding these items so they don't 
need to code anything just simple drop in imgs and fill in the info fields.

The plants will grow as long as they are watered (if they need watering this is a changeable option)
and when fully grown can be harvested to give items such as corn to the player.

-Relationship:
I started adding a relationship mechanic with the NPCs where the player would be rewarded
with cheaper items or perhaps help on the farm. I haven't made any visuals for this so far
but by interacting with the NPCs regularly (daily) the relationship will improve.

-Farming: with the hoe you can plough a field and plant seeds with the seed bag. Then
you can use the watering can to water them and they will grow. I programmed the basic
plant to need watering daily. Days can be skipped in the debug menu.

-Building: this isn't completely implemented but you can put together a simple building
with the current tools and i made the installed objects use a prototype method like the plants 
so new items could be added easily. Again this is a feature I have greatly improved on.


