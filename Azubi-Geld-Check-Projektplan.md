# Azubi-Geld-Check

> **Projektplan für das Praxisprojekt**\
> Client-Server-Webanwendung zur Orientierung über mögliche finanzielle
> Leistungen für Auszubildende

## 1. Projektüberblick

### 1.1 Projektidee

Der **Azubi-Geld-Check** ist eine Webanwendung, die Auszubildenden in
wenigen Minuten eine erste Orientierung dazu gibt, welche finanziellen
Leistungen ihnen zustehen könnten. Zu passenden Leistungen werden
weiterführende Informationen und direkte Links zu den zuständigen
Antragsstellen bereitgestellt.

### 1.2 Ausgangslage und Problemstellung

Viele Auszubildende kennen ihre möglichen Ansprüche nicht. Beispiele
sind:

-   Berufsausbildungsbeihilfe (BAB)
-   Kindergeld
-   steuerliche Absetzbarkeit von Fahrtkosten

Die relevanten Informationen sind auf verschiedenen Behördenseiten
verteilt und häufig schwer verständlich. Dadurch werden mögliche
Ansprüche nicht erkannt und finanzielle Leistungen unter Umständen nicht
beantragt.

### 1.3 Zielgruppe

-   Auszubildende im Alter von ungefähr 16 bis 25 Jahren
-   Eltern, die Auszubildende unterstützen oder beraten
-   Ausbildungsbetriebe
-   Berufsschulen

### 1.4 Projektziel

Ziel des Praxisprojekts ist ein lauffähiger, abgesicherter und
getesteter **Minimum Viable Product (MVP)** als Client-Server-Anwendung
mit:

-   React-Frontend
-   REST-API mit C# und ASP.NET Core
-   PostgreSQL-Datenbank
-   Benutzerkonten
-   Admin-Bereich zur Pflege der Leistungen und Regeln

### 1.5 Abgrenzung

Der Azubi-Geld-Check bietet eine erste Orientierung. Er stellt **keine
Rechts- oder Steuerberatung** dar und garantiert keinen
Leistungsanspruch.

Jedes Ergebnis soll:

-   die verwendete Quelle nennen,
-   den Stand beziehungsweise Gültigkeitszeitraum der Regel angeben,
-   auf die zuständige Stelle verweisen, beispielsweise Agentur für
    Arbeit, Familienkasse oder Finanzamt.

### 1.6 Projektorganisation

-   **Projektart:** Neuentwicklung
-   **Arbeitsform:** Gruppenarbeit zu zweit

------------------------------------------------------------------------

## 2. Funktionsumfang und Prioritäten

Der Umfang des MVP wird bewusst auf drei Leistungen begrenzt. Dadurch
bleibt ausreichend Zeit für Sicherheitsmaßnahmen und Tests. Weitere
Funktionen werden iterativ ergänzt, sofern Zeit zur Verfügung steht.

### 2.1 MVP-Funktionen

  --------------------------------------------------------------------------
                     Priorität Funktion              Beschreibung
  ---------------------------- --------------------- -----------------------
                             1 Anspruchs-Check       Fragebogen zu Alter,
                                                     Ausbildungsart,
                                                     Vergütung,
                                                     Wohnsituation und
                                                     Arbeitsweg. Ausgabe
                                                     einer Ergebnisliste mit
                                                     Antragslinks. Ohne
                                                     Login nutzbar.

                             1 Regelprüfung für drei Prüfung der
                               Leistungen            Voraussetzungen für
                                                     BAB, Kindergeld und
                                                     Fahrtkosten als
                                                     Werbungskosten.

                             1 Registrierung und     Benutzer können ein
                               Login                 Konto anlegen, sich
                                                     anmelden und abmelden.

                             1 Ergebnisse speichern  Angemeldete Benutzer
                                                     können ihr Profil und
                                                     ihre Ergebnisse
                                                     speichern, ansehen und
                                                     löschen.

                             1 Admin-Bereich         Administratoren pflegen
                                                     Leistungen, Regeln,
                                                     Links und
                                                     Gültigkeitszeiträume.

                             2 Konto löschen         Benutzer können ihr
                                                     Konto einschließlich
                                                     aller zugehörigen Daten
                                                     löschen lassen (DSGVO).
  --------------------------------------------------------------------------

