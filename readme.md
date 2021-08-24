

<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->

<!-- [![Stargazers][stars-shield]][stars-url] -->
<!-- [![Contributors][contributors-shield]][contributors-url] -->
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]


<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/LeeTeng2001/Pac-Pin-Pong">
    <img src="./readme source/game icon.png" alt="Main Logo" width="80" height="80">
  </a>

  <h3 align="center">Pac-Pin-Pong</h3>

  <p align="center">
    A fun local multiplayer game comprising Pacman, Pinball and Space Invader into one 
    <br />
    <a href="https://github.com/LeeTeng2001/Pac-Pin-Pong#about-the-project"><strong>Learn about the game »</strong></a>
    <br />
    <br />
    <a href="https://github.com/LeeTeng2001/Pac-Pin-Pong#gameplay-demo">View Demo</a>
    ·
    <a href="https://github.com/LeeTeng2001/Pac-Pin-Pong/issues">Report Bug</a>
    ·
    <a href="https://github.com/LeeTeng2001/Pac-Pin-Pong/issues">Request Feature</a>
  </p>
</p>



<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary><h2 style="display: inline-block">Table of Contents</h2></summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with-%EF%B8%8F-using">Built With</a></li>
      </ul>
    </li>
    <li><a href="#gameplay-demo">Gameplay Demo</a></li>
    <li>
      <a href="#getting-started-building--exploring">Getting Started (Building & Exploring)</a>
      <ul>
        <li><a href="#building">Building</a></li>
        <li><a href="#exploring">Exploring</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started-playing">Getting Started (Playing)</a>
      <ul>
        <li><a href="#download-instruction-for-macos">Download for MacOS</a></li>
        <li><a href="#download-instruction-for-window">Download for Window</a></li>
        <li><a href="#play-on-web">Play On Web</a></li>
        <li><a href="#game-explained">Game Explained</a></li>
        <li><a href="#controls">Controls</a></li>
      </ul>
    </li>
    <li><a href="#other-ideas-for-the-game">Other ideas for the game</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgements--credits">Acknowledgements & Credits</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

![](readme%20source/game%20title.png)


Recently I've been dabbling in Godot for quite a bit, it's a very capable engine for 2D and I really like static language that's why I've chosen C# as my project language, there aren't a lot of quality C# Godot demo out there that's why I've inclined to make one to contribute back to Godot.

