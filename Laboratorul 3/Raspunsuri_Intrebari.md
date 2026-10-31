*Răspunsuri pentru întrebările din interiorul problemelor*



*\* Problema 0, punctul b:*

&#x20; *- Versiune OpenGL: 3.3.0 - Build 27.20.100.8280*

&#x20; *- Producător (Vendor): Intel*

&#x20; *- Placă grafică (Renderer): Intel(R) UHD Graphics*



*\* Problema 1, punctul b: Muchia care apare în interiorul dreptunghiului (la activarea conturului cu tasta F) este diagonala. Aceasta reprezintă latura comună a celor două triunghiuri individuale din care este construit, de fapt, dreptunghiul.*



*\* Problema 1, punctul c: Degradeul nu este neted pe tot dreptunghiul deoarece interpolarea culorilor se face separat pentru fiecare dintre cele două triunghiuri. Pe diagonala comună, direcția gradientului se schimbă brusc, formând acea „cută” vizibilă.*



*\* Problema 1, punctul d: Când toate vârfurile au aceeași culoare, dreptunghiul devine colorat uniform. Când setăm culori diferite pe verticală, degradeul curge perfect de sus în jos, iar „cuta” de pe diagonală dispare, deoarece interpolarea în ambele triunghiuri se face în aceeași direcție, ascunzând limita dintre ele.*



*\* Problema 2, punctul d:* 

&#x20; *- Câte vârfuri și câți indici are un poligon cu n laturi?* 

*Are n + 1 vârfuri (unul în centru și n pe margini) și 3 \* n indici (deoarece sunt n triunghiuri, fiecare format din 3 indici).*



&#x20; *- De ce este necesar % sides la indicele ultimului triunghi?* 

*Pentru a „închide” poligonul. Ultimul triunghi trebuie să se lege înapoi de primul colț, iar operatorul % (modulo) previne depășirea numărului maxim de vârfuri și resetează indexul corect.*



&#x20; *- Ce s-ar vedea dacă ați scrie i / sides în loc de (float)i / sides?* 

*Deoarece ambele numere ar fi de tip întreg (int), rezultatul împărțirii ar fi mereu 0. Ca urmare, toate colțurile ar primi nuanța 0 (care înseamnă roșu), iar tot poligonul ar fi desenat în roșu complet, pierzându-se culorile curcubeului.*



*\* Problema 3, punctul a: Nu, la apăsarea tastei Tab forma nu este un cerc rotund perfect, ci arată ca o elipsă (este mai lată pe orizontală).*



*\* Problema 3, punctul b:*

&#x20; *- Câți pixeli au semiaxele elipsei într-o fereastră 800x600? Conform formulelor de transformare a sistemului NDC pe ecran, o rază de 0.5 devine o semiaxă de 200 de pixeli pe orizontală (0.5 \* 400) și o semiaxă de 150 de pixeli pe verticală (0.5 \* 300).*

&#x20; *- Ce se întâmplă la redimensionare? Dacă tragem de fereastră pentru a o face pătrată, elipsa devine un cerc rotund. Dacă o lărgim și mai mult pe orizontală, elipsa se va lăți proporțional.*



*\* Problema 3, punctul c:* 

&#x20; *- Prin ce număr am înmulți coordonatele X? Ar trebui să înmulțim coordonatele X cu 0.75 (raportul 600 / 800) pentru a compensa lățimea suplimentară a ecranului.*

&#x20; *- De ce nu este o soluție bună? Deoarece această "reparare" matematică ar funcționa doar cât timp fereastra stă exact la 800x600. Dacă utilizatorul redimensionează fereastra, forma se va deforma din nou.*

&#x20;

*\* Problema 3, punctul d: La testarea poligoanelor, pe măsură ce numărul de laturi crește, forma devine tot mai rotunjită. De obicei, începând de la n = 32, laturile individuale devin foarte greu de distins cu ochiul liber, formând un cerc aproape perfect.*





*\* Problema 4, punctul b:*

&#x20; *Valorile obținute prin FromHsv sunt:*

&#x20; *- (0.5, 1, 1) -> (0, 1, 1) (culoarea Cyan)*

&#x20; *- (0.25, 1, 1) -> (0.5, 1, 0) (un Verde-Gălbui, aflat între galben și verde)*

&#x20; *- (0, 0, 1) -> (1, 1, 1) (Alb, deoarece saturația 0 înseamnă nuanță de gri, iar luminozitatea 1 înseamnă alb pur)*

&#x20; *- (0, 1, 0.5) -> (0.5, 0, 0) (Roșu închis, deoarece nuanța 0 este roșu, dar luminozitatea este înjumătățită)*



*\* Problema 4, punctul c:* 

&#x20; *- De ce nu trebuie să aducem noi \_elapsed \* 0.05 în intervalul \[0, 1)? Nu este nevoie deoarece funcția ColorUtil.FromHsv face deja acest lucru automat chiar pe prima linie de cod (prin "h -= MathF.Floor(h);"). Acest mecanism permite timpului să crească la infinit, reluând mereu ciclul de la zero fără să ne facem noi griji.*



*\* Problema 4, punctul d:* 

&#x20; *- Ce se întâmplă dacă schimbăm viteza la 0.5 și saturația la 1? Ciclul de culori rulează mult mai rapid (de 10 ori mai repede). De asemenea, fiindcă saturația este setată la maxim (1), culorile au devenit mult mai vii și mai aprinse (culori pure), nemaifiind pastelate/șterse cum erau la 0.5.*