### 2.2 Geplante Ausbaustufen

  -----------------------------------------------------------------------------
                     Priorität Funktion              Beschreibung
  ---------------------------- --------------------- --------------------------
                             3 Weitere Leistungen    Ergänzung von Wohngeld,
                                                     vermögenswirksamen
                                                     Leistungen und
                                                     Arbeitnehmer-Sparzulage.

                             3 Schätzbeträge         Grobe Berechnung der
                                                     möglichen Höhe einer
                                                     Leistung statt einer
                                                     reinen Ja-/Nein-Aussage.

                             3 PDF-Export und        Ergebnisse als PDF
                               Erinnerungen          exportieren und an Fristen
                                                     erinnern, beispielsweise
                                                     an die Steuererklärung.
  -----------------------------------------------------------------------------

### 2.3 Fachliche Grundlage und Aktualität

Die Regeln -- beispielsweise Altersgrenzen, Bedingungen und Links --
werden anhand offizieller Quellen recherchiert. Vorgesehene Quellen
sind:

-   Agentur für Arbeit
-   Familienkasse
-   Finanzamt

Die Regeln werden mit Quelle und Gültigkeitsstand in der Datenbank
gespeichert und **nicht fest im Programmcode hinterlegt**. Dadurch
können sie aktualisiert werden, wenn sich gesetzliche oder fachliche
Vorgaben ändern.

------------------------------------------------------------------------

## 3. Architektur und Technologie

### 3.1 Architekturprinzip

Die Anwendung folgt einer klassischen
**Drei-Schichten-Client-Server-Architektur**:

1.  **Frontend:** React-Anwendung im Browser
2.  **Backend:** REST-API mit C# und ASP.NET Core
3.  **Datenbank:** PostgreSQL

Frontend und Backend kommunizieren ausschließlich über **HTTPS und
JSON**.

Die Datenbank ist von außen nicht erreichbar. Jeder Datenbankzugriff
erfolgt über die API. Die API prüft dabei Authentifizierung,
Berechtigungen und Eingaben.

### 3.2 Technologie-Stack

  -----------------------------------------------------------------------
  Bereich                 Technologie             Zweck
  ----------------------- ----------------------- -----------------------
  Frontend                React, TypeScript,      Fragebogen, Ergebnisse,
                          Vite, React Router      Login und
                                                  Admin-Oberfläche

  Backend                 C# mit ASP.NET Core Web REST-API, Regelprüfung
                          API, aktuelle           und Authentifizierung
                          .NET-LTS-Version        

  Datenzugriff            Entity Framework Core   ORM, parametrisierte
                          mit Npgsql              Abfragen und
                                                  Migrationen

  Datenbank               PostgreSQL              Speicherung von
                                                  Benutzern, Profilen,
                                                  Leistungen, Regeln und
                                                  Ergebnissen

  Authentifizierung       ASP.NET Core Identity   Registrierung,
                          und JWT                 Passwort-Hashing,
                                                  Rollen und Tokens

  API-Dokumentation       OpenAPI und Swagger UI  Automatische
                                                  API-Dokumentation und
                                                  manuelles Testen der
                                                  Endpunkte

  Versionsverwaltung      Git und GitHub,         Zusammenarbeit und
                          Feature-Branches und    Code-Reviews
                          Pull Requests           

  CI                      GitHub Actions          Build und automatische
                                                  Tests bei jedem Push

  Laufzeitumgebung        Docker und Docker       Einheitliche
                          Compose                 Entwicklungsumgebung;
                                                  Datenbank im internen
                                                  Netzwerk

  Planung                 GitHub Projects         Aufgaben,
                          (Kanban-Board)          Zuständigkeiten und
                                                  Fortschritt

  IDEs                    Visual Studio, Rider    Entwicklung
                          und VS Code             
  -----------------------------------------------------------------------

