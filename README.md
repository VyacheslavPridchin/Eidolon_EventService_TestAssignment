# Документация

<p align="center">
  <img width="500" src="materials/eidolon_logo.png">
</p>

## Оглавление
1. [Обзор](#обзор)
2. [Зависимости](#зависимости)
3. [Описание классов](#описание-классов)
4. [Применяемые практики](#применяемые-практики)
5. [Пример использования](#пример-использования)

## Обзор

Этот проект представляет собой тестовое задание для компании EIDOLON, целью которого является создание кроссплатформенного сервиса для отслеживания и отправки событий на сервер аналитики в Unity. Сервис обеспечивает совместимость с различными платформами и реализует функционал для обработки и отправки событий. `EventService` управляет процессом отслеживания событий, накопления их в течение заданного времени кулдауна и последующей отправки на сервер. Также гарантируется надежная доставка событий с повторной отправкой при следующем запуске приложения в случае сбоя.

> Вы можете использовать `TestScript` для тестирования решения. Взаимодействуйте с ним через компонент на сцене.



### Объекты на сцене
- `TestScript`: Имеет несколько кнопок, используется для тестирования сервиса;
- `EventService`: Сервис ивентов, настраиваются ссылки и `cooldownBeforeSend` в секундах;
- `NetworkManager`: Менеджер сети, настраивается `serverUrl`.
 
### Пространства имён и классы

```
Eidolon
├── Events
│   ├── class EventService
│   ├── class Event
│   └── Storage
│       ├── Abstract
│       │   ├── interface IEventsStorageProvider
│       │   └── abstract class EventsStorageProviderBase
│       ├── class EventsPlayerPrefsStorageProvider
│       └── class EventsFileStorageProvider
└── Network
    ├── class NetworkManager
    └── class Payload<T>
```

### Иерархия файлов
  
```
Assets
├── 📂 Scenes
│   └── ⏯️ SampleScene.scene
├── 📂 Scripts
│   ├── 📄 TestScript.cs
│   ├── 📂 EventService
│   │   ├── 📄 EventService.cs
│   │   ├── 📄 Event.cs
│   │   └── 📂 StorageProviders
│   │       ├── 📄 EventsFileStorageProvider.cs
│   │       ├── 📄 EventsPlayerPrefsStorageProvider.cs
│   │       └── 📂 Abstract
│   │           ├── 📄 IEventsStorageProvider.cs
│   │           └── 📄 EventsStorageProviderBase.cs
│   └── 📂 Network
│       ├── 📄 NetworkManager.cs
│       └── 📄 Payload.cs
└── 📂 Events Storage Providers
    ├── 💾 EventsFileStorage.asset
    └── 💾 EventsPlayerPrefsStorage.asset
```

## Зависимости

- **Easy Buttons:** Используется для удобства вызова методов в редакторе Unity (для UnityEditor). [Easy Buttons на GitHub](https://github.com/madsbangh/EasyButtons)
- **Newtonsoft.Json:** Используется для сериализации объектов в JSON. [Newtonsoft.Json для Unity на GitHub](https://github.com/applejag/Newtonsoft.Json-for-Unity)

## Описание классов

### Events

- **EventService**

  Управляет процессом отслеживания, хранения и отправки событий. Обрабатывает события, накопленные в течение кулдауна, и отправляет их на сервер. Обеспечивает гарантированную доставку событий.

  **Методы:**
  - `TrackEvent(string type, string data)`
    
    Принимает тип и данные события в виде строковых параметров. Событие автоматически обрабатывается и отправляется на сервер после истечения времени кулдауна. Внутренние процессы, такие как накопление событий и управление кулдауном, выполняются автоматически.

  - `TrackEvent(Event event)`
    
    Принимает объект `Event` с типом и данными события. Работает аналогично вышеописанному методу. Этот метод упрощает работу с событиями, уже представляющими собой объект.

- **Event**

  Представляет событие с типом и данными. Используется для формирования и отправки событий на сервер.

  **Схема класса:**
  ```csharp
  class Event
  {
      string Type;
      string Data;
  
      Event(string type, string data);
  }
  ```

### Events.Storage

- **IEventsStorageProvider**

  Интерфейс, определяющий методы для провайдеров хранения событий. Позволяет реализовать различные способы хранения событий.

  - **EventsStorageProviderBase**

    Абстрактный ScriptableObject класс для провайдеров хранения событий. Наследуется от `IEventsStorageProvider`. Определяет интерфейс для добавления, получения и удаления событий, что позволяет создавать новые провайдеры хранения данных без изменения основного кода.

    - **EventsPlayerPrefsStorageProvider**

      Наследуется от `EventsStorageProviderBase`. Реализация провайдера хранения событий с использованием PlayerPrefs. Сохраняет события в виде строковых данных в PlayerPrefs.

    - **EventsFileStorageProvider**

      Наследуется от `EventsStorageProviderBase`. Реализация провайдера хранения событий с использованием файловой системы. Сохраняет события в файле JSON на устройстве.
   
> Вы можете реализовать свой провайдер хранения событий. При разработке также должен наследоваться от `EventsStorageProviderBase`.

### Network

- **NetworkManager**

  Управляет сетевыми запросами, включая отправку данных событий на сервер. Поддерживает асинхронные операции для повышения производительности.

- **Payload<T>**

  Обеспечивает универсальную сериализацию данных в JSON. Позволяет легко отправлять объекты любых типов на сервер.

  **Схема класса:**
  ```csharp
  class Payload<T>
  {
      Dictionary<string, T> Data;
  
      Payload(string key, T data);
      void SetPayload(string key, T data);
      string GetJson();
  }
  ```

## Применяемые практики

### Архитектурные решения

1. **Разделение ответственности:**
   - Четкое разделение между отслеживанием событий, их хранением и сетевыми запросами делает код более поддерживаемым и расширяемым.
     - `EventService` отвечает за логику работы с событиями.
     - `EventsStorageProviderBase` и его реализации отвечают за хранение данных.
     - `NetworkManager` отвечает за сетевые взаимодействия.

2. **Гибкость добавления новых способов хранения данных:**
   - Абстрактный класс `EventsStorageProviderBase` и интерфейс `IEventsStorageProvider` предоставляют интерфейс для различных методов хранения событий. Это позволяет легко интегрировать новые провайдеры хранения, такие как базы данных или облачные сервисы, без необходимости изменения основного кода.

### Технические решения

1. **Асинхронные операции:**
   - Использование `async` и `await` в `NetworkManager` для выполнения сетевых запросов позволяет не блокировать основной поток выполнения, что особенно важно для поддержания высокой производительности в игровых приложениях.

2. **Универсальная сериализация объектов:**
   - Класс `Payload<T>` обеспечивает универсальную и автоматизированную сериализацию любых объектов в формат JSON. Это позволяет легко отправлять различные типы данных на сервер без изменения основного кода приложения.

3. **Гарантированная доставка событий:**
   - Сервис гарантирует доставку событий на сервер, используя кулдаун для уменьшения количества запросов и обеспечивая повторную отправку событий в случае сбоя или перезапуска приложения.
  
## Пример использования

  ```csharp
using Eidolon.Events;
using UnityEngine;
using Event = Eidolon.Events.Event;

public class Example : MonoBehaviour
{
    [SerializeField]
    private EventService eventService;

    private void Start()
    {
        TrackExampleEvents();
    }

    private void TrackExampleEvents()
    {
        Event newEvent = new Event("eventFirst", "first_data:1");
        eventService.TrackEvent(newEvent);
        eventService.TrackEvent("eventSecond", "second_data:22");
    }
}

  ```
