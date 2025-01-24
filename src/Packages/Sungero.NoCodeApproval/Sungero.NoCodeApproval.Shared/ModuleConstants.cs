using System;
using Sungero.Core;

namespace Sungero.NoCodeApproval.Constants
{
  public static class Module
  {
    /// <summary>
    /// Guid типа Contracts.OutgoingInvoice.
    /// </summary>
    public static readonly Guid OutgoingInvoiceTypeGuid = Guid.Parse("58AD01FB-6805-426B-9152-4DE16D83B258");
    
    /// <summary>
    /// Уникальный идентификатор для ExternalLink типа документа (Docflow.DocumentType) OutgoingInvoice.
    /// </summary>
    public static readonly Guid OutgoingInvoiceExternalEntityId = Guid.Parse("73DD30CD-1C0F-4BA7-A44D-BE2F6BBE6E16");
  }
}