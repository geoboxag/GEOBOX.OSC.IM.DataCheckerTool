# GEOBOX - DataChecker-Tool
Tool für das Reorganisieren der Datenprüfungen vom Autodesk AutoCAD Map 3D.

## Beschrieb
Autodesk AutoCAD Map 3D bietet zurzeit keine Möglichkeit, Datenprüfungen (DataChecks) zu organisieren und sortieren an.
Mit diesem Tool können die Datenprüfungen unabhängig organisert und sortiert werden.

## Vorgehen
1. Datenprüfungen mit dem Autodesk Infrastructure Administrator exportieren.
2. Die exportierte Datei mit dem Tool öffnen.
3. Datenprüfungen nach Wunsch organisieren und sortieren.
4. Datei speichern und Tool schliessen.
5. Die bestehenden Dantenprüfungen mit dem Autodesk Infrastructure Administrator löschen.
6. Die gespeicherte Datei mit dem Autodesk Infrastructure Administrator importieren.

## Voraussetzungen und Installation
### Voraussetzung
- Microsoft .NET Framework 4.7

### Installation
- Es benötigt keine Installation.
- Die Applikation kann aus einem beliebigen Verzeichnis aus gestartet werden.

### Hinweis
- Das Tool setzt keine Installation vom Autodesk Infrastructure Administrator oder dem Autodesk AutoCAD Map 3D auf der Arbeitsstation voraus.

## Funktionen
### Übersicht
![](https://github.com/geoboxag/GEOBOX.OSC.IM.DataCheckerTool/raw/master/_images/overview.png)

### Funktionen
| Nr. | Icon     | Beschrieb |
|-----|----------|-----------|
| 1   | ![][i1]  | Öffnen einer Datei mit den Datenprüfungen.           |
| 2a  | ![][i2a] | Sämtliche Themen und Prüfungen absteigend sortieren. |
| 2b  | ![][i2b] | Sämtliche Themen und Prüfungen aufsteigend sortieren.|
| 3a  | ![][i3a] | Enthaltene Themen und Prüfungen absteigend sortieren.|
| 3b  | ![][i3b] | Enthaltene Themen und Prüfungen aufsteigen sortieren.|
| 4   |          | Selektierter Eintrag wird in den Details angezeigt.  |
| 5   |          | Per "Ziehen und Loslassen" (Drag and Drop) können Themen und Prüfungen beliebig umsortiert werden. |
| 6   |          | Zeigt Meldungen vom System an.                       |
| 7   | ![][i7]  | Speichert die Datei mit den Datenprüfungen.          |


[i1]:  https://github.com/geoboxag/GEOBOX.OSC.IM.DataCheckerTool/raw/master/_images/gbGenOpen24.png "Open"
[i7]:  https://github.com/geoboxag/GEOBOX.OSC.IM.DataCheckerTool/raw/master/_images/gbGenSave24.png "Save"

[i2a]:  https://github.com/geoboxag/GEOBOX.OSC.IM.DataCheckerTool/raw/master/_images/gbSortAllAsc16.png "Order Icon"
[i2b]:  https://github.com/geoboxag/GEOBOX.OSC.IM.DataCheckerTool/raw/master/_images/gbSortAllDesc16.png "Order Icon"
[i3a]:  https://github.com/geoboxag/GEOBOX.OSC.IM.DataCheckerTool/raw/master/_images/gbSortAsc16.png "Order Icon"
[i3b]:  https://github.com/geoboxag/GEOBOX.OSC.IM.DataCheckerTool/raw/master/_images/gbSortDesc16.png "Order Icon"
