using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.ExternalSystem.Structures.Module
{
  #region Документы
  
  /// <summary>
  /// Документ "Счета от поставщиков".
  /// </summary>
  [Public]
  partial class SupplierInvoiceDto
  {
    /// <summary>
    /// Номер счета.
    /// </summary>
    public string НомерВходящегоДокумента { get; set; }

    /// <summary>
    /// Дата счета.
    /// </summary>
    public DateTime ДатаВходящегоДокумента { get; set; }

    /// <summary>
    /// Организация.
    /// </summary>
    public string Организация_Key { get; set; }

    /// <summary>
    /// Контрагент.
    /// </summary>
    public string Контрагент_Key { get; set; }

    /// <summary>
    /// Договор.
    /// </summary>
    public string ДоговорКонтрагента_Key { get; set; }

    /// <summary>
    /// Комментарий.
    /// </summary>
    public string Комментарий { get; set; }
    
    /// <summary>
    /// ИД в Directum RX.
    /// </summary>
    public long rx_ID { get; set; }
    
    /// <summary>
    /// Идентификатор в 1С.
    /// </summary>
    public string Ref_Key { get; set; }
  }
  
  /// <summary>
  /// Документ "Поступление (акты, накладные, УПД)".
  /// </summary>
  [Public]
  partial class ReceiptDto
  {
    /// <summary>
    /// Номер счета.
    /// </summary>
    public string НомерВходящегоДокумента { get; set; }

    /// <summary>
    /// Дата счета.
    /// </summary>
    public DateTime ДатаВходящегоДокумента { get; set; }

    /// <summary>
    /// Организация.
    /// </summary>
    public string Организация_Key { get; set; }

    /// <summary>
    /// Контрагент.
    /// </summary>
    public string Контрагент_Key { get; set; }

    /// <summary>
    /// Договор.
    /// </summary>
    public string ДоговорКонтрагента_Key { get; set; }

    /// <summary>
    /// Комментарий.
    /// </summary>
    public string Комментарий { get; set; }
    
    /// <summary>
    /// ИД в Directum RX.
    /// </summary>
    public long rx_ID { get; set; }
    
    /// <summary>
    /// Идентификатор в 1С.
    /// </summary>
    public string Ref_Key { get; set; }
    
    /// <summary>
    /// Признак "УПД"
    /// </summary>
    public bool ЭтоУниверсальныйДокумент { get { return true; } }
  }
  
  #endregion
  
  /// <summary>
  /// Регистр сведений "Сроки оплаты документов".
  /// </summary>
  [Public]
  partial class PaymentTermDto
  {
    /// <summary>
    /// Организация.
    /// </summary>
    public string Организация_Key { get; set; }
    
    /// <summary>
    /// Документ.
    /// </summary>
    public string Документ { get; set; }
    
    /// <summary>
    /// Тип документа.
    /// </summary>
    public string Документ_Type { get; set; }
    
    /// <summary>
    /// Срок оплаты.
    /// </summary>
    public DateTime СрокОплаты { get; set; }
  }
  
  /// <summary>
  /// Регистр сведений "Статусы документов".
  /// </summary>
  [Public]
  partial class DocumentStatusDto
  {
    /// <summary>
    /// Организация.
    /// </summary>
    public string Организация_Key { get; set; }
    
    /// <summary>
    /// Документ.
    /// </summary>
    public string Документ { get; set; }
    
    /// <summary>
    /// Тип документа.
    /// </summary>
    public string Документ_Type { get; set; }

    /// <summary>
    /// Статус.
    /// </summary>
    public string Статус { get; set; }
    
    /// <summary>
    /// Тип статуса.
    /// </summary>
    public string Статус_Type { get; set; }
  }
}