My main inspiration came from [this video by Alpha Beta Gamer](https://www.youtube.com/watch?v=8TOGBZp8_Ao&list=WL&index=9&t=4s), I think it's very clever to combine multiple childhood retro games into one that's why I wanted to challenge myself to recreate the same game.

**You can do whatever you want with this project.**
 Most of the assets and audio is created by me, when it's not, it's under cc0 license (like [kenney explosion fx](https://kenney.nl/) and some sound effect from freesound.org)




### Built With ❤️ Using

* [Godot Mono](https://godotengine.org/)
* [Affinity Designer](https://affinity.serif.com/en-us/designer/)
* [Ableton](https://www.ableton.com/en/)

_**Disclamer**: I'm not affiliated with any of the companies I just think that they're great and they provide really valuable tools for me that's why I wanted to give them credit_



## Gameplay Demo

<p align="center">
  <img src="https://github.com/LeeTeng2001/Pac-Pin-Pong/blob/main/readme%20source/gameplay%20demo%201.gif" />
</p>


<!-- GETTING STARTED -->
## Getting Started (Building & Exploring)

### Building

Just clone this repo and use [Godot Mono](https://godotengine.org/) to open the project folder "3 retro game in 1" and you'll be good to go. Check out Godot website if you don't how to setup mono environment
   ```sh
   git clone https://github.com/LeeTeng2001/Pac-Pin-Pong.git
   ```

### Exploring
'3 retro game in 1/Scenes/World.tscn' is the main entrance for the game, you can see examples like how player stats are stored, shader effects, ghost effects, power up and more!

Happy exploring!

## Getting Started (Playing)
### Download Instruction for MacOS
Because of the codesign issue on Mac, I recommend you to download the whole project and build it locally, otherwise if you know your way around you could download the latest release and code sign in order to play the game.

1. Visit [release page](https://github.com/LeeTeng2001/Pac-Pin-Pong/releases) and downlaod the latest release (for example **Pac-Pin-Pong-v1.0.0-macos.dmg**)
2. Open the dmg file and drag the **Pac-Pin-Pong** to your machine, preferably to **/Applications** folder
3. Open the Terminal app and insert the following command, you can drag the application to the terminal to auto fill the **\<path-to-the-application\>**
   ```bash
   xattr -rd com.apple.quarantine <path-to-the-application>
   sudo codesign --force --deep --sign - <path-to-the-application>
   ```
4. Double click the application to run

### Download Instruction for Window
1. Visit [release page](https://github.com/LeeTeng2001/Pac-Pin-Pong/releases) and downlaod the latest release (for example **Pac-Pin-Pong-v1.0.0-window.zip**)
2. Extract the zip file, double click the exe file to play

### Play on Web
1. Visit [itch io](https://lunafreya2001.itch.io/pac-pin-pong) and hit play


<!-- USAGE EXAMPLES -->
### Game Explained

I will refer left hand side player as player 1 and vice versa

Each player have two shoot buttons, one of them shoots the Space Invader bullet, the other shoots the Pacman towards the grid. In addition, there's also two sets of movement keys for each player, one of them controls the Pacman during his journey in the grid and the other controls the pinball pad movement.

The goal is to survive longer than the other player, every time you failed to catch the Pacman, run out of action time, collide with ghost without a buff, collide with bullets and etc. will deplete your health. You can also collect the points inside the Pacman grid to restore your health and bullets, successfully catching Pacman from the other player also restores health and bullets. At the same time, the Pacman points will make your Pacman moves faster which means it'll become really hard to control as time progresses.

Every time the game difficulty increases, the music will become more intense, as indicated by the VFX along with a Pacman map refresh. There's also powerup inside the grid that will either kill a random ghost, make them invisible, make you strong enough to eat them or a random effect.

It's best to experience it yourself! I have a blast playing this game with my friend.

### Controls
<dl>
  <dt>Player 1</dt>
  <dd><b>W, A, S, D</b> to move the pinball pad</dd>
  <dd><b>C, F, V, B</b> to move the Pacman</dd>
  <dd><b>E</b> to shoot pinball bullet</dd>
  <dd><b>Space</b> to shoot Pacman</dd>

  <dt>Player 2</dt>
  <dd><b>Up, Down, Left, Right</b> to move the pinball pad</dd>
  <dd><b>K, M, ,, .</b> to move the Pacman</dd>
  <dd><b>'</b> to shoot pinball bullet</dd>
  <dd><b>Enter</b> to shoot Pacman</dd>
</dl>

_If you find the control awkward you could always download the source file to change the controls internally, maybe I'll add a control panel in the upcoming release_

## Other ideas for the game

* Camera shaking effect ✅
* Configure current difficulty ✅
* More sound effects ✅
* Setting menu for adjusting game volume
* Main menu for picking difficulty
* AI opponent


## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are welcomed.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.


<!-- CONTACT -->
## Contact

Email: leeteng2001@sjtu.edu.cn

Project Link: [https://github.com/LeeTeng2001/Pac-Pin-Pong](https://github.com/LeeTeng2001/Pac-Pin-Pong)



<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements & Credits

* [Alpha Beta Gamer for the game idea](https://www.youtube.com/watch?v=8TOGBZp8_Ao&list=WL&index=9&t=4s)
* [Kenney for explosion sprites](https://kenney.nl/)
* [xyezawr for explosion sprites](https://xyezawr.itch.io/free-pixel-effects-pack-4-explosions)
* [V-ktor for explosion sound](https://freesound.org/people/V-ktor/sounds/435416/)
* [MATRIXXX_ for retro dead fx](https://freesound.org/people/MATRIXXX_/sounds/495541/)
* [othneildrew for this amazing readme template](https://github.com/othneildrew/Best-README-Template)




<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/LeeTeng2001/Pac-Pin-Pong.svg?style=for-the-badge
[contributors-url]: https://github.com/LeeTeng2001/Pac-Pin-Pong/graphs/contributors
[stars-shield]: https://img.shields.io/github/stars/LeeTeng2001/Pac-Pin-Pong.svg?style=for-the-badge
[stars-url]: https://github.com/LeeTeng2001/Pac-Pin-Pong/stargazers
[issues-shield]: https://img.shields.io/github/issues/LeeTeng2001/Pac-Pin-Pong.svg?style=for-the-badge
[issues-url]: https://github.com/LeeTeng2001/Pac-Pin-Pong/issues
[license-shield]: https://img.shields.io/github/license/LeeTeng2001/Pac-Pin-Pong.svg?style=for-the-badge
[license-url]: https://github.com/LeeTeng2001/Pac-Pin-Pong/blob/main/LICENSE
