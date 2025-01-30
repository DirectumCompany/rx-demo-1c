# rx-demo-1C
Решение представляет собой ограниченную демонстрационную версию интеграции базового решения DirectumRX c 1C-Бухгалтерией.

## Назначение
Решение направлено на достижение двух задач: 
+ Демонстрация типовых бизнес-процессов на примере:
    - От заключения договора с покупателем до реализации товаров и услуг;
    - От заключения договора с поставщиком до поступления товаров и услуг.
+ Предоставление примеров кода для разработчиков, которые демонстрируют возможности интеграции с системой 1С-Бухгалтерия.
 
## Описание решения

### Варианты процессов
Для демонстрации типовых бизнес-процессов в решении содержатся соответствующие варианты процессов:
+ Согласование документов, которые компания отправляет контрагенту:
  - "Процесс согласования договорных документов (интеграция с 1С)";
  - "Процесс согласования исходящих счетов (интеграция с 1С)";
  - "Процесс согласования исходящих УПД (интеграция с 1С)".
+ Согласование документов, которые контрагент отправляет компании:
  - "Процесс согласования входящих счетов (интеграция с 1С)";
  - "Процесс согласования входящих УПД (интеграция с 1С)".
Данные ВП автоматически загружаются в систему при применении настроек на сервер.

### No-code блоки
Для построения демонстрационных вариантов процессов в решении предоставлены следующие no-code блоки:
+ "Входящий счет. Передача в 1С статуса "Оплачен"";
+ "Исходящий счет. Передача в 1С статуса "Оплачен"";
+ "УПД. Передача в 1С признака "Документ подписан"";
+ "Входящий счет. Передача в 1С";
+ "Договор. Передача в 1С";
+ "УПД. Передача в 1С".

### Открытие связанных записей 
Открытие связанных записей в 1С позволяет переходить к связанным документам из DirectumRX. Поддерживаются следующие типы документов:
+ Универсальный передаточный документ (УПД);
+ Договор;
+ Входящий счет;
+ Исходящий счет.

### Этапы регламента
Этап регламента "Создание входящего счета в 1С". В рамках данного этапа создается входящий счет в 1С на основе данных входящего счета Directum RX.

## Варианты расширения функциональности и примеры кода
Решение rx-demo-1c состоит из трех модулей: 
+ ExternalSystem - модуль, ответственный за работу с внешней системой 1С-Бухгалтерия. Для обмена данными используется протокол OData.
+ NoCodeApproval - модуль, содержащий блок-скрипты для no-code и отвечающий за формирование сущностей в формате, поддерживаемом 1С-Бухгалтерия для обмена с системой. 
+ RuleBasedApproval - модуль, в состав которого входит справочник "SendIncomingInvoiceTo1CStage", являющийся этапом регламента для задачи на согласование по регламенту.
> [!NOTE]  
> Модуль RuleBasedApproval и его функциональность не используются при демонстрации типовых  бизнес - процессов. Его реализация оставлена в решении в качестве демонстрации возможности работы с 1С-Бухгалтерия при создании этапов регламента. 

Кроме модулей, решение содержит ряд перекрытых документов для возможности открытия связанных записей и подготовки данных к отправке, а также справочник "ExternalEntityLink" для создания и получения связей с внешней системой.

### Пример открытия связанной записи в системе 1С
Для открытия связанной записи в 1С необходимо, чтобы соответствующий документ в системе был синхронизирован (имелась соответствующая запись в ExternalEntityLink).
Для открытия документа по связанной ссылки используется метод [OpenEntityIn1CFor](https://github.com/DirectumCompany/rx-demo-1c/blob/master/src/Packages/Sungero.Demo1C/Sungero.Demo1C.ClientBase/ModuleClientFunctions.cs#L17-L39).

Пример для договора - [OpenEntityIn1CSungero](https://github.com/DirectumCompany/rx-demo-1c/blob/master/src/Packages/Sungero.Demo1C/Sungero.Demo1C.ClientBase/Contract/ContractActions.cs#L12-L22).

 Пример для входящего счета - [OpenEntityIn1CSungero](https://github.com/DirectumCompany/rx-demo-1c/blob/master/src/Packages/Sungero.Demo1C/Sungero.Demo1C.ClientBase/IncomingInvoice/IncomingInvoiceActions.cs#L12-L22). 

Реалзиация для других типов документов производится аналогичным образом.
### Пример создания сущности в формате 1С
Рассмотрим создание сущности на примере документа "Входящий счет". Для формирования документа используется метод [ConvertTo1cHeaderDto](https://github.com/DirectumCompany/rx-demo-1c/blob/master/src/Packages/Sungero.Demo1C/Sungero.Demo1C.Server/IncomingInvoice/IncomingInvoiceServerFunctions.cs#L19-L33), который формирует DTO для передачи в 1С. DTO представляет из себя [структуру](https://github.com/DirectumCompany/rx-demo-1c/blob/master/src/Packages/Sungero.ExternalSystem/Sungero.ExternalSystem.Shared/ModuleStructures.cs#L16-L58), в которой наименование полей соответствует связанному документу в 1С.
### Пример отправки статуса подписания в 1С
Для передачи статуса документа в 1С-Бухгалтерия необходимо подготовить сущность для отправки и иметь связанный документ в системе. Рассмотрим на примере передачи статуса УПД.
Для отправки статуса вызывается метод [SendDocumentStatusTo1C](https://github.com/DirectumCompany/rx-demo-1c/blob/master/src/Packages/Sungero.NoCodeApproval/Sungero.NoCodeApproval.Server/ModuleServerFunctions.cs#L17-L39), который вызывает метод [UpdateStatusForUniversalTransferDocument](https://github.com/DirectumCompany/rx-demo-1c/blob/master/src/Packages/Sungero.NoCodeApproval/Sungero.NoCodeApproval.Server/ModuleServerFunctions.cs#L63-L67). В методе происходит заполнение недостающих данных и происходит отправка статуса с использованием метода [UpdateDocumentStatus](https://github.com/DirectumCompany/rx-demo-1c/blob/master/src/Packages/Sungero.ExternalSystem/Sungero.ExternalSystem.Server/ModuleServerFunctions.cs#L213-#L226), который использует PATCH-запрос для обновления данных в соответствующем регистре 1С.
 
## Требования для установки решения

 + Directum RX 4.12 и выше.
 + 1С "Бухгалтерия предприятия" 3.0.164.20 и выше.
 + Расширение в 1С "Интеграционное расширение Directum RX".
 + Расширение в 1С "Интеграционное расширение Directum RX (демо)".

## Инструкция по установке

Инструкции для установки и настройки решения запрашивать через тех. поддержку Directum RX (доступно для тех, у кого куплен коннектор к 1С).

## Порядок демонстрации возможностей решения

Инструкцию по порядку демонстрации запрашивать через тех. поддержку Directum RX (доступно для тех, у кого куплен коннектор к 1С).

## Поддержка решения 

Разработкой и поддержкой решения занимается команда Enigma, отдел прикладной разработки RX. 
Вопросы, замечания, предложения оставляйте через Issues или запросите контактные данные у тех. поддержки.