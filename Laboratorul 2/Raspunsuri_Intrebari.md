*Răspunsuri pentru întrebările din interiorul problemelor (0-5)*



*\* Problema 0, punctul c: Dacă versiunea OpenGL ar fi mai mică de 3.3, aplicația ar eșua (crash) la pornire, deoarece șablonul necesită funcționalități din profilul Core minim 3.3.*



*\* Problema 1, punctul c: GL.ClearColor(...) se află în OnRenderFrame pentru a actualiza culoarea la fiecare cadru. Dacă ar fi mutat în OnLoad, s-ar executa o singură dată la pornire, iar animația cu schimbarea culorilor nu ar mai funcționa.*



*\* Problema 2, punctul c: Culoarea pixelilor din interior se obține prin procesul de interpolare. În etapa de rasterizare, placa video calculează automat un gradient între culorile celor 3 vârfuri, în funcție de distanța fiecărui pixel față de colțuri.*



*\* Problema 3, punctul c: Dacă apelăm GL.PolygonMode o singură dată în OnLoad, triunghiul va rămâne desenat permanent în acel mod (contur sau plin) pe tot parcursul rulării programului. Acest lucru ilustrează principiul de „mașină cu stări” (state machine) specific OpenGL: o setare activată rămâne valabilă până la următoarea modificare explicită.*



*\* Problema 4, punctul d: Folosirea unui pas fix pe cadru (ex. \_offset.X -= 0.01f;) ar face ca viteza triunghiului să depindă direct de rata de reîmprospătare (FPS). Pe un PC capabil de mii de cadre pe secundă, triunghiul s-ar mișca necontrolat de repede, zburând de pe ecran.*



*\* Problema 4, punctul e: Părțile care ies din cubul NDC (coordonatele cuprinse între -1 și 1) sunt tăiate și nu mai sunt desenate. Acest proces se numește clipping și are loc în pipeline-ul grafic după faza de Vertex Shader.*



*\* Problema 5, punctul b: Dacă se omite apelul UpdateVertices(), nu se va întâmpla nicio deformare vizibilă pe ecran. Funcția SetPosition modifică doar memoria RAM a procesorului, iar datele noi trebuie neapărat trimise către memoria plăcii video (VBO) folosind UpdateVertices().*



*\* Problema 5, punctul d: Problema 5 transferă mult mai multe date către GPU, trimițând întregul tablou de 18 float-uri la fiecare cadru, în comparație cu Problema 4 care trimitea doar un Vector3. Pentru un model cu un milion de vârfuri, acest transfer la fiecare cadru ar însemna un volum uriaș de date (zeci de MB), ceea ce ar încetini masiv performanța programului.*



*\* Problema 5, punctul e: Dacă toate vârfurile ajung în același punct, nu se mai desenează nimic. Aria triunghiului devine 0, astfel că nu mai există niciun pixel în interior care să fie colorat.*



*Răspunsuri pentru secțiunea „Întrebări de verificare”*



*1. Separarea OnUpdateFrame / OnRenderFrame: Sunt separate pentru a diferenția clar logica aplicației de randarea grafică. OnUpdateFrame este responsabilă pentru calcule (ex: citirea tastaturii, delta time). OnRenderFrame se ocupă strict cu trimiterea comenzilor video pe ecran (ex: GL.Clear, GL.DrawElements).*



*2. Apelarea funcțiilor GL.\*: Funcțiile OpenGL nu se pot apela în constructor deoarece contextul OpenGL și fereastra însăși nu au fost încă inițializate. OnLoad este prima funcție apelată abia după ce tot sistemul grafic este creat și gata să accepte comenzi.*



*3. Ce conține / nu conține un VAO: Un Vertex Array Object (VAO) reține regulile despre cum trebuie interpretate datele (atributele) și face legătura către buffere (VBO, EBO). VAO nu conține în sine datele brute ale vârfurilor. Dacă nu s-ar lega niciun VAO, placa video nu ar ști de unde și cum să citească informațiile, nefiind desenat nimic pe ecran.*



*4. Atribut vs Uniformă: Un atribut este o variabilă specifică și unică pentru fiecare vârf în parte (ex: coordonatele (X,Y,Z) sau culoarea (R,G,B) preluate din VBO). O uniformă este o variabilă globală care își păstrează aceeași valoare pe parcursul întregului apel de desenare pentru tot obiectul (ex: parametrul uOffset sau uTint pentru a schimba nuanța generală).*



*5. Stride și Offset: Stride = 24 indică lungimea în memorie (în octeți) a unui „pachet” întreg pentru un singur vârf (6 valori float a câte 4 octeți). Offset = 12 indică locul de unde începe citirea unui anumit atribut în interiorul pachetului (de exemplu, culoarea începe după primii 12 octeți ocupați de componenta de poziție).*



*6. Deformarea triunghiului: Când fereastra este lărgită sau micșorată, triunghiul se deformează deoarece sistemul de coordonate OpenGL standard (NDC) este blocat pe intervalul fix -1..1, pe care îl întinde orbește peste noile dimensiuni ale ferestrei. O Matrice de Proiecție (Camera) este elementul care rezolvă această distorsionare.*



*7. SwapBuffers() și cele două buffere: OpenGL randează grafica pe un buffer aflat în fundal (back buffer), apoi metoda SwapBuffers() inversează vizibilitatea acestuia cu buffer-ul afișat curent pe monitor (front buffer). Se folosesc două buffere (Double Buffering) pentru a se evita pâlpâirea pe ecran (flickering) și pentru ca utilizatorul să vadă mereu doar cadrul complet desenat, nu procesul de formare a acestuia.*



*8. args.Time (Delta Time): Această variabilă stochează timpul real (în secunde) scurs între generarea ultimului cadru și cel curent. Înmulțind orice deplasare cu args.Time, mișcarea devine fluidă și uniformă, indiferent dacă aplicația rulează lent sau extrem de rapid, anulând diferențele de cadre pe secundă între computere.*

