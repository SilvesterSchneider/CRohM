# CRohM
Backend:	1. Microsoft SQL Server Express installieren
			2. Microsoft SQL Management Studio installieren
			3. Visual Studio installieren
			4. Backend projekt öffnen mit Visual Studio
			5. Wenn nachdem das Projekt WebApi gestartet wurde, die Datenbank nicht automatisch erzeugt wurde dann
				1. Paket-Manager-Konsole aufrufen (In Search Region eingeben)
				2. Wenn keine Migrationen im Ordner ModelLayer\Migrations vorhanden sind oder falls gelöscht werden musste weil fehlerhaft
					1. "add-migration init" ausführen in Konsole
				3. "update-database" ausführen in Konsole

Frontend:	1. Visual Studio Code installieren
			2. Node JS installieren
			3. Visual Studio Code starten und frontend Projekt öffnen
			4. Neuen Terminal starten
			5. Wenn erstmaliger Start
				1. "npm install -g @angular/cli" ausführen
				2. "npm install" ausführen
			6. "ng serve" ausführen
			7. Browser öffnen und https://localhost:4200 öffnen
			8. Als admin mit Passwort @dm1n1stR4tOr anmelden
			9. Genießen!

----------

## CRohM als Docker-Version (Fullstack) starten

> - Docker wird sich die Abhängigkeiten vom ([docker hub](https://hub.docker.com/r/crohmcrms/crohm_crms/tags)) ziehen und Fullstack inklusive DB starten.
> - Da jeder branch auf Git seinen eigenen Container besitzt, wird der aktuell ausgecheckte branch gezogen.
> - Die DB wird vollständig leer sein und ist nicht persistent

1. Docker nach Anleitung installieren ([docker docs](https://docs.docker.com/get-docker/))
2. Datei `.env` nach eigenen Wünschen anpassen
3. Über die Komandozeile vom root Ordner (CRohM) `sh ./misc/docker-compose-up.sh` ausführen
4. Der Container sollte nun über die unter `.env` konfigurierten Werte erreichbar sein

----------

## CRohM als Docker-Version (Fullstack) stoppen

- Über die Komandozeile vom root Ordner (CRohM) `sh ./misc/docker-compose-down.sh` ausführen

**Tipp:** Gelegentlich über die Komandozeile `docker image prune --all` ausführen, um alte Container zu entfernen