------------------------------------------------------------------------

## 4. Datenmodell und REST-API

### 4.1 Grundsatz der Datensparsamkeit

Es werden nur Daten gespeichert, die für die Anspruchsprüfung
erforderlich sind. Insbesondere werden **kein Name, kein genaues
Geburtsdatum und keine Adresse** benötigt.

### 4.2 Datenbanktabellen

Die folgenden Tabellen und Felder bilden das geplante Datenmodell ab.

  -----------------------------------------------------------------------
  Tabelle                 Wichtige Felder         Zweck und Hinweise
  ----------------------- ----------------------- -----------------------
  `Users`                 `Id`, `Email`,          Benutzerverwaltung über
                          `PasswortHash`, `Rolle` ASP.NET Core Identity
                          (User/Admin),           
                          `Lockout-Zähler`        

  `Profile`               `Id`, `UserId` (FK),    Ein Profil pro Benutzer
                          `Geburtsjahr`,          
                          `Ausbildungsart`,       
                          `Bruttovergütung`,      
                          `WohntBeiEltern`,       
                          `ArbeitswegKm`          

  `Leistungen`            `Id`, `Name`,           Leistungen werden durch
                          `Beschreibung`,         Administratoren
                          `AntragsLink`,          gepflegt
                          `Quelle`, `GültigAb`,   
                          `GültigBis`, `Aktiv`    

  `Regeln`                `Id`, `LeistungId`      Bedingungen einer
                          (FK), `Feld`,           Leistung,
                          `Operator`, `Wert`      beispielsweise
                                                  `Alter <= 25`

  `Ergebnisse`            `Id`, `UserId` (FK),    Gespeicherte
                          `ErstelltAm`,           Ergebnisse; nur für den
                          `Eingaben` (JSON),      jeweiligen Benutzer
                          `Treffer` (JSON)        sichtbar

  `AuditLog`              `Id`, `UserId`,         Protokollierung von
                          `Aktion`, `Zeitpunkt`,  Logins und
                          `Erfolgreich`           Admin-Änderungen
  -----------------------------------------------------------------------

### 4.3 REST-Endpunkte

Alle Endpunkte werden über OpenAPI dokumentiert. Fehler werden
einheitlich im Format **ProblemDetails gemäß RFC 9457** zurückgegeben.

  -------------------------------------------------------------------------------
  Methode           Pfad                      Zugriff           Zweck
  ----------------- ------------------------- ----------------- -----------------
  `POST`            `/api/auth/register`      Öffentlich        Benutzerkonto
                                                                anlegen

  `POST`            `/api/auth/login`         Öffentlich, mit   Anmelden und
                                              Rate Limit        Token erhalten

  `POST`            `/api/auth/logout`        Benutzer          Abmelden und
                                                                Token ungültig
                                                                machen

  `GET`             `/api/leistungen`         Öffentlich        Aktive Leistungen
                                                                anzeigen

  `POST`            `/api/check`              Öffentlich        Anspruchs-Check
                                                                durchführen, ohne
                                                                das Ergebnis zu
                                                                speichern

  `GET` / `PUT`     `/api/profil`             Benutzer          Eigenes Profil
                                                                lesen und ändern

  `GET` / `POST`    `/api/ergebnisse`         Benutzer          Eigene Ergebnisse
                                                                lesen und
                                                                speichern

  `DELETE`          `/api/ergebnisse/{id}`    Benutzer, nur     Ein Ergebnis
                                              eigene Ergebnisse löschen

  `DELETE`          `/api/konto`              Benutzer          Konto
                                                                einschließlich
                                                                aller zugehörigen
                                                                Daten löschen

  `POST` / `PUT` /  `/api/admin/leistungen`   Admin             Leistungen
  `DELETE`                                                      verwalten

  `POST` / `PUT` /  `/api/admin/regeln`       Admin             Regeln verwalten
  `DELETE`                                                      
  -------------------------------------------------------------------------------

