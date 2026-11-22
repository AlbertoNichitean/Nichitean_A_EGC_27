## Întrebări Laborator 06

### P1. Ceasul (`TODO 6.1`)
**Întrebare:** Ce s-ar întâmpla cu un obiect care cade sub gravitație, dacă pasul nu ar fi limitat?

**Răspuns:**
Dacă pasul de timp ($\Delta t$) nu ar fi limitat la `MaxDelta` (`0,1 s`), la o blocare temporară a aplicației (de exemplu `0,5 s`), obiectul ar parcurge o distanță foarte mare într-un singur cadru. Din această cauză:
- ar „sări” brusc în jos (mișcare sacadată / smucită);
- ar putea trece direct prin podea sau prin alte obiecte fără să fie detectată coliziunea.

---

### P4. Input și jucătorul (`TODO 6.4`, `TODO 6.5`)
**Întrebare:** De ce saltul se declanșează cu `WasReleased` și nu cu `!IsDown`?

**Răspuns:**
- `WasReleased` este adevărat **într-un singur cadru**, exact în momentul în care tasta `Space` este eliberată după încărcare.
- `!IsDown` este adevărat **în toate cadrele** în care tasta nu este apăsată (inclusiv când jucătorul stă pe podea), ceea ce ar declanșa saltul în mod repetat la fiecare cadru, nu o singură dată la eliberare.

---

### P5*. Animație ierarhică (`TODO 6.6`)
**Întrebare:** Ce se schimbă dacă pivotul lunii devine copilul planetei?

**Răspuns:**
Dacă pivotul lunii ar deveni copilul planetei, luna ar moșteni și **rotația planetei în jurul propriei axe**, nu doar deplasarea acesteia pe orbită în jurul soarelui. Astfel, luna s-ar roti mult mai repede în jurul planetei (viteza de rotație axială a planetei s-ar aduna la viteza de revoluție a lunii).
