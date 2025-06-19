# SessionCompare

Библиотека для сравнения атрибутов элемента между сессиями AVEVA.

---

## Методы

| Метод                                             | Результат | Описание                                                                                |
|:--------------------------------------------------|:----------|:----------------------------------------------------------------------------------------|
| ByDates(STRING date1, STRING date2)               | ARRAY     | Сравнение атрибутов текущего элемента (CE) между сессиями до указанных дат date1, date2 |
| ByDates(STRING refno, STRING date1, STRING date2) | ARRAY     | Сравнение атрибутов элемента refno между сессиями до указанных дат date1, date2         |
| BySessions(REAL sess1, REAL sess2)                | ARRAY     | Сравнение атрибутов текущего элемента (CE) между сессиями sess1, sess2                  |
| BySessions(STRING refno, REAL sess1, REAL sess2)  | ARRAY     | Сравнение атрибутов элемента refno (CE) между сессиями sess1, sess2                     |

## Пример использования

<code>Q REF
Ref =16401/21
Q HIST
History 90 89 88 84 4
import 'C:\code\.net\SessionCompare\SessionCompare\bin\Debug\SessionCompare'
handle any
endhandle
using namespace 'SessionCompare'
!sc = object SessionCompare()
!hist = !sc.BySessions('=16401/21', 4, 90)
Q VAR !hist
\<ARRAY>
   [1]  \<ARRAY> 4 Elements
   [2]  \<ARRAY> 4 Elements
   [3]  \<ARRAY> 4 Elements
   [4]  \<ARRAY> 4 Elements
   [5]  \<ARRAY> 4 Elements
Q VAR !hist[1]
\<ARRAY>
   [1]  \<STRING> 'DESC'
   [2]  \<STRING> 'Description of the element'
   [3]  \<STRING> 'unset'
   [4]  \<STRING> 'a1'</code>

## Пример использования PML формы
<code>show !!CompareSessions</code>