------------------------------------------------------------------------

## 5. Authentifizierung und Sicherheitskonzept

### 5.1 Sicherheitsgrundlage

Die Sicherheitsmaßnahmen orientieren sich an folgenden
BSI-IT-Grundschutz-Bausteinen:

-   CON.2 -- Datenschutz
-   CON.8 -- Software-Entwicklung
-   CON.10 -- Entwicklung von Webanwendungen
-   APP.3.1 -- Webanwendungen und Webservices
-   APP.3.2 -- Webserver
-   APP.4.3 -- Relationale Datenbanken

Zusätzlich dienen die **OWASP Top 10** als Orientierung.

Maßnahmen der Priorität 1 sollen im MVP umgesetzt werden. Maßnahmen der
Priorität 2 werden umgesetzt, sofern Zeit bleibt.

### 5.2 Authentifizierung und Autorisierung

-   Registrierung und Login erfolgen über ASP.NET Core Identity.
-   Passwörter werden ausschließlich als Hash gespeichert
    (Identity-Standard PBKDF2, optional Argon2).
-   Passwort-Richtlinie: mindestens 10 Zeichen sowie Abgleich gegen
    bekannte unsichere Passwörter.
-   Kurzlebiger JWT-Access-Token mit 15 Minuten Laufzeit.
-   Refresh-Token als `HttpOnly`-, `Secure`- und `SameSite`-Cookie.
-   Rollenbasierte Zugriffskontrolle (RBAC) mit den Rollen `User` und
    `Admin`.
-   Admin-Endpunkte sind ausschließlich mit der Admin-Rolle erreichbar.
-   Jeder Zugriff auf Profile und Ergebnisse prüft, ob die Daten dem
    angemeldeten Benutzer gehören. Damit soll Broken Access Control
    verhindert werden.
-   Kontosperre nach 5 Fehlversuchen für 15 Minuten.

### 5.3 Sicherheitsmaßnahmen

  -------------------------------------------------------------------------------------------------
              Priorität Maßnahme             Umsetzung                             Bezug
  --------------------- -------------------- ------------------------------------- ----------------
                      1 Verschlüsselte       HTTPS überall, HSTS und Weiterleitung APP.3.1, OWASP
                        Übertragung          von HTTP auf HTTPS                    A02

                      1 Sichere HTTP-Header  Content-Security-Policy,              APP.3.1.A21
                                             Strict-Transport-Security,            
                                             X-Content-Type-Options, Content-Type  
                                             und Cache-Control                     

                      1 Eingabevalidierung   Serverseitige Validierung mit         CON.10, OWASP
                                             DataAnnotations oder                  A03
                                             FluentValidation, Whitelists und      
                                             Wertebereichen                        

                      1 Schutz vor           Ausschließlich EF Core                APP.4.3, OWASP
                        SQL-Injection        beziehungsweise parametrisierte       A03
                                             Abfragen; kein zusammengesetztes SQL  

                      1 Rate Limiting        ASP.NET Core Rate Limiter für Login,  APP.3.1
                                             Registrierung und Anspruchs-Check     

                      1 Sichere              ProblemDetails; im Produktivbetrieb   CON.10
                        Fehlerbehandlung     keine Stacktraces oder internen       
                                             Informationen ausgeben                

                      1 Geheimnisse schützen Keine Passwörter oder Schlüssel im    CON.8
                                             Repository; User-Secrets              
                                             beziehungsweise Umgebungsvariablen    
                                             verwenden                             

                      1 CORS                 Ausschließlich die eigene             APP.3.1
                                             Frontend-Adresse erlauben             

                      1 Datensparsamkeit     Nur Geburtsjahr speichern, Check ohne CON.2
                                             Konto ermöglichen und Kontolöschung   
                                             anbieten                              

                      2 Protokollierung      Serilog für Logins, Fehlversuche und  APP.3.1, OWASP
                                             Admin-Änderungen; keine Passwörter    A09
                                             oder Profildaten protokollieren       

                      2 Datenbank härten     Eigenen DB-Benutzer mit minimalen     APP.4.3
                                             Rechten verwenden, Datenbank nur im   
                                             internen Docker-Netz erreichbar       
                                             machen und Backups vorsehen           

                      2 Abhängigkeiten       `dotnet list package --vulnerable`,   CON.8, OWASP A06
                        prüfen               `npm audit` und Dependabot einsetzen  

                      2 Vier-Augen-Prinzip   Jeder Pull Request wird vom anderen   CON.8
                                             Teammitglied geprüft                  
  -------------------------------------------------------------------------------------------------

