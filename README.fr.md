# VR Chess

[English version](README.md)

Un jeu d'échecs en réalité virtuelle. On attrape les pièces avec les manettes, on les pose sur l'échiquier, et on joue contre un ami qui a son propre casque. Le projet est fait par le club, pour les nouveaux membres. Le jeu compte, mais l'objectif, c'est surtout d'apprendre en le construisant.

## Pourquoi ce projet

Tout le monde connaît les règles des échecs, donc on peut se concentrer sur le code. En chemin, on croise la programmation orientée objet (l'héritage pour les pièces, par exemple : une classe de base, puis le pion, la tour, le cavalier...), le principe de responsabilité unique, le fonctionnement de C# dans Unity, les tests et les bases du réseau.

On ne cherche pas à tout optimiser. Un moteur plus rapide mais plus dur à expliquer ne nous aide pas ici. Entre un code malin et un code qu'on comprend en le lisant, on choisit le second.

## Ce que fait le jeu

Toutes les règles y sont : roque, prise en passant, promotion, échec et mat, pat. En VR, on saisit une pièce, les cases où elle peut aller s'allument, et on la lâche où on veut. Si le coup n'est pas valide, la pièce retourne à sa place.

Pour jouer à deux, l'un crée une room et l'autre la rejoint. Au début, tout se passe sur le même réseau local. Chacun a son casque, personne n'a besoin de passer le sien à l'autre.

## Comment c'est construit

Le code est découpé en couches qui ne se mélangent pas. L'idée de base tient en une phrase : la VR propose un coup, le contrôleur de jeu décide, la vue affiche le résultat.

L'Engine contient les règles des échecs, en C# pur. Il ne connaît pas Unity, donc on peut le tester sans casque. Le Game regroupe le contrôleur de partie, qui vérifie un coup, l'applique et prévient tout le monde. Lui non plus ne connaît pas Unity. Côté Unity, la View s'occupe de ce qu'on voit (plateau, pièces, surbrillances, messages), VRInput transforme les gestes VR en coup à jouer, et Multiplayer gère la room et l'envoi des coups.

Sur le réseau, seuls les coups circulent. Chaque joueur garde ses pièces chez lui, et l'hôte reste l'arbitre : il valide chaque coup avant que l'autre le voie.

```
Assets/
└── _Project/
    └── Scripts/
        ├── Engine/        # règles des échecs, C# pur
        ├── Game/          # contrôleur de partie, joueurs, résultats, C# pur
        ├── View/          # plateau, pièces, surbrillances, messages
        ├── VRInput/       # gestes VR transformés en coups
        └── Multiplayer/   # room et code réseau
```

Chaque dossier a son propre namespace (`Chess.Engine`, `Chess.Game`, etc.). La règle à retenir : rien dans `Engine` ni dans `Game` ne doit faire référence à `UnityEngine`.

## Technologies

Unity (2022 LTS ou plus récent) avec le pipeline URP. OpenXR et le XR Interaction Toolkit pour la VR, Netcode for GameObjects pour le multijoueur, du C# partout, et le Unity Test Framework pour les tests.

## Démarrer

Il vous faut Unity Hub, Git, et la version d'Unity indiquée dans `ProjectSettings/ProjectVersion.txt`. Un casque compatible OpenXR est pratique mais pas obligatoire : un simulateur XR permet de tester sans. Pour essayer le multijoueur, deux appareils sur le même réseau local suffisent, ou deux instances d'Unity sur la même machine.

```bash
git clone <URL-DU-DEPOT>
```

Ouvrez le projet avec Unity Hub et laissez Unity importer les packages. Ouvrez ensuite la scène `GameTable` et lancez le mode Play. Les instructions propres à votre casque restent à ajouter ici.

## Idées pour plus tard

Rien de tout ça n'est prévu au départ. On y vient s'il reste du temps et si le reste tient la route. Comme la logique est séparée de l'affichage, la plupart de ces idées ne touchent que la View.

Un jeu d'échecs grandeur nature, avec des pièces géantes : on marche autour d'un échiquier à taille humaine. Il faudra repenser l'échelle, les déplacements et la façon de saisir une pièce.

Les XR Hands : jouer avec ses mains plutôt qu'avec les manettes, en pinçant les doigts pour prendre une pièce. Ça dépend du casque.

La réalité mixte : on pose un échiquier sur une vraie table et le casque simule les pièces par-dessus. Là aussi, tout dépend de ce que le casque sait faire.

Voir l'adversaire jouer en direct : sa main attrape une pièce, et elle se détache de sa case. Ça passe par un avatar réseau (tête et mains), pas par les pièces elles-mêmes. S'il la relâche sur un coup invalide, la pièce retourne à sa place.

Un adversaire IA, pour jouer seul. Plus tard encore, le jeu en ligne hors réseau local, avec un service de mise en relation comme Unity Relay et Lobby. Si la room est bien isolée derrière une interface, seule la manière de la créer et de la retrouver change.

Et les petits plus : chronomètres, pièces capturées, historique des coups, quelques environnements différents, revanche, abandon, sons et vibrations des manettes.

## Contribuer

Tout le monde peut contribuer, quel que soit son niveau. Quelques habitudes à garder : respecter les couches, une classe pour une responsabilité, des commentaires qui expliquent le pourquoi plutôt que le quoi, et un test pour chaque règle d'échecs ajoutée au moteur. On travaille sur une branche, et un autre membre relit la pull request avant la fusion.

## Licence

À définir.
