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
  [System.Diagnostics.CodeAnalysis.SuppressMessage("AppliedStylecopNamingRules.ApiNamingAnalyzer", "CR0001:ApiNamesMustNotContainCyrillic", Justification = "Reviewed.")]
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
    /// Идентификатор сущности RX.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
    public long rx_ID { get; set; }
    
    /// <summary>
    /// Идентификатор в 1С.
    /// </summary>
    public string Ref_Key { get; set; }
  }
  
  /// <summary>
  /// Документ "Поступление (акты, накладные, УПД)".
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("AppliedStylecopNamingRules.ApiNamingAnalyzer", "CR0001:ApiNamesMustNotContainCyrillic", Justification = "Reviewed.")]
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
    public long rx_ID { get; set; }
    
    /// <summary>
    /// Идентификатор в 1С.
    /// </summary>
    public string Ref_Key { get; set; }
    
    /// <summary>
    /// Признак "УПД".
    /// </summary>
    public bool ЭтоУниверсальныйДокумент { get; set; }
    
    /// <summary>
    /// Вид операции.
    /// </summary>
    public string ВидОперации { get; set; }
  }
  
  #endregion
  
  /// <summary>
  /// Регистр сведений "Сроки оплаты документов".
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("AppliedStylecopNamingRules.ApiNamingAnalyzer", "CR0001:ApiNamesMustNotContainCyrillic", Justification = "Reviewed.")]
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
  /// Услуга в табличной части счета от поставщика.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("AppliedStylecopNamingRules.ApiNamingAnalyzer", "CR0001:ApiNamesMustNotContainCyrillic", Justification = "Reviewed.")]
  [Public]
  partial class ServiceLineDto
  {
    /// <summary>
    /// Порядковый номер услуги в таблице счета.
    /// </summary>
    public string LineNumber { get; set; }
    
    /// <summary>
    /// Содержание.
    /// </summary>
    public string Содержание { get; set; }
    
    /// <summary>
    /// Количество.
    /// </summary>
    public string Количество { get; set; }
    
    /// <summary>
    /// Цена.
    /// </summary>
    public string Цена { get; set; }
    
    /// <summary>
    /// Сумма.
    /// </summary>
    public string Сумма { get; set; }
    
    /// <summary>
    /// Ставка НДС.
    /// </summary>
    public string СтавкаНДС { get; set; }
    
    /// <summary>
    /// Сумма НДС.
    /// </summary>
    public string СуммаНДС { get; set; }
  }
  
  /// <summary>
  /// Регистр сведений "Статусы документов".
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("AppliedStylecopNamingRules.ApiNamingAnalyzer", "CR0001:ApiNamesMustNotContainCyrillic", Justification = "Reviewed.")]
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