------------------------------------------------------------------------

## 6. Teststrategie

### 6.1 Testansatz

Es wird nach der **Testpyramide** getestet:

-   Viele schnelle Unit-Tests für die Regelprüfung
-   Gezielte Integrationstests für API und Sicherheitsfunktionen
-   Wenige End-to-End-Tests für die wichtigsten Benutzerabläufe

Alle automatisierten Tests sollen bei jedem Push über GitHub Actions
ausgeführt werden. Ein Pull Request darf nur gemergt werden, wenn alle
erforderlichen Tests erfolgreich sind.

### 6.2 Geplante Tests

  --------------------------------------------------------------------------------------------
  Testebene             Werkzeug                  Testumfang                 Stufe
  --------------------- ------------------------- -------------------------- -----------------
  Backend-Unit-Tests    xUnit, FluentAssertions   Regelprüfung je Leistung,  MVP
                                                  Grenzwerte (z. B. Alter    
                                                  genau an der Grenze) und   
                                                  Validierung                

  Frontend-Unit-Tests   Vitest, React Testing     Fragebogen-Komponenten,    MVP
                        Library                   Pflichtfelder und          
                                                  Ergebnisanzeige            

  Integrationstests     WebApplicationFactory,    API-Endpunkte,             MVP
                        Testcontainers mit echtem Datenbankinteraktion und   
                        PostgreSQL im Container   Statuscodes `200`, `400`,  
                                                  `401`, `403`, `404`        

  Sicherheitstests      xUnit-Integrationstests   Zugriff ohne Token         MVP
                                                  (`401`), User auf          
                                                  Admin-Endpunkt (`403`),    
                                                  fremde Ergebnis-ID         
                                                  (`404`), Kontosperre nach  
                                                  5 Fehlversuchen,           
                                                  SQL-Injection-Eingaben und 
                                                  vorhandene Security-Header 

  End-to-End-Tests      Playwright                Registrierung, Login,      MVP
                                                  Anspruchs-Check, Ergebnis  
                                                  speichern und Admin ändert 
                                                  eine Leistung              

  Schwachstellen-Scan   OWASP ZAP (Baseline-Scan) Automatisierter Scan der   MVP
                                                  laufenden Anwendung        

  Manuelle Tests        Swagger UI und            Testfälle mit              MVP
                        Testprotokoll             Beispiel-Auszubildenden,   
                                                  z. B. 17 Jahre bei den     
                                                  Eltern sowie 22 Jahre mit  
                                                  eigener Wohnung und 40 km  
                                                  Arbeitsweg                 

  Testabdeckung         Coverlet                  Ziel: mindestens 80 % für  Ausbau
                                                  die Regelprüfung           

  Akzeptanztests        Reqnroll (Gherkin,        Fachliche Szenarien in     Ausbau
                        ähnlich Cucumber)         Alltagssprache             

  Lasttests             k6                        Verhalten bei vielen       Ausbau
                                                  gleichzeitigen Anfragen    

  Mutationstests        Stryker.NET               Qualität der Unit-Tests    Ausbau
                                                  überprüfen                 

  Nutzertest            Mitschüler der Klasse     Verständlichkeit des       Ausbau
                                                  Fragebogens prüfen         
  --------------------------------------------------------------------------------------------

