# Coding Guidelines Frontend

## Benennung
- Dateien werden nach folgendem Schema benannt: "feature.type.ts." (also z.B. "roles.component.ts"). Der Featurename kann mit einem "-" separiert werden (z.B. "update-role.component.ts")
- Für Klassennamen wird Camel-Case verwendet. Der Klassenname enstpricht dem Datennamen inklusive Type. Beispiel: "update-role.component.ts" entspricht "UpdateRoleComponent"

## Ordnerstruktur
- In jeder Datei soll nur eine Komponente/Service etc. definiert werden, idealerweise soll eine Datei 400 Zeilen nicht überschreiten
- Im app/shared werden geteilte Komponenten abgelegt, die mehrfach wiederverwendet werden. Ansonsten sind die Komponenten nach Features abgelegt (z.B. app/contacts für Kontakte).
- Neue Komponenten, Services, etc. sollen über "ng generate" erstellt werden. Damit ist direkt die korrekte Struktur sichergestellt.

## Best Practices
- Präsentationslogik gehört in die Komponentenklasse, nicht in das Template (z.B. die Berechnung eines Durchschnitts, das Ergebnis sollte von der Komponente berechnet und bereitgestellt werden)
- Services sind Singletons und sollen für das Teilen von Daten und Funktionalität verwendet werden 
- Definierte Funktionen sollen handhabbar sein und idealerweise nicht mehr als 75 Zeilen enthalten.

## Formatierung
- Für die Code-Formatierung wird der Standard VS Code-Formatter verwendet 
- Die Einhaltung der Guidelines wird über TSLint geprüft. 

## Weitere Informationen
- Weiterhin wird der offizielle Styleguide von Angular angewendet, s. https://angular.io/guide/styleguide
