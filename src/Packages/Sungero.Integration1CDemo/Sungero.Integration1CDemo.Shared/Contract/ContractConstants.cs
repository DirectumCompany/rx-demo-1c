using System;
using Sungero.Core;

namespace Sungero.Integration1CDemo.Constants.Contracts
{
  public static class Contract
  {
    // Sid отметки "Утверждено".
    [Public]
    public const string PaginalApproveMarkKindSid = "3cdb9932-708f-4079-bc50-890b700202c6";
    
    // Полное имя класса, из которого вызывается метод получения отметки "Утверждено".
    [Public]
    public const string PaginalApproveMarkKindClass = "Sungero.Integration1CDemo.Functions.Contract";
    
    // Имя метода получения отметки "Утверждено".
    [Public]
    public const string PaginalApproveMarkKindMethod = "GetApprovedMarkAsHtml";
  }
}