### 6.3 Testdaten

Es werden ausschließlich **ausgedachte Testpersonen** verwendet. Echte
personenbezogene Daten sind nicht Teil der Tests.

------------------------------------------------------------------------

## 7. Vorgehen und Zeitplan

### 7.1 Arbeitsweise

Die Entwicklung erfolgt iterativ in kurzen Etappen, angelehnt an Scrum
und unterstützt durch ein Kanban-Board. Nach jeder Etappe soll ein
lauffähiger Stand vorhanden sein.

Geplant ist zunächst eine Projektwoche für das MVP. Danach folgen --
abhängig von Zeit und Priorität -- weitere Ausbaustufen.

### 7.2 Projektwoche

  -------------------------------------------------------------------------
                           Tag Schwerpunkt           Geplante Aufgaben
  ---------------------------- --------------------- ----------------------
                             1 Start                 Repository einrichten,
                                                     Docker Compose mit API
                                                     und PostgreSQL,
                                                     CI-Pipeline und
                                                     Frontend-Grundgerüst
                                                     erstellen. Regeln für
                                                     BAB, Kindergeld und
                                                     Fahrtkosten inklusive
                                                     Quellen recherchieren.

                             2 Kernfunktion          Datenmodell und
                                                     Migrationen erstellen,
                                                     `/api/check` mit
                                                     Regelprüfung
                                                     implementieren und
                                                     Unit-Tests ergänzen.
                                                     Fragebogen und
                                                     Ergebnisseite im
                                                     Frontend umsetzen.

                             3 Benutzerkonten        Registrierung, Login,
                                                     JWT und Rollen
                                                     implementieren.
                                                     Profile und Ergebnisse
                                                     speichern. Login- und
                                                     Kontoseiten im
                                                     Frontend erstellen.

                             4 Absicherung und Admin Sicherheitsmaßnahmen
                                                     der Priorität 1
                                                     umsetzen,
                                                     Admin-Bereich
                                                     erstellen sowie
                                                     Integrations- und
                                                     Sicherheitstests
                                                     durchführen.

                             5 Qualität und          End-to-End-Tests mit
                               Abschluss             Playwright, ZAP-Scan,
                                                     Fehlerbehebung,
                                                     Dokumentation und
                                                     Präsentation.

                        Danach Ausbau                Maßnahmen der
                                                     Priorität 2, weitere
                                                     Leistungen,
                                                     Schätzbeträge und
                                                     erweiterte Tests
                                                     umsetzen.
  -------------------------------------------------------------------------

### 7.3 Definition of Done

Eine Aufgabe gilt als abgeschlossen, wenn:

-   der Code in einem Pull Request vom anderen Teammitglied geprüft
    wurde,
-   die zugehörigen Tests vorhanden und erfolgreich sind,
-   der zugehörige Endpunkt in OpenAPI dokumentiert ist.

------------------------------------------------------------------------

## 8. Aufgabenverteilung

Person A verantwortet Backend, Datenbank und Sicherheit. Person B
verantwortet Frontend, End-to-End-Tests und Dokumentation.

