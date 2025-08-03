Dieses Projekt ist eine Umsetzung des Spiels Doom. Um zu gewinnen,
muss man alle fünf Bots, die auf der Karte verteilt patrouillieren,
eliminieren. Wenn die Bots dem Spieler zu viel Schaden zufügen, hat man
verloren.

Ich habe mich bei der Spielmechanik für Aufgabenteil A, also High
Mobility entschieden, sowie Einzelspieler mit animierten Bots.

Die Steuerung lautet wie folgt:

WASD - Bewegen

Shift - Dash

Space - Springen

Linksklick - Feuern

R - Nachladen

1 - Machine Gun ausrüsten (grüner Waffengriff)

2 - Granatwerfer ausrüsten (blauer Waffengriff)

3 - Grappling Hook ausrüsten (roter Waffengriff)

Wie man teilweise der Steuerung entnehmen kann, verfügt der Spieler
über einen Double Jump und einen Dash, der einen für kurze Zeit sehr
schnell nach vorne bewegt. Des Weiteren gibt es auf der Map verteilt
Trampoline (in blau), die einen in die Luft katapultieren und Wände
(in lila), an denen man klettern kann. All diese Spielerbewegungen
werden in der PlayerController.cs umgesetzt.

Die Machine Gun hat 30 Schuss pro Magazin und kann vollautomatisch
abgefeuert werden; diese wird in der GunSystem.cs umgesetzt.
Der Granatwerfer schießt Bomben mit unbegrenzter Muntion ab; dieser wird
in der ObstacleGun.cs umgesetzt.
Mit der Grappling Hook kann man sich an jedes beliebige Objekt in einer
begrenzten Distanz heranziehen; diese wird in der Grappling.cs
umgesetzt.

Die Bots werden in EnemyAi.cs umsetzt und besitzen grundsätzlich
drei States: Patrouillieren, Verfolgen, Angreifen. Die Bots verfügen
über zwei verschiedene Angriffsmethoden, die zufällig ausgeführt werden:
Schießen und mit Bomben werfen. Des Weiteren werden im dazugehörigen
Animator-Controller sechs verschiedene Animationen bei bestimmten
Bedingungen ausgeführt: Idle, laufen, schießen, werfen, getroffen werden
und sterben. Alles rund um die Animationen befindet sich im Animations-
Ordner.

Verwendete externe Assets:
- "Starter Assets - ThirdPerson" für das Modell der Bots
- Sechs verschiedene Animationen von Mixamo

Bekannte Bugs/Fehler:
- Der Shooting Point der Machine Gun wird manchmal nicht korrekt
erkannt/berechnet, was dazu führt, dass die Schüsse auf die Bots nicht
registriert werden. Dies hat mich leider auch daran gehindert, richtige
Schussanimationen einzubauen
- Das NavMeshSurface ist mir an manchen Treppen nicht ganz so gut
gelungen, wodurch Bots manchmal feststecken
- Wie auch schon bei Arkanoid gab es Probleme einen Restart-Button am
Ende des Spiels einzubauen, da beim Neustart einfach das Game gefreezed
bleibt

Video:

![Video](video/Video.mp4)
