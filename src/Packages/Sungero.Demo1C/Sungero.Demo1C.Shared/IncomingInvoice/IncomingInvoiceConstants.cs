using System;
using Sungero.Core;

namespace Sungero.Demo1C.Constants.Contracts
{
  public static class IncomingInvoice
  {
    /// <summary>
    /// Ставки НДС в Диадок.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed.")]
    public static class VatRatesFromDiadoc
    {
      public const string NoVat = "без НДС";
      public const string Vat0 = "0%";
      public const string Vat5 = "5%";
      public const string Vat5_105 = "5/105";
      public const string Vat7 = "7%";
      public const string Vat7_107 = "7/107";
      public const string Vat10 = "10%";
      public const string Vat10_110 = "10/110";
      public const string Vat18 = "18%";
      public const string Vat18_118 = "18/118";
      public const string Vat20_120 = "20/120";
      public const string Vat20 = "20%";
    }
    
    /// <summary>
    /// Ставки НДС в 1С.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed.")]
    public static class VatRatesFrom1C
    {
      public const string NoVat = "БезНДС";
      public const string Vat0 = "НДС0";
      public const string Vat5 = "НДС5";
      public const string Vat5_105 = "НДС5_105";
      public const string Vat7 = "НДС7";
      public const string Vat7_107 = "НДС7_107";
      public const string Vat10 = "НДС10";
      public const string Vat10_110 = "НДС10_110";
      public const string Vat18 = "НДС18";
      public const string Vat18_118 = "НДС18_118";
      public const string Vat20_120 = "НДС20_120";
      public const string Vat20 = "НДС20";
    }
  }
}