Beide Teammitglieder prüfen gegenseitig ihre Pull Requests, damit beide
das Gesamtsystem verstehen. Die Aufgabenverteilung und der
Arbeitsfortschritt sind über GitHub-Commits und das Kanban-Board
nachvollziehbar.

  -------------------------------------------------------------------------
  Aufgabe                 Person A                Person B
  ----------------------- ----------------------- -------------------------
  Projekt-Setup           Docker Compose,         React-Projekt, Routing
                          PostgreSQL und          und Grundlayout
                          CI-Pipeline             

  Fachliche Recherche     Kindergeld und          BAB
                          Fahrtkosten             

  Datenmodell und API     Entitäten, Migrationen, --
                          alle Endpunkte und      
                          Regelprüfung            

  Authentifizierung       Identity, JWT, Rollen   Login, Registrierung und
                          und Kontosperre         Token-Handling im
                                                  Frontend

  Oberfläche              --                      Fragebogen,
                                                  Ergebnisseite,
                                                  Kontobereich und
                                                  Admin-Oberfläche

  Absicherung             Header, Validierung,    Content-Security-Policy
                          Rate Limiting, CORS,    im Frontend prüfen und
                          Logging und             ZAP-Scan
                          Datenbank-Härtung       

  Tests                   Backend-Unit-Tests,     Frontend-Unit-Tests,
                          Integrations- und       Playwright-Tests und
                          Sicherheitstests        manuelles Testprotokoll

  Dokumentation           OpenAPI-Dokumentation   README, Benutzerhinweise
                          und Sicherheitskonzept  und Datenschutzhinweis

  Code-Review             Prüft Pull Requests von Prüft Pull Requests von
                          Person B                Person A

  Präsentation            Architektur und         Projektidee, Demo und
                          Sicherheit              Teststrategie
  -------------------------------------------------------------------------

------------------------------------------------------------------------

## 9. Risiken und Gegenmaßnahmen

  -----------------------------------------------------------------------
  Risiko                              Gegenmaßnahme
  ----------------------------------- -----------------------------------
  Fachlich falsche Ergebnisse         Ausschließlich offizielle Quellen
                                      verwenden. Bei jedem Ergebnis
                                      Quelle und Stand angeben, auf den
                                      Orientierungscharakter hinweisen
                                      und zur zuständigen Stelle
                                      verlinken.

  Regeln ändern sich jährlich         Regeln mit Gültigkeitszeitraum in
                                      der Datenbank speichern und über
                                      den Admin-Bereich pflegen.

  Zu wenig Zeit                       Strenge Priorisierung: zuerst MVP
                                      mit Sicherheitsmaßnahmen der
                                      Priorität 1 und Tests, danach
                                      Ausbau.

  Ausfall eines Teammitglieds         Gegenseitige Code-Reviews,
                                      Dokumentation im README und
                                      Aufgabenverwaltung über das Board.

  Sensible Daten (Einkommen und       Datensparsamkeit, Anspruchs-Check
  Wohnsituation)                      ohne Konto, Kontolöschung und keine
                                      Profildaten in Logs.
  -----------------------------------------------------------------------

## 10. Ausblick

Nach Abschluss des Praxisprojekts kann die Anwendung schrittweise
erweitert werden. Denkbare Erweiterungen sind:

-   zusätzliche Leistungen, beispielsweise Wohngeld,
-   Schätzbeträge zur möglichen Höhe von Leistungen,
-   Erinnerungen an wichtige Fristen,
-   Tests der Anwendung gemeinsam mit Berufsschulen oder
    Ausbildungsbetrieben.

------------------------------------------------------------------------

## 11. Projektleitlinien auf einen Blick

1.  **Fachlich nachvollziehbar:** Ergebnisse müssen Quelle und
    Regelstand nennen.
2.  **Keine Beratung vortäuschen:** Die Anwendung bietet Orientierung
    und ersetzt keine Rechts- oder Steuerberatung.
3.  **Datensparsam entwickeln:** Nur notwendige Daten speichern und den
    Check ohne Benutzerkonto ermöglichen.
4.  **Sicherheit von Anfang an berücksichtigen:** Authentifizierung,
    Autorisierung, Validierung und Schutz der Daten gehören zum MVP.
5.  **Regeln konfigurierbar halten:** Fachliche Bedingungen und
    Gültigkeitszeiträume gehören in die Datenbank, nicht fest in den
    Code.
6.  **Qualität absichern:** Tests, Code-Reviews und CI sind Teil des
    Entwicklungsprozesses.
7.  **Umfang kontrollieren:** Erst das priorisierte MVP fertigstellen,
    danach Erweiterungen angehen.
