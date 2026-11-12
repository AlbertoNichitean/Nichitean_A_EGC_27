1. La P1 (Proiecția și FOV):
Întrebare: Lățiți fereastra și apăsați din nou I: care dintre cele două unghiuri s-a schimbat?

Răspuns: Se schimbă hFOV (FOV-ul orizontal). vFOV (cel vertical) rămâne fix la 60° (sau valoarea setată de tine), pentru că înălțimea ferestrei dictează unghiul vertical, iar lățimea mărită extinde automat câmpul vizual pe orizontală prin formula de aspect.

2. La P2 (Testul de adâncime - tasta Z):
Întrebare: Apăsați Z (testul de adâncime pornește). Imaginea se strică. De ce?

Răspuns: Deoarece buffer-ul de adâncime (depth buffer) nu este șters la fiecare cadru nou (GL.Clear nu ștergea și adâncimea, ci doar culoarea). Valorile de adâncime din cadrul anterior rămân memorate și blochează desenarea corectă a noilor obiecte.

3. La P3 (Sfera UV):
Întrebare 1: Câte vârfuri și câte triunghiuri are sfera mare (32 de felii / slices, 16 benzi / stacks)?

-Coloane: $slices + 1 = 33$ vârfuri pe rând (inclusiv cusătura).

-Rânduri: $stacks + 1 = 17$ rânduri de vârfuri.

-Total vârfuri: $17 \times 33 = 561$ vârfuri.

-Total triunghiuri: $stacks \times slices \times 2 = 16 \times 32 \times 2 = 1024$ triunghiuri (fiecare din cele $16 \times 32$ celule are câte 2 triunghiuri).


Întrebare 2: Câte dintre triunghiuri nu se văd și de ce?Răspuns: Triunghiurile de la cei doi poli (polul nord și polul sud).


La pol, raza paralelei ($r\_p$) este 0, ceea ce face ca toate vârfurile de pe rândul respectiv să cadă în același punct fizic. Astfel, triunghiurile adiacente polului au arie zero (două vârfuri se suprapun), dar sunt păstrate în cod pentru uniformitatea grilei. 

4. La P4 (Camera FPS și MaxPitch):
Întrebare: Puneți temporar MaxPitch = 90 și priviți drept în sus: ce se întâmplă?

Răspuns: Imaginea se răstoarnă sau începe să se rotească haotic/incontrolabil. Motivul este că la 90° direcția camerei devine paralelă cu axa Y a lumii, produsul vectorial pentru calculul dreptei devine nul (vector nul), iar vectorii camerei își pierd stabilitatea (fenomen de tip gimbal lock). Limitarea la $\pm89^\circ$ previne exact această problemă.   