*\* Problema 5, punctul b: Ce se oprește la pauză și ce continuă să ruleze?*

&#x20; *La apăsarea tastei P, variabila "\_elapsed" nu mai crește, deci timpul animației (unda și ciclul de culori al fundalului) îngheață. Totuși, bucla principală a aplicației, redesenarea cadrelor și procesarea tastaturii (deplasarea formei cu săgețile) continuă să ruleze normal, deoarece ele nu depind de \_elapsed, ci de args.Time (delta time).*



*\* Problema 5, punctul c:* 

&#x20; *Folosind formula (vColor = aPosition \* 0.5 + 0.5):*

&#x20; *- Un vârf aflat în (0, 0, 0) va primi culoarea (0.5, 0.5, 0.5), adică un Gri mediu.*

&#x20; *- Un vârf aflat în (-1, -1, 0) va primi culoarea (0, 0, 0.5), adică Albastru închis (x și y devin 0, dar z devine 0.5).*



*\* Problema 5, punctul d: Încotro se deplasează unda în fiecare caz?*

&#x20; *- Cu "+ uTime", faza crește, deci unda se deplasează spre stânga.*

&#x20; *- Cu "- uTime", faza scade, deci unda s-ar deplasa în direcția opusă, adică spre dreapta.*



*\* Problema 5, punctul e: Câți octeți s-ar copia pe CPU vs GPU pentru 1 milion de vârfuri?*

&#x20; *- Pe CPU (cum am făcut în Lab 2): 1 milion de vârfuri \* 6 float-uri pe vârf \* 4 octeți pe float = 24 de milioane de octeți (aprox. 24 MB) care ar trebui trimiși către placa video la FIECARE cadru (ceea ce ar distruge performanța).*

&#x20; *- Pe GPU (cum am făcut acum în vertex shader): Se trimit fix 0 octeți în plus pentru deformare! Datele vârfurilor rămân nemodificate în VBO (în memoria video), iar noi trimitem doar o variabilă uniformă de 4 octeți (uTime) pentru a anima totul direct pe placa video.*



*Răspunsuri - Întrebări de verificare (Laborator 3)*



*1. De ce un poligon regulat cu n laturi se desenează cu n+1 vârfuri și 3\*n indici? Ce rol are centrul?*

*Un poligon cu n laturi are n colțuri. Pentru a umple forma desenând triunghiuri în evantai, se mai adaugă un vârf în centru, rezultând n+1 vârfuri. Forma va fi compusă din n triunghiuri (câte unul pentru fiecare latură a poligonului). Deoarece fiecare triunghi are nevoie de 3 indici, rezultă 3\*n indici în total. Rolul centrului este de a servi drept punct comun de legătură (pivot) pentru toate triunghiurile evantaiului.*



*2. De ce cercul arată ca o elipsă într-o fereastră de 800x600? Care element al proiectului va rezolva problema?*

*Cercul este turtit deoarece sistemul de coordonate normalizate (NDC) asociază orbește intervalul \[-1, 1] lățimii și înălțimii ferestrei, ignorând proporțiile (aspect ratio) acesteia. Cum 800 de pixeli pe lățime reprezintă o distanță mai mare decât 600 de pixeli pe înălțime, 1 unitate NDC pe orizontală este mai "lungă" vizual. Elementul care va rezolva problema în laboratorul următor este Matricea de Proiecție (Camera).*



*3. Care este diferența dintre RGB și HSV? De ce este HSV potrivit pentru un ciclu de culori?*

*RGB definește culoarea prin cantitățile de roșu, verde și albastru. HSV definește culoarea prin Nuanță (poziția pe cercul curcubeului), Saturație (puritate) și Luminozitate. HSV este ideal pentru un ciclu de culori deoarece ne permite să trecem prin tot curcubeul pur și simplu modificând constant o singură variabilă (Nuanța/Hue), în timp ce în RGB ar fi necesar un algoritm complicat de comutare a celor trei canale.*



*4. De ce uTime este o uniformă și nu un atribut de vârf?*

*Timpul (uTime) are exact aceeași valoare globală pentru absolut tot desenul în cadrul unui frame curent. O uniformă este perfectă pentru valori care rămân constante pentru întregul obiect. Un atribut de vârf (precum culoarea colțului sau coordonata X) este folosit pentru date care variază strict de la un punct la altul.*



*5. Ce se oprește când apăsați P și ce continuă să ruleze? De ce nu am oprit bucla principală?*

*La apăsarea tastei P se oprește doar timpul animației (variabila \_elapsed), ceea ce face ca unda și culorile să înghețe pe ecran. Continuă să ruleze mișcarea cu săgețile (deoarece depinde de delta time, nu de \_elapsed) și procesul de redesenare a ecranului. Nu am oprit bucla principală deoarece aplicația s-ar bloca complet ("Not Responding" în Windows) și nu ar mai putea citi când vrem să scoatem pauza sau să mișcăm forma.*



*6. Care este avantajul calculării unei deformări în vertex shader, față de modificarea vârfurilor pe CPU și recopierea lor în VBO?*

*Avantajul este performanța extremă. Dacă am modifica vârfurile pe CPU, ar trebui să trimitem zeci de MB de date (pozițiile recalculate) prin placa de bază către placa video la FIECARE cadru (cum era apelul BufferSubData). Deformarea direct în Vertex Shader se întâmplă nativ pe procesorul grafic (GPU), în paralel pentru toate punctele, fără transfer masiv de date (trimitem doar 4 octeți care reprezintă valoarea timpului).*















