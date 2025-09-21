Here I explain some of the design decisions, possible improvements, and the reasoning behind certain choices made in the codebase.

There are 2 tagged version in GIT
	- Initial version: 0.0.1 - Basic implementation but should fully comply with requirements.
	- New version: 0.0.2 - Added more realistic usecase for Creating/Joining games and querying game state.

BattleshipsController
	- Improvement: Adding method to get all open games would be useful for players to join existing games.
	- CreateGameAsync/JoinGameAsync
		- Since it contains some I/O bound operations, it is marked as async. To make it responsive and not block the thread.
	- Shoot
		- This method is synchronous because it performs quick operations that do not involve I/O-bound tasks, ensuring low latency for user interactions.

ShipsDefinitionService
	- During game creation it always reads the ship definitions from a JSON file.
	- Improvement: Consider caching the ship definitions in memory to avoid repeated file reads, which can improve performance.

Tests
	- Base tests were implemented to cover the main functionalities of the game, including game creation, shooting mechanics, and some edge cases.
	- Improvement: Add more tests to unsure integrity of all logic pieces and robustness.

General
	- There is eternal discussion about vars vs explicit types. I used some vars where it was obvious what type it is, otherwise I used explicit types.
		With codestyle config it can be enforced and I don't mind eitherway.
	- Improvement: Most entities can have ID with much smaller data type. Since we know how many entities can exist in the game, it would be
		more efficient to send smaller data types so responses are smaller.
		