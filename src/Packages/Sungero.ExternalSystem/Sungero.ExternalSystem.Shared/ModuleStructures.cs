using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.ExternalSystem.Structures.Module
{
  /// <summary>
  /// Входящий счет в 1С.
  /// </summary>
  [Public]
  partial class IncomingInvoiceDto
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
    
    public long rx_ID {get; set;}
    
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public string Ref_Key { get; set; }
  }
  
  /// <summary>
  /// Срок оплаты в 1С.
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
}