using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.RuleBasedApproval.SendIncomingInvoiceTo1CStage;

namespace Sungero.RuleBasedApproval.Server
{
  partial class SendIncomingInvoiceTo1CStageFunctions
  {
/// <summary>
    /// Создание входящего счета в 1С.
    /// </summary>
    /// <param name="approvalTask">Задача на согласование по регламенту.</param>
    /// <returns>Результат выполнения кода.</returns>
    public override Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      Logger.DebugFormat("SendIncomingInvoiceTo1CStage. Start sending the incoming invoice to 1C, approval task (ID={0}) (StartId={1}) (Iteration={2}) (StageNumber={3}).",
                         approvalTask.Id, approvalTask.StartId, approvalTask.Iteration, approvalTask.StageNumber);
      
      var mainDocument = approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault();
      if (mainDocument == null)
      {
        Logger.ErrorFormat("SendIncomingInvoiceTo1CStage. Primary document is not found. Approval task (ID={0}) (StartId={1}) (Iteration={2}) (StageNumber={3}).",
                           approvalTask.Id, approvalTask.StartId, approvalTask.Iteration, approvalTask.StageNumber);
        return this.GetErrorResult(Docflow.Resources.PrimaryDocumentNotFoundError);
      }
      
      var invoices = GetIncomingInvoicesForSyncTo1C(approvalTask);
      if (!invoices.Any())
      {
        Logger.DebugFormat("SendIncomingInvoiceTo1CStage. Incoming invoices are not found. Approval task (ID={0}).", approvalTask.Id);
        return this.GetSuccessResult();
      }
      
      var needRetry = false;
      foreach (var invoice in invoices)
      {
        try
        {
          var created = Sungero.Integration1CDemo.PublicFunctions.Module.CreateIncomingInvoice1C(invoice);
          if (created)
            Logger.DebugFormat("SendIncomingInvoiceTo1CStage. The incoming invoice is succcesssfully sent to 1C. Approval task (ID={0}), Document (ID={1}).", approvalTask.Id, invoice.Id);
          else
            Logger.DebugFormat("SendIncomingInvoiceTo1CStage. The incoming invoice is not sent to 1C. Approval task (ID={0}), Document (ID={1}).", approvalTask.Id, invoice.Id);
        }
        catch (Exception ex)
        {
          needRetry = true;
          Logger.ErrorFormat("SendIncomingInvoiceTo1CStage. Error while creating incoming invoice in 1C. Approval task (ID={0}) (Iteration={1}) (StageNumber={2}) for document (ID={3})",
                             ex, approvalTask.StartId, approvalTask.Iteration, approvalTask.StageNumber, invoice.Id);
        }
      }
      
      if (needRetry)
        return this.GetRetryResult(string.Empty);
      
      return this.GetSuccessResult();
    }

    /// <summary>
    /// Получить входящие счета для отправки в 1С.
    /// </summary>
    /// <param name="approvalTask">Задача на согласование по регламенту.</param>
    /// <returns>Список входящих счетов.</returns>
    /// <remarks>Получает основной документ задачи на согласование, если это входящий счет.
    /// Получает все входящие счета из группы "Приложения", но только со статусом "Принят к оплате".
    /// Счета из группы "Дополнительно" игнорируются.</remarks>
    private static List<Sungero.Integration1CDemo.IIncomingInvoice> GetIncomingInvoicesForSyncTo1C(Sungero.Docflow.IApprovalTask approvalTask)
    {
      var result = new List<Sungero.Integration1CDemo.IIncomingInvoice>();
      
      var mainDocument = approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault();
      if (mainDocument != null && Sungero.Integration1CDemo.IncomingInvoices.Is(mainDocument))
        result.Add(Sungero.Integration1CDemo.IncomingInvoices.As(mainDocument));
      
      var addendaInvoices = approvalTask.AddendaGroup.OfficialDocuments
        .Where(x => Sungero.Integration1CDemo.IncomingInvoices.Is(x) && x.LifeCycleState == Sungero.Contracts.IncomingInvoice.LifeCycleState.Active)
        .Select(x => Sungero.Integration1CDemo.IncomingInvoices.As(x));
      result.AddRange(addendaInvoices);
      
      return result;
    }    
  }
}