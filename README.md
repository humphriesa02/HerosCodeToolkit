# HerosCode.Toolkit

If this ever has "Set up for AI Context" in it - shoot me.

## What this is:
A "simple" toolkit to help Hero/HerosCode (hey that's me) make games easier.

Also works as a fun experiment to see how much game logic I can make generic, a skill I think is rather applicable.

## AI Disclaimer
Now I know what you're thinking. As of writing (9/30/2026) I can admit AI is enescapable. I'm not trying to fight that.
I simply want to *understand* and be the *owner* of what I write in this codebase.

Does that mean research could be assisted via AI? Of course. Could AI be physically writing some rather difficult logic, and I transcribe said
logic into the code myself? Perhaps. But that's where it stays. No "entire chunks of code written by AI". No vibe coding. This is my package.

## Current Status:
* Actor - Base entity for all objects in my games. Serializeable, moveable, state-driven, etc.
* Room - Holds actors as of now. A single room can serialize/deserialize all actors within itself
* Saving - currently support for JSON serializing and deserializing

## Future Plans:
* State machine
* Movement systems
* Menu systems
* Minimaps
* Camera systems
* Inventory