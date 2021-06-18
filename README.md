# Карточная игра на основе боя двух "стеков" юнитов
#### Используя Unity

### Юниты
* Пехотинец (легкий юнит, небольшая атака и защита, низкая цена, может быть клонирован и может лечиться).
  * Спецдействие - умеет надевать "апы" (цит, шлем, пика и т.д.) на стоящих рядом рыцарей
* Лучник (легкий юнит, небольшая атака и защита, средняя цена, может быть клонирован и может лечиться).
  * Спецдействие - умеет стрелять из лука (средняя сила) на некоторую дальность
* Целитель (легкий юнит, небольшая атака и защита, высокая цена, не может быть клонирован, может лечиться).
  * Спецдействие - умеет лечить стоящих рядом юнитов
* Рыцарь (тяжелый юнит, большая атака и защита, высокая цена, не может быть клонирован, может лечиться).
  * Спецдействие - нет
  * Может носить "апы" (надевает пехотинец), увеличивающие его атаку или защиту. Они могут быть уничножены от удара противника с некоторой вероятностью. В один момент времени могут быть надеты лишь одни "апы"
* Перекати-поле (тяжелый юнит, нелевая атака и высокая защита, очень высокая цена, не может быть клонирован, не может лечиться).
  * Спецдействие - нет.
  * Должен быть внедрен из внешней библиотеки

##### Спецдействия могут применять только юниты, перед которыми не было войск противника в начале хода
##### Все юниты наследуют от интерфейса IUnit, определяющего свойства Id, Name, Attack, Defence, Cost, Health
 
 
 ### Общая механика игры
1. В начале создаются армии на некоторую сумму. Обе или одну из них можно создать автоматически (рандомно)
2. Далее каждый ход происходит после нажатия на кпопку хода. Есть возможность отмены хода и возврата хода
3. После победы одной из армий (все юниты другой армии уничтожены) выводится сообщение о победе (может быть ничья) и можно начать новую игру

#### Ход состоит из следующих действий
1. В начале рандомно выбирается сторона, идущая первой
2. Затем юнит этой стороны, стоящий во главе колонны, атакует юнита второй стороны, стоящего во главе своей колонны
3. Если атакуемый юнит остался жив, то он атакует атаковавшего его юнита в ответ
4. После этого юниты колонны стороны, ходящей первой, начиная со второго "ряда", поочередно используют свои спецспособности (спецдействие должно срабатывать не всегда, а с некоторой вероятностью)
5. Затем то же самое делает вторая колонна
6. В конце хода с поля убираются все уничтоженные юниты

### Игра может проходить в формации:
* колонна по одному на колонну по одному (вышеописанная механика хода относится в первую очередь к этой формации, для остальных будет добавлена информация позже)
* колонна по 3 на колонну по три
* стенка на стенку

### Дополнительные возможности игры:
* Воозможность отмены хода и возврата отмененного хода (то есть история ходов должна сохраняться)
* Возможность рандомно набрать одну или обе армии

## Колонная по одному
Юниты располагаются в горизонтальной колонне, первый юнит одной армии стоит напротив первого юнита другой армии. При уничтожении юнита его место занимает следующий
Лучник может рандомно попасть в любой юнит вражеской колонны. Доступными для применения спецдействий юнитами считаются то, что стоят справа и слева от применяющего (который не должен быть первым в колонне)

## Стенка на стенку
Юниты располагаются в вертикальной колонне колонне, начало колонны в верхней части поля. При уничтожении юниты снизу подтягиваются наверх. Первыми юнитами считаются те, напротив которых на том же уровне находятся юниты во вражеской колонне. Остальное так же, как в колонне по одному

## Колонна по три юнита
Юниты располагаются горизонтальной колонной по 3 юнита вертикально. Первые юниты - юниты первой колонны, стоящий напротив вражеских юнитов первой колонны. В каждой следующей колонне не может быть меньше юнитов, чем в предыдущей. При убийстве сначала подтягиваются юниты этой же колонны, стоящие ниже, а затем, при необходимости, юниты следующей колонны, начиная с ее конца. Стоящими рядом с юнитом юнитами считаются юниты этой же колонны выше и ниже юнита и юниты следующей и предыдущей колонн, стоящие на том же месте (на том же ряду).

![image](https://user-images.githubusercontent.com/63730899/122614773-f2f56000-d08f-11eb-8662-3e61a21427b8.png)
