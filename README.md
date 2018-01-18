# ball-game-3d (working title)

[character-concept]: https://i.imgur.com/pEul0Om.png "Character concept"

![alt-text][character-concept]
A side-view fighting game where brains beat brawn. With a ball.

## Concept

Players fight each other on a floating stage and attempt to push each other off the stage, like in smash. Players are given a set amount of stocks and whoever runs out of stocks first loses the game.
The game mechanic given to players is telekinetically pushing a ball towards the other player. Both players control the same ball.

The ball is suspended between the players in a position of rest when neither player attempts to move it. 

# Design

## Feature 1: Telekinesis, functional design
The ball attempts to return to a position exactly in the middle of the two players positions. Both players can simultaneously influence the ball. Players can time their telekinetics to get the ball into an ocillating state, to attempt to outmanouver the opponent.

### F1: Telekinesis, technical design
### F1A: - attempting to return to rest
The ball will attempt to return to a position  between the two players when neither of them are pushing it. This is done with a [PID controller](https://en.wikipedia.org/wiki/PID_controller). Using the same jargon as the article behind the link: the setpoint is the midpoint between the two players positions, and the process variable is the balls position.

# !!!Warning, Maths ahead!!!

You can skip this segment if you want. But for those who are interested:

A PID controller is basically just a maths function with some state. It gives a response to a situation based on the error (difference) between the setpoint (target value) and the process variable (current value).

This response is a sum of three components: 
 - a Proportional component `proportional_gain * error(t)` 
 - an Integral component    `integral_gain * integral from 0 to t of the error`
 - a Derivative component   `derivative_gain *D(t)`

It is an engineering concept used in many industrial applications to achieve a steady transition from one state to another in a dynamic system. E.g. it is used to make servos move smoothly.

### Our PID as used in the game
```
t = a specific point in time
T = a function parameter
error(t) = target(t) - ball_position(t)
integral(t) = the integral of error(T) from T=0 to T=t

response(t) = proportional_gain *error + integral_gain *integral + derivative_gain * D(t)
```
the target is the point between the players, and the *_gains are constants that can be fiddled with to change the behaviour of the ball.


### F1B: influencing the ball
This can be achieved in at least two ways:
 - Moving the target from feature 1 along a line between the players. By defining a ratio of the players influence on the ball, and lerping the target with this ratio. This is the way the current implementation works.
  - Letting each players influence create a force on the ball, this would be easy to test out, and could lead to more controlled gameplay.
  
## F2:Players influence, functional design
The players can influence the ball by pressing a button. This will set their influence to the max value. However, players only have a certain amount of concentration available, which will deplete while excercising influence on the ball. It will replete when the players stop. 

## F3: Other player input
The players can move left and right, and jump. Jumping will slightly decrease a players influence on the ball. A player can use this to initiate a spring-like effect by jumping, then pushing the ball towards the enemy.

## F4: Spawning
Once the players have fallen out of play, they will spawn back onto the stage if they have stock left.
At spawntime, the players will be returned to starting positions, and the ball will reset.
If the ball for some reason gets stuck on some strange geometry or is too far out of play and osciallating wildly, it will also respawn after a whil

## F5: HUD and visual feedback
The HUD will display the players stocks, their influence, and their concentration. In the flash version this was done with a brain representing a meter, and the influence and concentration bars moving up and down the brain like liquids on top of each other. This worked since influence can never be higher than concentration. 

### Visual feedback on the ball:

the two players are colored red and blue (or some other colrs), and the color of the ball changes depending on how much influence each player has on it.

i.e.
 - red has 100 influence, blue has 0, the color of the ball is red.
 - red has 0 influence, blue has 100, the color of the ball is blue.
 - red has 100 influence, blue has 100 influence, the color of the ball is magenta.

The ball also grows when more influence is being exerted upon it.

## F6: Start Menu
There will be a start menu, where a user can start a stock game, exit the game, change controls, and look at the credits.

## F7: Pause Menu
There will be a pause menu, where a user can unpause the game, or exit the match.

## F8: Win screen
A win screen declaring the victor of a match, and some stats.

# Nice to have features/unfinished ideas

Once the core features above are in the game, and graphics and gamefeel have been polished, there are some things I would like to try. If we have time they could be fun to implement.

#### 3-player games
This one is highly speculative, but could be fun to try.

#### 3D version of game
Would only work with two players: A rotating camera, like in Soul calibur, and 3D physics. The telekinesis logic wouldn't really change.

#### Health system
Could be fun to try out a health system. Players would take damage proportional to the velocity of the ball.

#### Powerups
These mostly make sense with the health systems but there are some which could work without, such as a shield, an immaterialiser, and a mass increase.
